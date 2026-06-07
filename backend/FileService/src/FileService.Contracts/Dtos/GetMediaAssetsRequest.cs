namespace FileService.Contracts.Dtos;

public record GetMediaAssetsRequest(IReadOnlyCollection<Guid> MediaAssetIds);