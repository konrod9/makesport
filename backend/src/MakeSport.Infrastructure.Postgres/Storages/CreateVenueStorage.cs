using MakeSport.Application.DTOs;
using MakeSport.Application.UseCases;
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Domain.Venues;

namespace MakeSport.Infrastructure.Postgres.Storages;

public class CreateVenueStorage(
    VenueDbContext dbContext,
    IGuidFactory guidFactory) : ICreateVenueStorage
{
    public async Task<VenueDto> CreateAsync(string name, string description, double lat, double lon, CancellationToken cancellationToken)
    {
        var venue = new Venue()
        {
            VenueId = guidFactory.Create(),
            Name = name,
            Description = description,
            Location = GeoCoordinate.Create(lat, lon)
        };
        
        await dbContext.Venues.AddAsync(venue, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new VenueDto(venue.VenueId, venue.Name, venue.Description, venue.Location.Latitude, venue.Location.Longitude);
    }
}