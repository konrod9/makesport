using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Auth.JwtValidation.Abstractions;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Requests;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;
using Microsoft.Extensions.Logging;

namespace VenuesService.Application.UseCases.CreateVenue;

public class CreateVenueUseCase
{
    private readonly IValidator<CreateVenueRequest> _validator;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateVenueUseCase> _logger;
    private readonly ICurrentUserService _currentUserService;

    public CreateVenueUseCase(IValidator<CreateVenueRequest> validator, IApplicationDbContext dbContext,
        ILogger<CreateVenueUseCase> logger, ICurrentUserService currentUserService)
    {
        _validator = validator;
        _dbContext = dbContext;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid, Error>> Handle(CreateVenueRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        var venueId = VenueId.NewId();
        var address = Address.Create(request.Address.City, request.Address.Street, request.Address.Building).Value;
        var coordinates = Coordinates.Create(request.Coordinates.Latitude, request.Coordinates.Longitude).Value;
        var workingHours = WorkingHours.Create(request.WorkingHours.WorkingStart, request.WorkingHours.WorkingEnd).Value;
        
        var userId = _currentUserService.UserId;
        
        var venue = Venue.Create(
            venueId, 
            request.Title,
            request.Description, 
            address, 
            coordinates,
            request.IsOpen,
            request.HasLighting, 
            request.IsFree, 
            workingHours, 
            request.Surface, 
            request.SportType, 
            userId);

        var result = await _dbContext.Venues.AddAsync(venue, cancellationToken);
        var saveResult = await _dbContext.TrySaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Created venue {VenueId}", venue.Id);

        return venue.Id.Value;
    }
}