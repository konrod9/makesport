using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Dtos;
using Microsoft.EntityFrameworkCore;
using VenuesService.Contracts.Dtos;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.UseCases.GetById;

public class GetByIdUseCase
{
    private readonly IApplicationReadDbContext _dbContext;
    private readonly IFileCommunicationService _fileCommunicationService;

    public GetByIdUseCase(
        IApplicationReadDbContext dbContext,
        IFileCommunicationService fileCommunicationService)
    {
        _dbContext = dbContext;
        _fileCommunicationService = fileCommunicationService;
    }

    public async Task<Result<VenueDto, Error>> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        var venueId = VenueId.Create(id);
        
        var row = await _dbContext.VenuesQuery
            .Where(v => v.Id == VenueId.Create(venueId))
            .Select(v => new
            {
                v.Id,
                v.Title,
                v.Description,
                v.SportType,
                v.Surface,
                v.Rating,
                v.ReviewCount,
                v.IsOpen,
                v.HasLighting,
                v.IsFree,
                WorkingHours = v.WorkingHours.Value,
                Address = new AddressDto(
                    v.Address.City,
                    v.Address.Street,
                    v.Address.Building,
                    v.Address.FullName),
                v.Location,
                v.VideoId,
                v.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
            return GeneralErrors.NotFound(id, "площадка");

        var venue = new VenueDto
        {
            Id = row.Id,
            Title = row.Title,
            Description = row.Description,
            SportType = row.SportType,
            Surface = row.Surface,
            Rating = row.Rating,
            ReviewCount = row.ReviewCount,
            IsOpen = row.IsOpen,
            HasLighting = row.HasLighting,
            IsFree = row.IsFree,
            WorkingHours = row.WorkingHours,
            Address = row.Address,
            Coordinates = new CoordinatesDto(row.Location.Y, row.Location.X),
            Video = new MediaDto { Id = row.VideoId },
            CreatedAt = row.CreatedAt
        };

        var images = await _fileCommunicationService
            .GetByOwners(new GetByOwnersRequest([id], "venue"), cancellationToken);
        if (images.IsFailure)
            return Error.Failure("file-service-error", "Error while getting files from FileService");
        
        venue.Images = images.Value.MediaAssets
            .Select(i => new MediaDto { Id = i.Id, Status = i.Status, Url = i.Url })
            .ToList();

        return venue;
    }
}