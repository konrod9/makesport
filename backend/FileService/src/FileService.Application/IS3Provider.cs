namespace FileService.Application;

public interface IS3Provider
{
    Task UploadFileAsync(Stream stream, string bucketName, string key, string contentType, CancellationToken ct);

    Task<string> GenerateDownloadUrlAsync(string bucketName, string key);
}