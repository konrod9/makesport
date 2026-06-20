using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Domain.Venues;

public class Venue
{
    // EF Core requires a parameterless constructor for materialization
    private Venue()
    {
    }

    private Venue(
        VenueId id, 
        string title, 
        string? description, 
        Address address, 
        Coordinates coordinates,
        bool isOpen, 
        bool hasLighting,
        bool isFree, 
        string? workingHours, 
        double rating, 
        int reviewCount,
        string surface, 
        string sportType)
    {
        Id = id;
        Title = title;
        Description = description;
        Address = address;
        Coordinates = coordinates;
        IsOpen = isOpen;
        HasLighting = hasLighting;
        IsFree = isFree;
        WorkingHours = workingHours;
        Rating = rating;
        ReviewCount = reviewCount;
        Surface = surface;
        SportType = sportType;
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

    public static Venue Create(
        VenueId id, 
        string title,
        string? description,
        Address address, 
        Coordinates location,
        bool isOpen, 
        bool hasLighting,
        bool isFree, 
        string? workingHours, 
        string surface, 
        string sportType)
    {
        var rating = 0;
        var reviewCount = 0;
        
        return new Venue(id, 
            title,
            description, 
            address,
            location, 
            isOpen, 
            hasLighting,
            isFree,
            workingHours,
            rating,
            reviewCount,
            surface,
            sportType);
    }
}