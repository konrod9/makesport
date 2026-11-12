namespace VenuesService.Contracts.Requests.Reviews;

public record CreateReviewRequest(int Rating, string? Text);