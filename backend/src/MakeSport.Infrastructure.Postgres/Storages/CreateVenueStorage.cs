using MakeSport.Application.DTOs;
using MakeSport.Application.UseCases;
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Domain.Venues;
using MakeSport.Domain.Venues.ValueObjects;

namespace MakeSport.Infrastructure.Postgres.Storages;

public class CreateVenueStorage(
    VenueDbContext dbContext,
    IGuidFactory guidFactory) : ICreateVenueStorage
{
    public async Task<VenueDto> CreateAsync(string name, string description, double lat, double lon, CancellationToken cancellationToken)
    {
        var venue = new Venue()
        {
            Id = guidFactory.Create(),
            Title = name,
            Description = description,
            Location = GeoCoordinates.Create(lat, lon)
        };
        
        await dbContext.Venues.AddAsync(venue, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new VenueDto(venue.Id, venue.Title, venue.Description, venue.Location.Latitude, venue.Location.Longitude);
    }
}