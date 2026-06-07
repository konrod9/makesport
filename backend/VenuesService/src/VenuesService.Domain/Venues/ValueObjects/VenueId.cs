namespace VenuesService.Domain.Venues.ValueObjects;

public record VenueId
{
    private VenueId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }
    
    public static VenueId NewId() => new(Guid.NewGuid());
    
    public static VenueId Empty() => new(Guid.Empty);
    
    public static VenueId Create(Guid id) => new(id);
    
    public static implicit operator VenueId(Guid id) => new (id);

    public static implicit operator Guid(VenueId venueId)
    {
        ArgumentNullException.ThrowIfNull(venueId);
        return venueId.Value;
    }
}