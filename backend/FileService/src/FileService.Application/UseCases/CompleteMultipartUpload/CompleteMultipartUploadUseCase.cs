using CSharpFunctionalExtensions;
using FileService.Application.FilesStorage;
using FileService.Contracts.Shared;
using Microsoft.Extensions.Logging;

namespace FileService.Application.UseCases.CompleteMultipartUpload;

public class CompleteMultipartUploadUseCase
{
    private readonly IMediaAssetsRepository _mediaAssetsRepository;
    private readonly ILogger<CompleteMultipartUploadUseCase> _logger;
    private readonly IFileStorageProvider _fileStorageProvider;

    public CompleteMultipartUploadUseCase(
        IMediaAssetsRepository mediaAssetsRepository,
        ILogger<CompleteMultipartUploadUseCase> logger,
        IFileStorageProvider fileStorageProvider)
    {
        _mediaAssetsRepository = mediaAssetsRepository;
        _logger = logger;
        _fileStorageProvider = fileStorageProvider;
    }

    public async Task<UnitResult<Error>> Handle(
        CompleteMultipartUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        var (_, isFailure, mediaAsset, error) = await _mediaAssetsRepository
            .GetBy(m => m.Id == request.MediaAssetId, cancellationToken);
        if (isFailure)
            return error;
        
        if (mediaAsset.MediaData.ExpectedChunksCount != request.PartETags.Count)
            return GeneralErrors.Failure($"Chunk count mismatch. Expected {mediaAsset.MediaData.ExpectedChunksCount} but received {request.PartETags.Count}.");
        
        var completeResult = await _fileStorageProvider.CompleteMultipartUploadAsync(
            mediaAsset.Key,
            request.UploadId,
            request.PartETags, 
            cancellationToken);
        if (completeResult.IsFailure)
        {
            mediaAsset.MarkFailed();
            await _mediaAssetsRepository.SaveAsync(cancellationToken);
            
            _logger.LogError("Failed to complete multipart upload for media asset with id {MediaAssetId}. Error: {Error}", 
                mediaAsset.Id, completeResult.Error);
            
            return completeResult.Error;
        }

        mediaAsset.MarkUploaded();
        
        await _mediaAssetsRepository.SaveAsync(cancellationToken);
        
        _logger.LogInformation("Completed multipart upload for media asset with id {MediaAssetId}", mediaAsset.Id);

        return UnitResult.Success<Error>();
    }
}