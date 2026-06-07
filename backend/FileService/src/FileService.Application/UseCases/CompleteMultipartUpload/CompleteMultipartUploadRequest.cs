using FileService.Contracts;
using FileService.Contracts.Dtos;

namespace FileService.Application.UseCases.CompleteMultipartUpload;

public record CompleteMultipartUploadRequest(
    Guid MediaAssetId, 
    string UploadId, 
    IReadOnlyList<PartETagDto> PartETags);