using FileService.Contracts;

namespace FileService.Application.UseCases.StartMultipartUpload;

public record StartMultipartUploadResponse(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<ChunkUploadUrl> ChunkUploadUrls,
    long ChunkSize);