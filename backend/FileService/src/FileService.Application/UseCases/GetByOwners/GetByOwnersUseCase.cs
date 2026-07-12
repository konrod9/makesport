using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Contracts.Dtos;
using FileService.Contracts.Shared;
using FileService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace FileService.Application.UseCases.GetByOwners;

public class GetByOwnersUseCase
{
    private readonly IReadDbContext _readDbContext;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly HybridCache _cache;
    private readonly FileStorageOptions _options;

    public GetByOwnersUseCase(
        IReadDbContext readDbContext, 
        IFileStorageProvider fileStorageProvider, 
        HybridCache cache,
        IOptions<FileStorageOptions> options)
    {
        _readDbContext = readDbContext;
        _fileStorageProvider = fileStorageProvider;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<Result<GetByOwnersResponse, Error>> Handle(
        GetByOwnersRequest request,
        CancellationToken cancellationToken)
    {
        if (request.OwnerIds.Count == 0)
            return new GetByOwnersResponse([]);

        var ownerIds = request.OwnerIds.ToHashSet();

        var mediaAssets = await _readDbContext.MediaAssetsQuery
            .Where(m => ownerIds.Contains(m.OwnerId)
                        && m.Status != MediaStatus.Deleted
                        && m.OwnerType == request.OwnerType)
            .ToListAsync(cancellationToken);

        var readyMediaAssets = mediaAssets.Where(m => m.Status == MediaStatus.Uploaded).ToList();
        var keys = readyMediaAssets.Select(m => m.Key).ToList();

        var presignedUrl = await GetPresignedUrlsFromCache(keys, cancellationToken);

        var (_, isFailure, urls, error) = await _fileStorageProvider.GenerateDownloadUrlsAsync(keys, cancellationToken);
        if (isFailure)
            return error;

        var urlsDict = urls.ToDictionary(u => u.StorageKey, u => u.PresignedUrl);

        var results = new List<GetMediaAssetDto>();
        foreach (var mediaAsset in mediaAssets)
        {
            string? downloadUrl = null;

            if (urlsDict.TryGetValue(mediaAsset.Key, out var url))
            {
                downloadUrl = url;
            }

            var mediaAssetDto = new GetMediaAssetDto(
                mediaAsset.Id,
                mediaAsset.Status.ToString(),
                mediaAsset.AssetType.ToString(),
                mediaAsset.OwnerId,
                mediaAsset.OwnerType,
                downloadUrl);

            results.Add(mediaAssetDto);
        }

        return new GetByOwnersResponse(results);
    }

    private async Task<Dictionary<StorageKey, string>> GetPresignedUrlsFromCache(
        IEnumerable<StorageKey> storageKeys,
        CancellationToken cancellationToken)
    {
        var keys = storageKeys.ToList();

        if (keys.Count == 0)
            return [];

        var cachedUrlsTasks = keys.Select(async key =>
        {
            var url = await _cache.GetOrCreateAsync(
                key.Value,
                factory: _ => ValueTask.FromResult<string?>(null),
                options: new HybridCacheEntryOptions()
                {
                    Expiration = TimeSpan.FromDays(_options.DownloadUrlExpirationDays)
                        .Subtract(TimeSpan.FromHours(1))
                },
                cancellationToken: cancellationToken);

            return (key, url);
        });

        var cachedUrls = await Task.WhenAll(cachedUrlsTasks);

        var presignedUrls = new Dictionary<StorageKey, string>();
        var keysToGenerate = new List<StorageKey>();

        foreach (var (key, url) in cachedUrls)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                presignedUrls[key] = url;
            }
            else
            {
                keysToGenerate.Add(key);
            }
        }

        if (keysToGenerate.Count == 0)
            return presignedUrls;

        var mediaUrlsResult = await _fileStorageProvider.GenerateDownloadUrlsAsync(keysToGenerate, cancellationToken);
        if (mediaUrlsResult.IsFailure)
            return presignedUrls;

        var setTasks = mediaUrlsResult.Value.Select(async mediaUrl =>
        {
            presignedUrls[mediaUrl.StorageKey] = mediaUrl.PresignedUrl;

            await _cache.SetAsync(
                key: mediaUrl.StorageKey.Value,
                value: mediaUrl.PresignedUrl,
                options: new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromDays(_options.DownloadUrlExpirationDays)
                        .Subtract(TimeSpan.FromHours(1))
                },
                cancellationToken: cancellationToken);
        });

        await Task.WhenAll(setTasks);

        return presignedUrls;
    }
}