namespace FileService.Contracts.Dtos;

public record GetByOwnersResponse(IReadOnlyList<GetMediaAssetDto> MediaAssets);