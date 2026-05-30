using FileService.Domain.Assets;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure.Postgres;

public class FileServiceDbContext : DbContext
{
    private DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
}