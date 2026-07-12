namespace FileService.Contracts.Dtos;

public record GetMediaAssetDto(
    Guid Id,
    string Status,
    string AssetType,
    Guid OwnerId,
    string OwnerType,
    string? Url);