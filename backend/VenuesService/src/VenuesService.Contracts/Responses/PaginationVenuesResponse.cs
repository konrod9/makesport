using VenuesService.Contracts.Dtos;

namespace VenuesService.Contracts.Responses;

public record PaginationVenuesResponse(
    IReadOnlyList<VenueDto> Venues,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);