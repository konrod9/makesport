using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Domain.Venues;

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
    
    public bool IsOpen { get; private set; }
    public bool HasLighting { get; private set; }
    public bool IsFree { get; private set; }

    public string? WorkingHours { get; private set; }
    
    public double Rating { get; private set; }
    public int ReviewCount { get; private set; }
    
    public string SportType { get; private set; }
    
    public string Surface { get; private set; }

    public Address Address { get; private set; }

    public Coordinates Coordinates { get; private set; }

    public Guid VideoId { get; private set; }

    public static Venue Create(VenueId id, string title, string? description, Address address, Coordinates location)
    {
        return new Venue(id, title, description, address, location);
    }
}