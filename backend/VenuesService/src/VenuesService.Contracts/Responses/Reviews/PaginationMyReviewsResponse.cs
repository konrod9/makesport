using VenuesService.Contracts.Dtos.Reviews;

namespace VenuesService.Contracts.Responses.Reviews;

public record PaginationMyReviewsResponse(
    IReadOnlyList<MyReviewDto> Reviews,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);