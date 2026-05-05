using MakeSport.Contracts.Dtos;

namespace MakeSport.Contracts.Requests;

public record CreateVenueRequest(string Title, string? Description, AddressDto Address, CoordinatesDto Coordinates);