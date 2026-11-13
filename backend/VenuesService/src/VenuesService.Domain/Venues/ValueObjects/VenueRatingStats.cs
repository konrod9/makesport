namespace VenuesService.Domain.Venues.ValueObjects;

public record VenueRatingStats(double Average, int Count)
{
    public static VenueRatingStats Empty => new(0, 0);
}