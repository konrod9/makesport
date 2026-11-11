namespace VenuesService.Domain.Reviews.ValueObjects;

public record ReviewId
{
    private ReviewId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }
    
    public static ReviewId NewId() => new(Guid.NewGuid());
    
    public static ReviewId Empty() => new(Guid.Empty);
    
    public static ReviewId Create(Guid id) => new(id);
    
    public static implicit operator ReviewId(Guid id) => new (id);

    public static implicit operator Guid(ReviewId reviewId)
    {
        ArgumentNullException.ThrowIfNull(reviewId);
        return reviewId.Value;
    }
}