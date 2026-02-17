namespace MakeSport.Application.UseCases.CreateVenue;

public record CreateVenueCommand(string Name, string Description, double Latitude, double Longitude);