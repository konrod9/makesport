namespace MakeSport.API.Requests;

public class CreateVenueRequest
{
    public required string Title { get; set; }
    
    public string? Description { get; set; }

    public required string City { get; set; }
    
    public required string Street { get; set; }
    
    public int? Building { get; set; }

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}