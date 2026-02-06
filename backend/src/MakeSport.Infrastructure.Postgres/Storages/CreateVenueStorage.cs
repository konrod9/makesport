using MakeSport.Application.DTOs;
using MakeSport.Application.UseCases;
using MakeSport.Domain.Venues;

namespace MakeSport.Infrastructure.Postgres.Storages;

public class CreateVenueStorage(
    VenueDbContext dbContext,
    IGuidFactory guidFactory) : ICreateVenueStorage
{
    public async Task<VenueDto> CreateAsync(string name, string description, CancellationToken cancellationToken)
    {
        var venue = new Venue()
        {
            VenueId = guidFactory.Create(),
            Name = name,
            Description = description
        };
        
        await dbContext.Venues.AddAsync(venue, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new VenueDto(venue.VenueId, venue.Name, venue.Description);
    }
}