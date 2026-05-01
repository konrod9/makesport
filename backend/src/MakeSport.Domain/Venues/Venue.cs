using MakeSport.Domain.Venues.ValueObjects;

namespace MakeSport.Domain.Venues;

public class Venue
{
    // EF Core requires a parameterless constructor for materialization
    private Venue()
    {
        
    }
    
    private Venue(VenueId id, string title, string description, Address address, GeoCoordinates location)
    {
        Id = id;
        Title = title;
        Description = description;
        Address = address;
        Location = location;
    }
    
    public VenueId Id { get; private set; }
    
    public string Title { get; private set; }
    
    public string Description { get; private set; }
    
    public Address Address { get; private set; }
    
    public GeoCoordinates Location { get; private set; }
    
    public static Venue Create(VenueId id, string title, string description, Address address, GeoCoordinates location)
    {
        return new Venue(id, title, description, address, location);
    }
}