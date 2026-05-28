using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Domain.Shared;

namespace FileService.Application;

public interface IS3Provider
{
    Task<Result<string, Error>> StartMultipartUploadAsync(
        string bucketName,
        string key,
        string contentType,
        CancellationToken ct);

    Task<Result<IReadOnlyList<string>, Error>> GenerateAllChunksUploadUrlsAsync(
        string bucketName,
        string key,
        string uploadId,
        int totalChunks,
        CancellationToken ct);

    Task<Result<string, Error>> GenerateDownloadUrlAsync(string bucketName, string key);

    Task<Result<string, Error>> CompleteMultipartUploadAsync(
        string bucketName,
        string key,
        string uploadId,
        IReadOnlyList<PartETagDto> partETags,
        CancellationToken ct);
}