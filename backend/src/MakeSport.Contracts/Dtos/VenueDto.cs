using MakeSport.Contracts.Requests;

namespace MakeSport.Contracts.Dtos;

public record VenueDto(Guid Id, string Title, string? Description, AddressDto Address, CoordinatesDto Coordinates);