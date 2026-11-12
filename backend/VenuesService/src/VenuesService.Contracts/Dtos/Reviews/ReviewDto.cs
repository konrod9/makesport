using VenuesService.Contracts.Enums;

namespace VenuesService.Contracts.Dtos.Reviews;

public record ReviewDto
{
    public Guid Id { get; init; }
    
    public Guid VenueId { get; init; }
    
    public Guid AuthorId { get; init; }
    
    public string AuthorName { get; init; } = string.Empty;
    
    public int Rating { get; init; }
    
    public string? Text { get; init; }
    
    public ReviewStatus Status { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? UpdatedAt { get; init; }
}