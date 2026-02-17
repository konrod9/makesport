namespace MakeSport.Domain.Venues;

public class GeoCoordinate
{
    public GeoCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    
    public double Latitude { get; }
    
    public double Longitude { get; }
    
    public static GeoCoordinate Create(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");
        }

        return new GeoCoordinate(latitude, longitude);
    }
}