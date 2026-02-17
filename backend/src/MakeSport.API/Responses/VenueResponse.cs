namespace MakeSport.API.Responses;

public class VenueResponse
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
}