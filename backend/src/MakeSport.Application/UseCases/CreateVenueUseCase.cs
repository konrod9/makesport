using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases;

public class CreateVenueUseCase(ICreateVenueStorage storage) : ICreateVenueUseCase
{
    public async Task<VenueDto> Handle(CreateVenueCommand command, CancellationToken cancellationToken)
    {
        return await storage.CreateAsync(command.Name, command.Description, cancellationToken);
    }
}