using FileService.Domain.Assets;

namespace FileService.Application;

public interface IReadDbContext
{
    IQueryable<MediaAsset> MediaAssetsQuery { get; }
}