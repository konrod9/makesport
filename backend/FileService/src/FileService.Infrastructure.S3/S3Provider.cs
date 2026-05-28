using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using FileService.Application;
using FileService.Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileService.Infrastructure.S3;

public class S3Provider : IS3Provider
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
        string bucketName,
        string key,
        string contentType,
        CancellationToken ct)
    {
        try
        {
            var request = new InitiateMultipartUploadRequest
            {
                BucketName = bucketName,
                Key = key,
                ContentType = contentType
            };

            var response = await _s3Client.InitiateMultipartUploadAsync(request, ct);

            return response.UploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to start multipart upload for bucket {BucketName} and key {Key}. Error: {ErrorMessage}",
                bucketName, key, ex.Message);
            return S3ErrorMapper.ToError(ex);
        }
    }
    
    public async Task<Result<IReadOnlyList<string>, Error>> GenerateAllChunksUploadUrlsAsync(
        string bucketName,
        string key,
        string uploadId,
        int totalChunks,
        CancellationToken ct)
    {
        var tasks = Enumerable.Range(0, totalChunks)
            .Select(async partNumber =>
            {
                await _requestsSemaphore.WaitAsync(ct);

                try
                {
                    var request = new GetPreSignedUrlRequest
                    {
                        BucketName = bucketName,
                        Key = key,
                        Verb = HttpVerb.PUT,
                        UploadId = uploadId,
                        PartNumber = partNumber,
                        Expires = DateTime.UtcNow.AddHours(_s3Options.UploadUrlExpirationHours),
                        Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP
                    };

                    var url = await _s3Client.GetPreSignedURLAsync(request);

                    return url;
                }
                finally
                {
                    _requestsSemaphore.Release();
                }
            });
        
        var results = await Task.WhenAll(tasks);

        return results;
    }

    public async Task<string> GenerateDownloadUrlAsync(string bucketName, string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddHours(_s3Options.DownloadUrlExpirationHours),
            Protocol = _s3Options.WithSsl ? Protocol.HTTPS : Protocol.HTTP
        };

        var response = await _s3Client.GetPreSignedURLAsync(request);

        return response;
    }
}