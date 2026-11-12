using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Shared.Enums;

namespace VenuesService.Domain.UnitTests;

public class ReviewRatingShould
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void CreateRating_WithValueInAllowedRange(int rating)
    {
        var result = ReviewRating.Create(rating);

        Assert.True(result.IsSuccess);
        Assert.Equal(rating, result.Value.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(int.MaxValue)]
    public void Fail_WhenRatingOutOfRange(int rating)
    {
        var result = ReviewRating.Create(rating);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("review.rating.invalid", Assert.Single(result.Error.Messages).Code);
    }

    [Fact]
    public void ConvertRatingToIntImplicitly()
    {
        var rating = ReviewRating.Create(4).Value;

        int value = rating;

        Assert.Equal(4, value);
    }
}
