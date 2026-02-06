using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases.GetVenues;

public interface IGetVenuesUseCase
{
    Task<IEnumerable<VenueDto>> Handle(GetVenuesQuery query, CancellationToken cancellationToken);
}