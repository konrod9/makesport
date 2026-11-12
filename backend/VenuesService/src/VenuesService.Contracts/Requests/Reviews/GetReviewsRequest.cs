using VenuesService.Contracts.Enums;

namespace VenuesService.Contracts.Requests.Reviews;

public record GetReviewsRequest(
    int Page = 1,
    int PageSize = 20,
    ReviewSortField SortBy = ReviewSortField.CreatedAt,
    SortDirection SortDirection = SortDirection.Desc);