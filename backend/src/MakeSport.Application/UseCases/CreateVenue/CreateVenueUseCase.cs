using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Application.Validation;
using MakeSport.Contracts.Dtos;
using MakeSport.Contracts.Requests;
using MakeSport.Domain.Shared;
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

    public async Task<Result<Guid, Error>> Handle(CreateVenueRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var venueId = VenueId.NewId();
        var address = Address.Create(request.Address.City, request.Address.Street, request.Address.Building).Value;
        var coordinates = Coordinates.Create(request.Coordinates.Latitude, request.Coordinates.Longitude).Value;
        
        var venue = Venue.Create(venueId, request.Title, request.Description, address, coordinates);
        
        // TODO: Добавить сохранение в репозиторий
        
        // TODO: Добавить логирование
        
        return venue.Id.Value;
    }
}