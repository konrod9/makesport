namespace FileService.Application;

public interface IS3Provider
{
    Task<string> GenerateDownloadUrlAsync(string bucketName, string key);
}