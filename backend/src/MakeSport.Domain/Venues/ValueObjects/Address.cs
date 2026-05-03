using CSharpFunctionalExtensions;
using MakeSport.Domain.Shared;

namespace MakeSport.Domain.Venues.ValueObjects;

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
    
    public static Result<Address, Error> Create(string city, string street, string? building)
    {
        if (string.IsNullOrWhiteSpace(street))
            return Errors.General.ValueIsRequired(nameof(street));
        
        if (string.IsNullOrWhiteSpace(city))
            return Errors.General.ValueIsRequired(nameof(city));

        return new Address(city, street, building);
    }
}