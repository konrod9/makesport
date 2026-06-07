namespace FileService.Contracts.Dtos;

public record GetMediaAssetInfoDto(
    Guid Id,
    string Status,
    string AssetType,
    string? Url,
    long? Size,
    string? FileName,
    string? ContentType);