using VenuesService.Contracts.Enums;

namespace VenuesService.Contracts.Dtos.Reviews;

public record MyReviewDto
{
    public Guid Id { get; init; }
    
    public Guid VenueId { get; init; }
    
    public string VenueTitle { get; init; } = string.Empty;
    
    public int Rating { get; init; }
    
    public string? Text { get; init; }
    
    public ReviewStatus Status { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? UpdatedAt { get; init; }
}