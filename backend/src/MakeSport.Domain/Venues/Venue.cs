namespace MakeSport.Domain.Venues;

public class Venue
{
    public Guid VenueId { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public GeoCoordinate Location { get; set; }
}