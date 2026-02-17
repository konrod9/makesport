using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases.CreateVenue;

public interface ICreateVenueStorage
{
    Task<VenueDto> CreateAsync(string name, string description, double lat, double lon, CancellationToken cancellationToken);
}