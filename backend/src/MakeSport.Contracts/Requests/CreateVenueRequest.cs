namespace MakeSport.Contracts.Requests;

public record CreateVenueRequest
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    public string City { get; set; } = string.Empty;
    
    public string Street { get; set; } = string.Empty;
    
    public int? Building { get; set; }

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}