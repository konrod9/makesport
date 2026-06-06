using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Application;
using FileService.Application.Dtos;
using FileService.Application.FilesStorage;
using FileService.Contracts;
using FileService.Domain;
using FileService.Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public class S3Provider : IDisposable, IFileStorageProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _s3Options;
    private readonly ILogger<S3Provider> _logger;

    private readonly SemaphoreSlim _requestsSemaphore;

    public S3Provider(IAmazonS3 s3Client, IOptions<S3Options> s3Options, ILogger<S3Provider> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _s3Options = s3Options.Value;
        _requestsSemaphore = new SemaphoreSlim(_s3Options.MaxConcurrentRequests);
    }

    public async Task<Result<string, Error>> StartMultipartUploadAsync(
        StorageKey storageKey,
        MediaData mediaData,
        CancellationToken ct)
    {
        try
        {
            var request = new InitiateMultipartUploadRequest
            {
                BucketName = storageKey.Location,
                Key = storageKey.Value,
                ContentType = mediaData.ContentType.Value
            };

            var response = await _s3Client.InitiateMultipartUploadAsync(request, ct);

            return response.UploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to start multipart upload for bucket {BucketName} and key {Key}. Error: {ErrorMessage}",
                storageKey.Location, storageKey.Value, ex.Message);
            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<IReadOnlyList<ChunkUploadUrl>, Error>> GenerateAllChunksUploadUrlsAsync(
        StorageKey storageKey,
        string uploadId,
        int totalChunks,
        CancellationToken ct)
    {
        try
        {
            var tasks = Enumerable.Range(1, totalChunks)
                .Select(async partNumber =>
                {
                    await _requestsSemaphore.WaitAsync(ct);

                    try
                    {
                        var request = new GetPreSignedUrlRequest
                        {
                            BucketName = storageKey.Location,
                            Key = storageKey.Value,
                            Verb = HttpVerb.PUT,
                            UploadId = uploadId,
                            PartNumber = partNumber,
                            Expires = DateTime.UtcNow.AddHours(_s3Options.UploadUrlExpirationHours),
                            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP
                        };

                        var url = await _s3Client.GetPreSignedURLAsync(request);

                        return new ChunkUploadUrl(partNumber, url);
                    }
                    finally
                    {
                        _requestsSemaphore.Release();
                    }
                });

            var results = await Task.WhenAll(tasks);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate chunk upload URLs for bucket {BucketName}. Error: {ErrorMessage}",
                storageKey.Location, ex.Message);

            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> GenerateDownloadUrlAsync(StorageKey storageKey)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = storageKey.Location,
                Key = storageKey.Value,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddDays(_s3Options.DownloadUrlExpirationDays),
                Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP
            };

            var response = await _s3Client.GetPreSignedURLAsync(request);

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to generate download URL. Error: {ErrorMessage}",
                e.Message);
            return S3ErrorMapper.ToError(e);
        }
    }

    public async Task<Result<IReadOnlyList<MediaUrl>, Error>> GenerateDownloadUrlsAsync(
        IEnumerable<StorageKey> storageKeys,
        CancellationToken ct)
    {
        try
        {
            var tasks = storageKeys.Select(async storageKey =>
            {
                await _requestsSemaphore.WaitAsync(ct);

                try
                {
                    var request = new GetPreSignedUrlRequest
                    {
                        BucketName = storageKey.Location,
                        Key = storageKey.Value,
                        Verb = HttpVerb.GET,
                        Expires = DateTime.UtcNow.AddDays(_s3Options.DownloadUrlExpirationDays),
                        Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP
                    };

                    var url = await _s3Client.GetPreSignedURLAsync(request);

                    return new MediaUrl(storageKey, url);
                }
                finally
                {
                    _requestsSemaphore.Release();
                }
            });
            
            return await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate download URLs. Error: {ErrorMessage}",
                ex.Message);

            return S3ErrorMapper.ToError(ex);
        }
    }

    public async Task<Result<string, Error>> CompleteMultipartUploadAsync(
        StorageKey storageKey,
        string uploadId,
        IReadOnlyList<PartETagDto> partETags,
        CancellationToken ct)
    {
        try
        {
            var request = new CompleteMultipartUploadRequest
            {
                BucketName = storageKey.Location,
                Key = storageKey.Value,
                UploadId = uploadId,
                PartETags = partETags.Select(p => new PartETag
                {
                    ETag = p.ETag, PartNumber = p.PartNumber
                }).ToList()
            };

            var response = await _s3Client.CompleteMultipartUploadAsync(request, ct);

            return response.Key;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to complete multipart upload. Error: {ErrorMessage}", ex.Message);

            return S3ErrorMapper.ToError(ex);
        }
    }

    public void Dispose()
    {
        _requestsSemaphore.Release();
        _requestsSemaphore.Dispose();
    }
}