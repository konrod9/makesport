using System.Linq.Expressions;
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
    private readonly FileServiceDbContext _dbContext;
    private readonly ILogger<MediaAssetsRepository> _logger;

    public MediaAssetsRepository(FileServiceDbContext dbContext, ILogger<MediaAssetsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Error>> AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(mediaAsset, cancellationToken);
        
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
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

    public async Task<Result<MediaAsset, Error>> GetBy(Expression<Func<MediaAsset, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var mediaAsset = await _dbContext.MediaAssets.FirstOrDefaultAsync(predicate, cancellationToken);
        if (mediaAsset is null)
            return GeneralErrors.NotFound(null, "Media asset not found.");

        return mediaAsset;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}