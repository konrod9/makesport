using VenuesService.Contracts.Dtos;

namespace VenuesService.Contracts.Responses;

public record VenuesResponse(
    IReadOnlyList<VenueDto> Venues,
    int TotalCount);