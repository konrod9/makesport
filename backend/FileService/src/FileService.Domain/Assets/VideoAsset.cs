using CSharpFunctionalExtensions;
using FileService.Domain.Shared;

namespace FileService.Domain.Assets;

public class VideoAsset : MediaAsset
{
    public const long MaxSize = 5_358_709_120;
    public const string Location = "videos";
    public const string RawPrefix = "raw";
    public const string AllowedContentType = "video";

    public static readonly string[] AllowedExtensions = ["mp4", "avi", "mov", "mkv"];

    private VideoAsset(
        Guid id,
        MediaData mediaData,
        MediaStatus status,
        StorageKey key) : base(id, mediaData, AssetType.Video, status, key)
    {
    }

    public static UnitResult<Error> Validate(MediaData mediaData)
    {
        if (!AllowedExtensions.Contains(mediaData.FileName.Extension))
            return Error.Validation("video.invalid.extensions",
                $"File extension must be one of: {string.Join(", ", AllowedExtensions)}");

        if (mediaData.ContentType.Category != MediaType.Video)
            return Error.Validation("video.invalid.content-type",
                $"File content type must be of type {AllowedContentType}");

        if (mediaData.Size > MaxSize)
            return Error.Validation("video.invalid.size", $"File size must be less than {MaxSize} bytes");

        return UnitResult.Success<Error>();
    }

    public static Result<VideoAsset, Error> CreateForUpload(Guid id, MediaData mediaData)
    {
        var validationResult = Validate(mediaData);
        if (validationResult.IsFailure)
            return Result.Failure<VideoAsset, Error>(validationResult.Error);

        var key = StorageKey.Create(Location, null, id.ToString());
        if (key.IsFailure)
            return key.Error;
        
        return new VideoAsset(
            id,
            mediaData,
            MediaStatus.Uploading,
            key.Value
        );
    }
}