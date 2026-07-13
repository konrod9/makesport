using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;

namespace FileService.Domain.Assets;

public abstract class MediaAsset
{
    public Guid Id { get; protected set; }

    public MediaData MediaData { get; protected set; }

    public AssetType AssetType { get; protected set; }
    
    public Guid OwnerId { get; protected set; }

    public string OwnerType { get; protected set; } = string.Empty;

    public StorageKey Key { get; protected set; }

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    public MediaStatus Status { get; protected set; }

    // For EF Core
    protected MediaAsset()
    {
    }

    protected MediaAsset(
        Guid id,
        MediaData mediaData,
        AssetType assetType,
        MediaStatus status,
        Guid ownerId,
        string ownerType,
        StorageKey key)
    {
        Id = id;
        MediaData = mediaData;
        AssetType = assetType;
        Status = status;
        OwnerId = ownerId;
        OwnerType = ownerType.Trim().ToLowerInvariant();
        Key = key;
    }

    public static Result<MediaAsset, Error> CreateForUpload(MediaData mediaData, AssetType assetType, Guid ownerId,
        string ownerType)
    {
        var assetId = Guid.NewGuid();

        switch (assetType)
        {
            case AssetType.Video:
                var videoResult = VideoAsset.CreateForUpload(assetId, mediaData, ownerId, ownerType);
                return videoResult.IsFailure ? videoResult.Error : videoResult.Value;
            case AssetType.Image:
                var imageResult = ImageAsset.CreateForUpload(assetId, mediaData, ownerId, ownerType);
                return imageResult.IsFailure ? imageResult.Error : imageResult.Value;
            case AssetType.Avatar:
            case AssetType.Preview:
            default:
                throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
        }
    }

    public UnitResult<Error> MarkUploaded()
    {
        if (Status != MediaStatus.Uploading)
            return UnitResult.Success<Error>();

        Status = MediaStatus.Uploaded;
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> MarkFailed()
    {
        Status = MediaStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<Error>();
    }
}