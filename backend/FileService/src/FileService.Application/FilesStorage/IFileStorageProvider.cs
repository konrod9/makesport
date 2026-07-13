using CSharpFunctionalExtensions;
using FileService.Application.Models;
using FileService.Contracts.Dtos;
using FileService.Contracts.Shared;
using FileService.Domain;

namespace FileService.Application.FilesStorage;

public interface IFileStorageProvider
{
    Task<Result<string, Error>> StartMultipartUploadAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken ct);

    Task<Result<IReadOnlyList<ChunkUploadUrl>, Error>> GenerateAllChunksUploadUrlsAsync(
        StorageKey storageKey,
        string uploadId,
        int totalChunks,
        CancellationToken ct,
        bool useExternalEndpoint = false);

    Task<Result<string, Error>> GenerateDownloadUrlAsync(
        StorageKey storageKey,
        bool useExternalEndpoint = false);

    Task<Result<IReadOnlyList<MediaUrl>, Error>> GenerateDownloadUrlsAsync(
        IEnumerable<StorageKey> storageKeys,
        CancellationToken ct = default,
        bool useExternalEndpoint = false);

    Task<Result<string, Error>> CompleteMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        IReadOnlyList<PartETagDto> partETags,
        CancellationToken ct);
}