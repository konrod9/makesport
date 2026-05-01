using FluentValidation;
using MakeSport.Application.DTOs;
using MakeSport.Domain.Venues;
using MakeSport.Domain.Venues.ValueObjects;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueUseCase(
    ICreateVenueStorage storage,
    IValidator<CreateVenueCommand> validator) : ICreateVenueUseCase
{
    public async Task<VenueDto> Handle(CreateVenueCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var venueId = VenueId.NewId();

        var venue = Venue.Create(venueId, command.Title, command.Description, command);
    }
}