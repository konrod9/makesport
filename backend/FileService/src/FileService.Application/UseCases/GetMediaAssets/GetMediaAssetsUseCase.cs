using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Contracts;
using FileService.Domain;
using FileService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileService.Application.UseCases.GetMediaAssets;

public class GetMediaAssetsUseCase
{
    private readonly IReadDbContext _readDbContext;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly HybridCache _cache;
    private readonly FileStorageOptions _options;

    public GetMediaAssetsUseCase(
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

    public async Task<Result<GetMediaAssetsResponse, Error>> Handle(
        GetMediaAssetsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.MediaAssetIds.Count == 0)
            return new GetMediaAssetsResponse([]);

        var mediaAssets = await _readDbContext.MediaAssetsQuery
            .Where(m => request.MediaAssetIds.Contains(m.Id) && m.Status != MediaStatus.Deleted)
            .ToListAsync(cancellationToken: cancellationToken);

        var readyMediaAssets = mediaAssets.Where(m => m.Status == MediaStatus.Ready).ToList();
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
                downloadUrl);

            results.Add(mediaAssetDto);
        }

        return new GetMediaAssetsResponse(results);
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