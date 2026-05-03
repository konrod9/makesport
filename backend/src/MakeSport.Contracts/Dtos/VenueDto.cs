namespace MakeSport.Contracts.Dtos;

public record VenueDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}