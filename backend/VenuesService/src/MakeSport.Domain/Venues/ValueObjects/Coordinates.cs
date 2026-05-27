using CSharpFunctionalExtensions;
using MakeSport.Domain.Shared;

namespace MakeSport.Domain.Venues.ValueObjects;

public record Coordinates
{
    private Coordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public double Latitude { get; }
    
    public double Longitude { get; }
    
    public static Result<Coordinates, Error> Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return GeneralErrors.ValueIsInvalid("latitude", "Latitude must be between -90 and 90 degrees");
        }

        if (longitude < -180 || longitude > 180)
        {
            return GeneralErrors.ValueIsInvalid("longitude", "Longitude must be between -180 and 180 degrees");
        }

        return new Coordinates(latitude, longitude);
    }
}