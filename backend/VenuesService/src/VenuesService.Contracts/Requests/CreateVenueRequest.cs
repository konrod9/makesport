using VenuesService.Contracts.Dtos;

namespace VenuesService.Contracts.Requests;

public record CreateVenueRequest(string Title, string? Description, AddressDto Address, CoordinatesDto Coordinates);