using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases.GetVenues;

public interface IGetVenuesStorage
{
    Task<IEnumerable<VenueDto>> GetVenues(CancellationToken cancellationToken);
}