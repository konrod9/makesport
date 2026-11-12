namespace VenuesService.Contracts.Dtos.Reviews;

public record VenueRatingSummaryDto
{
    public double AverageRating { get; init; }

    public int ReviewCount { get; init; }

    public IReadOnlyDictionary<int, int> Distribution { get; init; }
        = new Dictionary<int, int>();
}