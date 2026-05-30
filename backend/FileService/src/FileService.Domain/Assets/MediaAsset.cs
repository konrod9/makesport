using CSharpFunctionalExtensions;
using FileService.Domain.Shared;

namespace FileService.Domain.Assets;

public abstract class MediaAsset
{
    public Guid Id { get; protected set; }

    public MediaData MediaData { get; protected set; }

    public AssetType AssetType { get; protected set; }

    public StorageKey Key { get; protected set; }

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    public MediaOwner Owner { get; protected set; }

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
        StorageKey key)
    {
        Id = id;
        MediaData = mediaData;
        AssetType = assetType;
        Status = status;
        Key = key;
    }

    public static Result<MediaAsset, Error> CreateForUpload(MediaData mediaData, AssetType assetType)
    {
        var assetId = Guid.NewGuid();
        
        switch (assetType)
        {
            case AssetType.Video:
                var videoResult = VideoAsset.CreateForUpload(assetId, mediaData);
                return videoResult.IsFailure ? videoResult.Error : videoResult.Value;
            case AssetType.Image:
                // TODO: Сделать для изображений
                break;
            case AssetType.Avatar:
            case AssetType.Preview:
            default:
                throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
        }
    }
}