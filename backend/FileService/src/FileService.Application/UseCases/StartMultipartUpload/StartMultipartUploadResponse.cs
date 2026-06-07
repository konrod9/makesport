using FileService.Contracts;
using FileService.Contracts.Dtos;

namespace FileService.Application.UseCases.StartMultipartUpload;

public record StartMultipartUploadResponse(
    Guid MediaAssetId,
    string UploadId,
    IReadOnlyList<ChunkUploadUrl> ChunkUploadUrls,
    int ChunkSize);