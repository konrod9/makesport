namespace MakeSport.Application.DTOs;

public record VenueDto(Guid Id, string Name, string Description, double Latitude, double Longitude);