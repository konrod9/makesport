namespace MakeSport.Application.UseCases.CreateVenue;

public record CreateVenueCommand(
    string Title,
    string Description,
    string City,
    string Street,
    int Building,
    double Latitude,
    double Longitude);