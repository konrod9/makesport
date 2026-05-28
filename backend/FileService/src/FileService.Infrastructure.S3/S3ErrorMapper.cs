using Amazon.S3;
using FileService.Domain.Shared;

namespace FileService.Infrastructure.S3;

public static class S3ErrorMapper
{
    public static Error ToError(Exception ex) => ex switch
    {
        AmazonS3Exception { ErrorCode: "NoSuchBucket" }
            => FileServiceErrors.BucketNotFound(),

        AmazonS3Exception { ErrorCode: "AccessDenied" or "SignatureDoesNotMatch" or "InvalidAccessKeyId" }
            => FileServiceErrors.Forbidden(),

        AmazonS3Exception { ErrorCode: "InvalidRequest" or "InvalidArgument" }
            => FileServiceErrors.ValidationFailed(),

        AmazonS3Exception { ErrorCode: "InternalError" }
            => FileServiceErrors.InternalServerError(),

        AmazonS3Exception { ErrorCode: "NoSuchKey" }
            => FileServiceErrors.ObjectNotFound(),

        AmazonS3Exception { ErrorCode: "NoSuchUpload" }
            => FileServiceErrors.UploadNotFound(),

        ArgumentException => FileServiceErrors.ValidationFailed(),

        HttpRequestException => FileServiceErrors.NetworkIssue(),

        OperationCanceledException => FileServiceErrors.OperationCanceled(),

        _ => FileServiceErrors.Unknown()
    };
}