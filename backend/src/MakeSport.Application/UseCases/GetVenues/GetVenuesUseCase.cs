using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases.GetVenues;

public class GetVenuesUseCase(IGetVenuesStorage storage) : IGetVenuesUseCase
{
    public async Task<IEnumerable<VenueDto>> Handle(GetVenuesQuery query, CancellationToken cancellationToken)
    {
        return await storage.GetVenues(cancellationToken);
    }
}