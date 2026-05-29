namespace FileService.Domain;

public enum AssetType
{
    Video,
    Image,
    Avatar,
    Preview
}

public static class AssetTypeExtensions
{
    public static AssetType ToAssetType(this string value)
    {
        return value switch
        {
            "video" => AssetType.Video,
            "image" => AssetType.Image,
            "avatar" => AssetType.Avatar,
            "preview" => AssetType.Preview,
            _ => throw new ArgumentException($"Invalid asset type: {value}")
        };
    }
}