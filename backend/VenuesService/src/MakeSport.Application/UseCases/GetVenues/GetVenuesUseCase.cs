using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Application.Validation;
using MakeSport.Contracts.Dtos;
using MakeSport.Contracts.Requests;
using MakeSport.Contracts.Responses;
using MakeSport.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace MakeSport.Application.UseCases.GetVenues;

public class GetVenuesUseCase
{
    private readonly IVenuesReadDbContext _readDbContext;
    private readonly IValidator<GetVenuesRequest> _validator;

    public GetVenuesUseCase(IVenuesReadDbContext readDbContext, IValidator<GetVenuesRequest> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
    }

    public async Task<Result<PaginationVenuesResponse, Error>> Handle(GetVenuesRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }
        
        var query = _readDbContext.VenuesQuery;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(v => v.Title.Contains(request.Search));
        }

        var venuesCount = await query.CountAsync(cancellationToken);

        var venues = await query
            .OrderByDescending(v => v.Title)
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                Address = new AddressDto(v.Address.City, v.Address.Street, v.Address.Building),
                Coordinates = new CoordinatesDto(v.Coordinates.Latitude, v.Coordinates.Longitude)
            })
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        var totalPages = (int)Math.Ceiling((double)venuesCount / request.PageSize);

        return new PaginationVenuesResponse(venues, venuesCount, request.Page, request.PageSize, totalPages);
    }
}