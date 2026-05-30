namespace FileService.Application.UseCases.GetMediaAssets;

public record GetMediaAssetsRequest(IReadOnlyCollection<Guid> MediaAssetIds);