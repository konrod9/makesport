using VenuesService.Contracts.Dtos;

namespace VenuesService.Contracts.Requests;

public record CreateVenueRequest
{
    public string Title { get; init; }
    
    public string? Description { get; init; }
    
    public AddressDto Address { get; init; }
    
    public CoordinatesDto Coordinates { get; init; }
    
    public string SportType { get; init; } = string.Empty;
    
    public string Surface { get; init; } = string.Empty;
    
    public bool IsOpen { get; init; }
    
    public bool HasLighting { get; init; }
    
    public bool IsFree { get; init; }
    
    public string? WorkingHours { get; init; }
}