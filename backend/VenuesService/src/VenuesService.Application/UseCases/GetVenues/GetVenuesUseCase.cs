using CSharpFunctionalExtensions;
using FileService.Contracts;
using FluentValidation;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Dtos;
using VenuesService.Contracts.Requests;
using VenuesService.Contracts.Responses;
using Microsoft.EntityFrameworkCore;
using Error = VenuesService.Domain.Shared.Error;

namespace VenuesService.Application.UseCases.GetVenues;

public class GetVenuesUseCase
{
    private readonly IVenuesReadDbContext _readDbContext;
    private readonly IValidator<GetVenuesRequest> _validator;
    private readonly IFileCommunicationService _fileCommunicationService;

    public GetVenuesUseCase(IVenuesReadDbContext readDbContext, IValidator<GetVenuesRequest> validator,
        IFileCommunicationService fileCommunicationService)
    {
        _readDbContext = readDbContext;
        _validator = validator;
        _fileCommunicationService = fileCommunicationService;
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

        List<VenueDto> venues = await query
            .OrderByDescending(v => v.Title)
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
                WorkingHours = v.WorkingHours.Value,
                Address = new AddressDto(v.Address.City, v.Address.Street, v.Address.Building, v.Address.FullName),
                Coordinates = new CoordinatesDto(v.Coordinates.Latitude, v.Coordinates.Longitude),
                Video = new MediaDto()
                {
                    Id = v.VideoId
                }
            })
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)venuesCount / request.PageSize);
        
        return new PaginationVenuesResponse(venues, venuesCount, request.Page, request.PageSize, totalPages);

        /*IReadOnlyList<Guid> mediaAssetIds = venues.Where(v => v.Video != null).Select(v => v.Video!.Id).ToList();

        var mediaAssets = await _fileCommunicationService
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
        }

        return new PaginationVenuesResponse(venues, venuesCount, request.Page, request.PageSize, totalPages);*/
    }
}