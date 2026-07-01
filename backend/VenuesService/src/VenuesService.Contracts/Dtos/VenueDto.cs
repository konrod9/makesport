namespace VenuesService.Contracts.Dtos;

public record VenueDto
{
    public Guid Id { get; init; }
    
    public string Title { get; init; } = string.Empty;
    
    public string? Description { get; init; }
    
    public string SportType { get; init; } = string.Empty;
    
    public string Surface { get; init; } = string.Empty;
    
    public double Rating { get; init; }
    
    public int ReviewCount { get; init; }
    
    public bool IsOpen { get; init; }
    
    public bool HasLighting { get; init; }
    
    public bool IsFree { get; init; }
    
    public string? WorkingHours { get; init; }
    
    public AddressDto Address { get; init; }
    
    public CoordinatesDto Coordinates { get; init; }

    public MediaDto? Video { get; set; }

    public IReadOnlyList<MediaDto> Images { get; init; } = [];
}