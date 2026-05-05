using CSharpFunctionalExtensions;
using MakeSport.Application.UseCases;
using MakeSport.Domain.Shared;
using MakeSport.Domain.Venues;
using MakeSport.Infrastructure.Postgres.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MakeSport.Infrastructure.Postgres;

public class VenuesRepository : IVenuesRepository
{
    private readonly VenueDbContext _dbContext;
    private readonly ILogger<VenuesRepository> _logger;

    public VenuesRepository(VenueDbContext dbContext, ILogger<VenuesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken ct = default)
    {
        _dbContext.Add(venue);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return venue.Id.Value;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            if (pgEx is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null }
                && pgEx.ConstraintName.Contains("venue", StringComparison.InvariantCultureIgnoreCase))
            {
                return VenueErrors.TitleConflict(venue.Title);
            }

            _logger.LogError(ex, "Database update error while creating venue with title {Title}", venue.Title);
            return VenueErrors.DatabaseError();
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Operation was cancelled while creating venue with title {Title}", venue.Title);
            return VenueErrors.OperationCancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating venue with title {Title}", venue.Title);
            return VenueErrors.DatabaseError();
        }
    }
}