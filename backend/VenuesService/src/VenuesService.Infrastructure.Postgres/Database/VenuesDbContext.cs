using CSharpFunctionalExtensions;
using VenuesService.Application;
using VenuesService.Domain.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Shared;
using VenuesService.Infrastructure.Postgres.Persistence;

namespace VenuesService.Infrastructure.Postgres.Database;

public class VenuesDbContext(DbContextOptions<VenuesDbContext> options, ILogger<VenuesDbContext> logger)
    : DbContext(options), IApplicationReadDbContext, IApplicationDbContext
{
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Review> Reviews { get; }

    public IQueryable<Venue> VenuesQuery => Venues.AsNoTracking().AsQueryable();
    public IQueryable<Review> ReviewsQuery => Reviews.AsNoTracking().AsQueryable();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VenuesDbContext).Assembly);
    }
    
    public async Task<UnitResult<Error>> TrySaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await base.SaveChangesAsync(ct);
            return UnitResult.Success<Error>();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (DbUpdateException ex) when (ex.GetBaseException() is PostgresException pg)
        {
            logger.LogError(pg, ex.GetBaseException().Message);
            return PersistenceExceptionTranslator.Translate(pg);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("The operation was canceled.");
            return GeneralErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while saving changes");
            return GeneralErrors.DatabaseError();
        }
    }

    public void ClearChangeTracker() => ChangeTracker.Clear();
}