using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Application.Validation;
using MakeSport.Contracts.Requests;
using MakeSport.Domain.Shared;
using MakeSport.Domain.Venues;
using MakeSport.Domain.Venues.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueUseCase
{
    private readonly IValidator<CreateVenueRequest> _validator;
    private readonly IVenuesRepository _repository;
    private readonly ILogger<CreateVenueUseCase> _logger;

    public CreateVenueUseCase(IValidator<CreateVenueRequest> validator, IVenuesRepository repository, ILogger<CreateVenueUseCase> logger)
    {
        _validator = validator;
        _repository = repository;
        _logger = logger;
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
        
        var result = await _repository.AddAsync(venue, cancellationToken);
        if (result.IsFailure)
            return result.Error;
        
        _logger.LogInformation("Created venue {VenueId}", venue.Id);
        
        return venue.Id.Value;
    }
}