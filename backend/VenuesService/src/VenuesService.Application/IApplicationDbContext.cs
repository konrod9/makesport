using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues;

namespace VenuesService.Application;

public interface IApplicationDbContext
{
    DbSet<Venue> Venues { get; }
    DbSet<Review> Reviews { get; }
    
    Task<UnitResult<Error>> TrySaveChangesAsync(CancellationToken ct = default);

    void ClearChangeTracker();
}