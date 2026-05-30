using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Contracts;
using FileService.Domain;
using FileService.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace FileService.Application.UseCases.GetMediaAssetInfo;

public class GetMediaAssetInfoUseCase
{
    private readonly IReadDbContext _readDbContext;
    private readonly IFileStorageProvider _fileStorageProvider;

    public GetMediaAssetInfoUseCase(IReadDbContext readDbContext, IFileStorageProvider fileStorageProvider)
    {
        _readDbContext = readDbContext;
        _fileStorageProvider = fileStorageProvider;
    }

    public async Task<Result<GetMediaAssetInfoDto?, Error>> Handle(
        Guid mediaAssetId, 
        CancellationToken cancellationToken)
    {
        var mediaAsset = await _readDbContext.MediaAssetsQuery
            .FirstOrDefaultAsync(m => m.Id == mediaAssetId, cancellationToken);
        if (mediaAsset == null)
            return Result.Success<GetMediaAssetInfoDto?, Error>(null);
        
        string? url = null;

        if (mediaAsset.Status == MediaStatus.Ready)
        {
            var (_, isFailure, presignedUrl, error) = await _fileStorageProvider.GenerateDownloadUrlAsync(mediaAsset.Key);
            if (isFailure)
                return error;
            
            url = presignedUrl;
        }

        var mediaAssetInfoDto = new GetMediaAssetInfoDto(
            mediaAsset.Id,
            mediaAsset.Status.ToString().ToLowerInvariant(),
            mediaAsset.AssetType.ToString().ToLowerInvariant(),
            url,
            mediaAsset.MediaData.Size,
            mediaAsset.MediaData.FileName.Value,
            mediaAsset.MediaData.ContentType.Value);
        
        return mediaAssetInfoDto;
    }
}