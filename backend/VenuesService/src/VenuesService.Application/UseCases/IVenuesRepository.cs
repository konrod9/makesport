using CSharpFunctionalExtensions;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues;

namespace VenuesService.Application.UseCases;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken ct = default);
}