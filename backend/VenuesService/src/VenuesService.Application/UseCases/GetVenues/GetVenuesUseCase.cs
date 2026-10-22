using CSharpFunctionalExtensions;
using FileService.Contracts;
using FileService.Contracts.Dtos;
using FluentValidation;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Dtos;
using VenuesService.Contracts.Requests;
using VenuesService.Contracts.Responses;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Error = VenuesService.Domain.Shared.Error;

namespace VenuesService.Application.UseCases.GetVenues;

public class GetVenuesUseCase
{
    private readonly IVenuesReadDbContext _readDbContext;
    private readonly IValidator<GetVenuesRequest> _validator;
    private readonly IFileCommunicationService _fileCommunicationService;
    private readonly IVenueSortingService _sortingService;

    public GetVenuesUseCase(IVenuesReadDbContext readDbContext, IValidator<GetVenuesRequest> validator,
        IFileCommunicationService fileCommunicationService, IVenueSortingService sortingService)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _fileCommunicationService = fileCommunicationService;
        _sortingService = sortingService;
    }

    public async Task<Result<PaginationVenuesResponse, Error>> Handle(GetVenuesRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var query = _readDbContext.VenuesQuery;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(v => EF.Functions.Like(v.Title.ToLower(), $"%{request.Search.ToLower()}%"));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(v => v.Address.City == request.City);
        }

        if (!string.IsNullOrWhiteSpace(request.SportType))
        {
            query = query.Where(v => v.SportType == request.SportType);
        }

        if (!string.IsNullOrWhiteSpace(request.Surface))
        {
            query = query.Where(v => v.Surface == request.Surface);
        }

        if (request.OnlyFree.HasValue && request.OnlyFree.Value)
        {
            query = query.Where(v => v.IsFree);
        }

        if (request.OnlyOpen.HasValue && request.OnlyOpen.Value)
        {
            query = query.Where(v => v.IsOpen);
        }

        if (request.HasLighting.HasValue && request.HasLighting.Value)
        {
            query = query.Where(v => v.HasLighting);
        }

        var venuesCount = await query.CountAsync(cancellationToken);

        query = _sortingService.ApplySort(
            query,
            request.SortBy,
            request.SortDirection,
            request.Latitude,
            request.Longitude);

        var hasOrigin = request is { Latitude: not null, Longitude: not null };
        var origin = hasOrigin
            ? new Point(request.Longitude!.Value, request.Latitude!.Value) { SRID = 4326 }
            : new Point(0, 0) { SRID = 4326 };

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
                v.CreatedAt,
                DistanceMeters = hasOrigin
                    ? EF.Functions.Distance(v.Location, origin, false)
                    : (double?)null
            })
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
                CreateAt = v.CreatedAt,
                DistanceMeters = v.DistanceMeters,
            }).ToList();

        var totalPages = (int)Math.Ceiling((double)venuesCount / request.PageSize);

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

        /*IReadOnlyList<Guid> mediaAssetIds = venues.Where(v => v.Video != null).Select(v => v.Video!.Id).ToList();

        Result<GetMediaAssetsResponse, FileService.Contracts.Shared.Error> mediaAssets = await _fileCommunicationService
            .GetMediaAssets(new GetMediaAssetsRequest(mediaAssetIds), cancellationToken);
        if (mediaAssets.IsFailure)
            return Error.Failure("file-service-error", "Error while getting videos from FileService");

        var mediaAssetsDict = mediaAssets.Value.MediaAssets.ToDictionary(x => x.Id, x => x);

        foreach (VenueDto venue in venues)
        {
            if (venue.Video != null && mediaAssetsDict.TryGetValue(venue.Video.Id, out GetMediaAssetDto? mediaAsset))
            {
                venue.Video = new MediaDto
                {
                    Id = mediaAsset.Id, Status = mediaAsset.Status, Url = mediaAsset.Url,
                };
            }
        }*/

        return new PaginationVenuesResponse(venues, venuesCount, request.Page, request.PageSize, totalPages);
    }
}