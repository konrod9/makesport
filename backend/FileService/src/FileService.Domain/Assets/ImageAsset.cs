using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace FileService.Domain.Assets;

public class ImageAsset : MediaAsset
{
    public const long MAX_SIZE = 5_368_709;

    public const string ASSET_TYPE = "image";
    public const string LOCATION = "pictures";
    public const string ALLOWED_CONTENT_TYPE = "image";

    public static readonly string[] AllowedExtensions = ["jpg", "jpeg", "png", "webp"];

    private ImageAsset() { }

    private ImageAsset(
        Guid id,
        MediaData mediaData,
        MediaStatus status,
        Guid ownerId,
        string ownerType,
        StorageKey key)
        : base(id, mediaData, AssetType.Image, status, key)
    {
    }

    public static UnitResult<Error> Validate(MediaData mediaData)
    {
        if (!AllowedExtensions.Contains(mediaData.FileName.Extension))
        {
            return Error.Validation("image.invalid.extension", $"File extension must be one of: {string.Join(", ", AllowedExtensions)}");
        }

        if (mediaData.ContentType.Category != MediaType.Image)
        {
            return Error.Validation("image.invalid.content-type", $"File content type must be {ALLOWED_CONTENT_TYPE}");
        }

        if (mediaData.Size > MAX_SIZE)
        {
            return Error.Validation("image.invalid.size", $"File size must be less than {MAX_SIZE} bytes");
        }

        return UnitResult.Success<Error>();
    }

    public static Result<ImageAsset, Error> CreateForUpload(Guid id, MediaData mediaData, Guid ownerId, string ownerType)
    {
        UnitResult<Error> validationResult = Validate(mediaData);
        if (validationResult.IsFailure)
            return validationResult.Error;

        Result<StorageKey, Error> key = StorageKey.Create(LOCATION, null, id.ToString());
        if (key.IsFailure)
            return key.Error;

        var image = new ImageAsset(
            id,
            mediaData,
            MediaStatus.Uploading,
            ownerId,
            ownerType,
            key.Value);

        return image;
    }
}