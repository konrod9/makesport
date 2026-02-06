using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases;

public interface ICreateVenueUseCase
{
    Task<VenueDto> Handle(CreateVenueCommand command, CancellationToken cancellationToken);
}