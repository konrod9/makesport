using MakeSport.Application.DTOs;
using MakeSport.Application.UseCases.GetVenues;
using Microsoft.EntityFrameworkCore;

namespace MakeSport.Infrastructure.Postgres.Storages;

public class GetVenuesStorage(VenueDbContext dbContext) : IGetVenuesStorage
{
    public async Task<IEnumerable<VenueDto>> GetVenues(CancellationToken cancellationToken)
    {
        return await dbContext.Venues.Select(venue => new VenueDto(
                venue.Id,
                venue.Title,
                venue.Description,
                venue.Location.Latitude,
                venue.Location.Longitude))
            .ToArrayAsync(cancellationToken);
    }
}