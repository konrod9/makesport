using CSharpFunctionalExtensions;
using FileService.Application;
using FileService.Domain.Assets;
using FileService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FileService.Infrastructure.Postgres.Repositories;

public class MediaAssetsRepository : IMediaAssetsRepository
{
    private readonly FileServiceDbContext _context;
    private readonly ILogger<MediaAssetsRepository> _logger;

    public MediaAssetsRepository(FileServiceDbContext context, ILogger<MediaAssetsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken)
    {
        await _context.AddAsync(mediaAsset, cancellationToken);
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return mediaAsset.Id;
        }
        catch (DbUpdateException ex) when(ex.InnerException is PostgresException)
        {
            _logger.LogError(ex, "Database update error while adding media asset with id {MediaAssetId}", mediaAsset.Id);
            return FileServiceErrors.DatabaseError();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while adding media asset with id {MediaAssetId}", mediaAsset.Id);
            return FileServiceErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding media asset with id {MediaAssetId}", mediaAsset.Id);
            return FileServiceErrors.DatabaseError();
        }
    }
}