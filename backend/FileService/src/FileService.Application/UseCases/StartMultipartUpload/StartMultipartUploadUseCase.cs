using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Domain;
using FileService.Domain.Assets;
using FileService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace FileService.Application.UseCases.StartMultipartUpload;

public class StartMultipartUploadUseCase
{
    private readonly ILogger<StartMultipartUploadUseCase> _logger;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IChunkSizeCalculator _chunkSizeCalculator;
    private readonly IMediaAssetsRepository _mediaAssetsRepository;

    public StartMultipartUploadUseCase(
        ILogger<StartMultipartUploadUseCase> logger,
        IFileStorageProvider fileStorageProvider,
        IChunkSizeCalculator chunkSizeCalculator, IMediaAssetsRepository mediaAssetsRepository)
    {
        _logger = logger;
        _fileStorageProvider = fileStorageProvider;
        _chunkSizeCalculator = chunkSizeCalculator;
        _mediaAssetsRepository = mediaAssetsRepository;
    }

    public async Task<Result<StartMultipartUploadResponse, Error>> Handle(StartMultipartUploadRequest request,
        CancellationToken cancellationToken)
    {
        var fileNameResult = FileName.Create(request.FileName);
        if (fileNameResult.IsFailure)
            return fileNameResult.Error;

        var contentTypeResult = ContentType.Create(request.ContentType);
        if (contentTypeResult.IsFailure)
            return contentTypeResult.Error;

        var chunkCalculationResult = _chunkSizeCalculator.CalculateChunkSize(request.Size);
        if (chunkCalculationResult.IsFailure)
            return chunkCalculationResult.Error;

        var mediaDataResult = MediaData.Create(
            fileNameResult.Value,
            contentTypeResult.Value,
            request.Size,
            chunkCalculationResult.Value.TotalChunks
        );

        var mediaAssetResult = MediaAsset.CreateForUpload(mediaDataResult.Value, request.AssetType.ToAssetType());

        await _mediaAssetsRepository.AddAsync(mediaAssetResult.Value, cancellationToken);

        var startUploadResult = await _fileStorageProvider.StartMultipartUploadAsync(
            mediaAssetResult.Value.Key,
            mediaAssetResult.Value.MediaData,
            cancellationToken);
        if (startUploadResult.IsFailure)
            return startUploadResult.Error;

        var chunkUploadUrlsResult = await _fileStorageProvider.GenerateAllChunksUploadUrlsAsync(
            mediaAssetResult.Value.Key,
            startUploadResult.Value,
            chunkCalculationResult.Value.TotalChunks,
            cancellationToken);
        if (chunkUploadUrlsResult.IsFailure)
            return chunkUploadUrlsResult.Error;

        _logger.LogInformation("Media Asset started uploading with id {MediaAssetId}, key: {StorageKey}",
            mediaAssetResult.Value.Id,
            mediaAssetResult.Value.Key);
        
        return new StartMultipartUploadResponse(
            mediaAssetResult.Value.Id,
            startUploadResult.Value,
            chunkUploadUrlsResult.Value,
            chunkCalculationResult.Value.ChunkSize);
    }
}