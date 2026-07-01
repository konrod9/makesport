using CSharpFunctionalExtensions;
using VenuesService.Contracts.Dtos;
using VenuesService.Domain.Shared;

namespace VenuesService.Domain.Venues.ValueObjects;

public record Address
{
    private Address(string city, string street, string? building)
    {
        City = city;
        Street = street;
        Building = building;
    }

    public string City { get; }

    public string Street { get; }

    public string? Building { get; }

    public string FullName =>
        $"{City}, {Street}{(string.IsNullOrWhiteSpace(Building) ? string.Empty : ", " + Building)}";

    public static Result<Address, Error> Create(string city, string street, string? building)
    {
        if (string.IsNullOrWhiteSpace(street))
            return GeneralErrors.ValueIsRequired(nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            return GeneralErrors.ValueIsRequired(nameof(city));

        return new Address(city, street, building);
    }
}