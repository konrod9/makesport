namespace FileService.Contracts.Shared;

public static class FileServiceErrors
{
    public static Error BucketNotFound()
    {
        return Error.NotFound("no.such.bucket", "Bucket not found.");
    }

    public static Error UploadNotFound()
    {
        return Error.NotFound("upload.not.found", "Upload not found.");
    }
    
    public static Error ObjectNotFound(string? objectKey = null)
    {
        var key = objectKey is null ? string.Empty : $"with key {objectKey}";
        return Error.NotFound("object.not.found", $"Object '{key}' not found.");
    }

    public static Error Forbidden()
    {
        return Error.Failure("access.denied", "Access denied.");
    }

    public static Error ValidationFailed()
    {
        var message = "Request contains invalid data.";

        return Error.Validation("validation.failed", message);
    }

    public static Error InternalServerError()
    {
        return Error.Failure("internal.server.error", "Internal storage error.");
    }

    public static Error OperationCanceled()
    {
        return Error.Failure("operation.canceled", "Operation was canceled.");
    }

    public static Error NetworkIssue()
    {
        return Error.Failure(
            "network.issue",
            "Network error while interacting with the file storage.");
    }

    public static Error Unknown()
    {
        return Error.Failure("unknown.error", "An unknown error occurred.");
    }
    
    public static Error DatabaseError() =>
        Error.Failure("file-service.database.error", "Error while accessing the database in file service");
    
    public static Error OperationCancelled() =>
        Error.Failure("file-service.cancelled", "Operation was cancelled");
}