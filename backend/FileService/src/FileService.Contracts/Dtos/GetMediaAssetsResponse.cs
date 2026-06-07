namespace FileService.Contracts.Dtos;

public record GetMediaAssetsResponse(IReadOnlyList<GetMediaAssetDto> MediaAssets);