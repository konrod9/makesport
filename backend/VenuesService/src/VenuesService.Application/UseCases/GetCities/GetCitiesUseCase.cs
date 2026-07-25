using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using VenuesService.Contracts.Dtos;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.UseCases.GetCities;

public class GetCitiesUseCase
{
    private readonly IVenuesReadDbContext _readDbContext;

    public GetCitiesUseCase(IVenuesReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<CityDto>, Error>> Handle(CancellationToken cancellationToken)
    {
        var cities = await _readDbContext.VenuesQuery
            .GroupBy(v => v.Address.City)
            .Select(v => new CityDto(
                v.First().Address.City, 
                v.First().Coordinates.Latitude, 
                v.First().Coordinates.Longitude))
            .ToListAsync(cancellationToken: cancellationToken);

        return cities;
    }
}