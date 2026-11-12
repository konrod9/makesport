namespace VenuesService.Contracts.Requests.Reviews;

public record UpdateReviewRequest(int Rating, string? Text);