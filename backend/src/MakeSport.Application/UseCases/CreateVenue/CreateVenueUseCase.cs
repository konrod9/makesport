using FluentValidation;
using MakeSport.Application.DTOs;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueUseCase(
    ICreateVenueStorage storage,
    IValidator<CreateVenueCommand> validator) : ICreateVenueUseCase
{
    public async Task<VenueDto> Handle(CreateVenueCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        
        return await storage.CreateAsync(command.Name, command.Description, cancellationToken);
    }
}