using CSharpFunctionalExtensions;
using FileService.Contracts.Dtos;
using FileService.Contracts.Shared;

namespace FileService.Contracts;

public interface IFileCommunicationService
{
    Task<Result<GetMediaAssetsResponse, Error>> GetMediaAssets(GetMediaAssetsRequest request, CancellationToken cancellationToken);
    
    Task<Result<GetByOwnersResponse, Error>> GetByOwners(GetByOwnersRequest request, CancellationToken cancellationToken);
}