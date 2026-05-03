using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Contracts.Dtos;
using MakeSport.Contracts.Requests;
using MakeSport.Domain.Venues;
using MakeSport.Domain.Venues.ValueObjects;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueUseCase
{
    private readonly IValidator<CreateVenueRequest> _validator;

    public CreateVenueUseCase(IValidator<CreateVenueRequest> validator)
    {
        _validator = validator;
    }

    public async Task<Result<VenueDto, string>> Handle(CreateVenueRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            //return validationResult.ToError();
        }

        var venueId = VenueId.NewId();

        // TODO: создавать Address и другие value objects 
        
        var venue = Venue.Create(venueId, command.Title, command.Description, command);
    }
}