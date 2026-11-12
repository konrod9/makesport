using VenuesService.Contracts.Dtos.Reviews;

namespace VenuesService.Contracts.Responses.Reviews;

public record PaginationReviewsResponse(
    IReadOnlyList<ReviewDto> Reviews,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);