using MakeSport.Contracts.Dtos;

namespace MakeSport.Contracts.Responses;

public record PaginationVenuesResponse(
    IReadOnlyList<VenueDto> Venues,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);