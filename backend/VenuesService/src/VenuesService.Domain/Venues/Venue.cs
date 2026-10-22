using NetTopologySuite.Geometries;
using VenuesService.Domain.Venues.ValueObjects;
using Coordinates = VenuesService.Domain.Venues.ValueObjects.Coordinates;

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
        bool isOpen,
        bool hasLighting,
        bool isFree,
        WorkingHours workingHours,
        double rating,
        int reviewCount,
        string surface,
        string sportType,
        Guid? createdByUserId,
        DateTimeOffset createdAt,
        Point location)
    {
        Id = id;
        Title = title;
        Description = description;
        Address = address;
        IsOpen = isOpen;
        HasLighting = hasLighting;
        IsFree = isFree;
        WorkingHours = workingHours;
        Rating = rating;
        ReviewCount = reviewCount;
        Surface = surface;
        SportType = sportType;
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
        Location = location;
    }

    public VenueId Id { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public bool IsOpen { get; private set; }
    public bool HasLighting { get; private set; }
    public bool IsFree { get; private set; }

    public WorkingHours WorkingHours { get; private set; }

    public double Rating { get; private set; }
    public int ReviewCount { get; private set; }

    public string SportType { get; private set; }

    public string Surface { get; private set; }

    public Address Address { get; private set; }

    public Point Location { get; private set; }

    public Coordinates Coordinates => Coordinates.Create(Location.Y, Location.X).Value;

    public Guid VideoId { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Venue Create(
        VenueId id,
        string title,
        string? description,
        Address address,
        Coordinates location,
        bool isOpen,
        bool hasLighting,
        bool isFree,
        WorkingHours workingHours,
        string surface,
        string sportType,
        Guid? userId)
    {
        var rating = 0;
        var reviewCount = 0;

        var point = new Point(location.Longitude, location.Latitude) { SRID = 4326 };

        return new Venue(id,
            title,
            description,
            address,
            isOpen,
            hasLighting,
            isFree,
            workingHours,
            rating,
            reviewCount,
            surface,
            sportType,
            userId,
            DateTimeOffset.UtcNow, 
            point);
    }
}