using CSharpFunctionalExtensions;
using FileService.Domain.Assets;
using FileService.Domain.Shared;

namespace FileService.Application;

public interface IMediaAssetsRepository
{
    Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken);
}