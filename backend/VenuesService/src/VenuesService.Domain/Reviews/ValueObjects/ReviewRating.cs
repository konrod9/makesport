using CSharpFunctionalExtensions;
using VenuesService.Domain.Shared;

namespace VenuesService.Domain.Reviews.ValueObjects;

public record ReviewRating
{
    private ReviewRating(int value) => Value = value;

    public int Value { get; }

    public static Result<ReviewRating, Error> Create(int rating)
    {
        if (rating is < 1 or > 5)
            return ReviewErrors.RatingOutOfRange();

        return new ReviewRating(rating);
    }

    public static implicit operator int(ReviewRating rating)
    {
        ArgumentNullException.ThrowIfNull(rating);
        return rating.Value;
    }
}