using MakeSport.Domain.Venues.ValueObjects;

namespace MakeSport.Domain.Venues;

public class Venue
{
    // EF Core requires a parameterless constructor for materialization
    private Venue()
    {
        
    }
    
    private Venue(VenueId id, string title, string? description, Address address, Coordinates coordinates)
    {
        Id = id;
        Title = title;
        Description = description;
        Address = address;
        Coordinates = coordinates;
    }
    
    public VenueId Id { get; private set; }
    
    public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    public Address Address { get; private set; }
    
    public Coordinates Coordinates { get; private set; }
    
    public Guid VideoId { get; private set; }
    
    public static Venue Create(VenueId id, string title, string? description, Address address, Coordinates location)
    {
        return new Venue(id, title, description, address, location);
    }
}