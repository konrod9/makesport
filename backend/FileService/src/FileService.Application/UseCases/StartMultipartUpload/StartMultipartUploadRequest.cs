namespace FileService.Application.UseCases.StartMultipartUpload;

public record StartMultipartUploadRequest(
    string FileName,
    string AssetType,
    string ContentType,
    long Size,
    Guid OwnerId,
    string OwnerType);