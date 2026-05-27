using CSharpFunctionalExtensions;
using MakeSport.Domain.Shared;
using MakeSport.Domain.Venues;

namespace MakeSport.Application.UseCases;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken ct = default);
}