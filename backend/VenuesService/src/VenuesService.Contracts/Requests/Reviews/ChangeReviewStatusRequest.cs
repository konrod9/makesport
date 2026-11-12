using VenuesService.Contracts.Enums;

namespace VenuesService.Contracts.Requests.Reviews;

public record ChangeReviewStatusRequest(ReviewStatus? Status);