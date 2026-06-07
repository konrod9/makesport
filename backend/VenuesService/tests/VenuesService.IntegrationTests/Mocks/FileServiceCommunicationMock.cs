using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Dtos;
using FileService.Contracts.Shared;

namespace VenuesService.IntegrationTests.Mocks;

public class FileServiceCommunicationMock : IFileCommunicationService
{

    public Task<Result<GetMediaAssetsResponse, Error>> GetMediaAssets(GetMediaAssetsRequest request, CancellationToken cancellationToken)
    {
        var result = new GetMediaAssetsResponse([
            new GetMediaAssetDto(Guid.NewGuid(), "ready", "video", "url"),
            new GetMediaAssetDto(Guid.NewGuid(), "ready", "video", "url"), 
            new GetMediaAssetDto(Guid.NewGuid(), "ready", "video", "url")
        ]);

        return Task.FromResult(Result.Success<GetMediaAssetsResponse, Error>(result));
    }
}