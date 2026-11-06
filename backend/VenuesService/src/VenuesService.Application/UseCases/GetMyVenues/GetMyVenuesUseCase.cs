using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Dtos;
using MakeSport.Auth.JwtValidation.Abstractions;
using Microsoft.EntityFrameworkCore;
using VenuesService.Contracts.Dtos;
using VenuesService.Contracts.Responses;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.UseCases.GetMyVenues;

public class GetMyVenuesUseCase
{
    private readonly IVenuesReadDbContext _dbContext;
    private ICurrentUserService _currentUserService;
    private readonly IFileCommunicationService _fileCommunicationService;

    public GetMyVenuesUseCase(IVenuesReadDbContext dbContext, ICurrentUserService currentUserService,
        IFileCommunicationService fileCommunicationService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileCommunicationService = fileCommunicationService;
    }

    public async Task<Result<VenuesResponse, Error>> Handle(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty || userId == null)
            return GeneralErrors.NotFound();

        var query = _dbContext.VenuesQuery;

        if (userId != Guid.Empty)
        {
            query = query.Where(v => v.CreatedByUserId == userId);
        }

        var venuesCount = await query.CountAsync(cancellationToken);

        var rows = await query
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
            .ToListAsync(cancellationToken);

        List<VenueDto> venues = rows
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                SportType = v.SportType,
                Surface = v.Surface,
                Rating = v.Rating,
                ReviewCount = v.ReviewCount,
                IsOpen = v.IsOpen,
                HasLighting = v.HasLighting,
                IsFree = v.IsFree,
                WorkingHours = v.WorkingHours,
                Address = v.Address,
                Coordinates = new CoordinatesDto(v.Location.Y, v.Location.X),
                Video = new MediaDto()
                {
                    Id = v.VideoId
                },
                CreatedAt = v.CreatedAt,
            }).ToList();

        var venueIds = venues.Select(v => v.Id).ToList();

        Result<GetByOwnersResponse, FileService.Contracts.Shared.Error> mediaAssets =
            await _fileCommunicationService.GetByOwners(
                new GetByOwnersRequest(venueIds, "venue"),
                cancellationToken);
        if (mediaAssets.IsFailure)
            return Error.Failure("file-service-error", "Error while getting files from FileService");

        var imagesByVenue = mediaAssets.Value.MediaAssets
            .GroupBy(m => m.OwnerId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var venue in venues)
        {
            if (imagesByVenue.TryGetValue(venue.Id, out var images))
            {
                venue.Images = images.Select(i => new MediaDto()
                {
                    Id = i.Id,
                    Status = i.Status,
                    Url = i.Url
                }).ToList();
            }
        }

        return new VenuesResponse(venues, venuesCount);
    }
}