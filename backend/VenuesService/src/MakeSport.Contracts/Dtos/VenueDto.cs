namespace MakeSport.Contracts.Dtos;

public record VenueDto
{
    public Guid Id { get; init; }
    
    public string Title { get; init; } = string.Empty;
    
    public string? Description { get; init; }
    
    public AddressDto Address { get; init; }
    
    public CoordinatesDto Coordinates { get; init; }

    public MediaDto? Video { get; init; }

    public IReadOnlyList<MediaDto> Images { get; init; } = [];
}