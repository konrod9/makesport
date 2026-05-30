using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Contracts;
using FileService.Domain;
using FileService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FileService.Application.UseCases.GetMediaAssets;

public class GetMediaAssetsUseCase
{
    private readonly IReadDbContext _readDbContext;
    private readonly IFileStorageProvider _fileStorageProvider;

    public GetMediaAssetsUseCase(
        IReadDbContext readDbContext,
        IFileStorageProvider fileStorageProvider)
    {
        _readDbContext = readDbContext;
        _fileStorageProvider = fileStorageProvider;
    }

    public async Task<Result<GetMediaAssetsResponse, Error>> Handle(GetMediaAssetsRequest request, CancellationToken ct)
    {
        if (request.MediaAssetIds.Count == 0)
            return new GetMediaAssetsResponse([]);

        var mediaAssets = await _readDbContext.MediaAssetsQuery
            .Where(m => request.MediaAssetIds.Contains(m.Id) && m.Status != MediaStatus.Deleted)
            .ToListAsync(cancellationToken: ct);
        
        var readyMediaAssets = mediaAssets.Where(m => m.Status == MediaStatus.Ready).ToList();
        var keys = readyMediaAssets.Select(m => m.Key).ToList();
        
        var (_, isFailure, urls, error) = await _fileStorageProvider.GenerateDownloadUrlsAsync(keys, ct);
        if (isFailure)
            return error;

        var urlsDict = urls.ToDictionary(u => u.StorageKey, u => u.PresignedUrl);

        var results = new List<GetMediaAssetDto>();
        foreach (var mediaAsset in mediaAssets)
        {
            urlsDict.TryGetValue(mediaAsset.Key, out var url);
            
            var mediaAssetDto = new GetMediaAssetDto(
                mediaAsset.Id,
                mediaAsset.Status.ToString(),
                mediaAsset.AssetType.ToString(),
                url);
            
            results.Add(mediaAssetDto);
        }
        
        return new GetMediaAssetsResponse(results);
    }
}