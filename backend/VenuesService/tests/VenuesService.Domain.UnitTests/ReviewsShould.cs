using CSharpFunctionalExtensions;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Shared.Enums;

namespace VenuesService.Domain.UnitTests;

public class ReviewsShould
{
    private static readonly Guid CorrectReviewId = Guid.Parse("784DD77A-E026-4EAC-BFB6-4B66469837B8");
    private static readonly Guid CorrectVenueId = Guid.Parse("168AE119-27BF-4A45-A9A6-3C3057B63FA2");
    private static readonly Guid CorrectAuthorId = Guid.Parse("89C24AAF-4629-4D9A-901B-B4D75468C689");
    private const string CorrectAuthorName = "Bob Smith";
    private const int CorrectRating = 3;
    private const string CorrectText = "Lorem ipsum dolor sit amet";

    [Fact]
    public void CreateReview_WithGivenArguments()
    {
        var before = DateTimeOffset.UtcNow;

        var reviewResult = CreateReview();

        var after = DateTimeOffset.UtcNow;

        Assert.True(reviewResult.IsSuccess);

        var review = reviewResult.Value;
        Assert.Equal(CorrectReviewId, review.Id.Value);
        Assert.Equal(CorrectVenueId, review.VenueId.Value);
        Assert.Equal(CorrectAuthorId, review.AuthorId);
        Assert.Equal(CorrectAuthorName, review.AuthorName);
        Assert.Equal(CorrectRating, review.Rating.Value);
        Assert.Equal(CorrectText, review.Text);
        Assert.Equal(ReviewStatus.Published, review.Status);
        Assert.Null(review.UpdatedAt);
        Assert.InRange(review.CreatedAt, before, after);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void CreateReview_WithRatingInAllowedRange(int rating)
    {
        var reviewResult = CreateReview(rating: rating);

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal(rating, reviewResult.Value.Rating.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(6)]
    public void Fail_WhenRatingOutOfRange(int rating)
    {
        var reviewResult = CreateReview(rating: rating);

        AssertValidationFailure(reviewResult, "review.rating.invalid");
    }

    [Fact]
    public void CreateReview_WithTextAtMaxLength()
    {
        var text = new string('a', ReviewsConstraints.TextMaxLength);

        var reviewResult = CreateReview(text: text);

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal(ReviewsConstraints.TextMaxLength, reviewResult.Value.Text!.Length);
    }

    [Fact]
    public void Fail_WhenTextExceedsMaxLength()
    {
        var text = new string('a', ReviewsConstraints.TextMaxLength + 1);

        var reviewResult = CreateReview(text: text);

        AssertValidationFailure(reviewResult, "review.text.too.long");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeTextToNull_WhenTextIsBlank(string? text)
    {
        var reviewResult = CreateReview(text: text);

        Assert.True(reviewResult.IsSuccess);
        Assert.Null(reviewResult.Value.Text);
    }

    [Fact]
    public void TrimText_WhenTextHasSurroundingWhitespace()
    {
        var reviewResult = CreateReview(text: "  Отличная площадка  ");

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal("Отличная площадка", reviewResult.Value.Text);
    }

    [Fact]
    public void UseDefaultAuthorName_WhenAuthorNameIsNull()
    {
        var reviewResult = Review.Create(
            CorrectReviewId,
            CorrectVenueId,
            CorrectAuthorId,
            null!,
            CorrectRating,
            CorrectText);

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal("Пользователь", reviewResult.Value.AuthorName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UseDefaultAuthorName_WhenAuthorNameIsBlank(string authorName)
    {
        var reviewResult = CreateReview(authorName: authorName);

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal("Пользователь", reviewResult.Value.AuthorName);
    }

    [Fact]
    public void TrimAuthorName_WhenAuthorNameHasSurroundingWhitespace()
    {
        var reviewResult = CreateReview(authorName: "  Bob Smith  ");

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal(CorrectAuthorName, reviewResult.Value.AuthorName);
    }

    [Fact]
    public void TruncateAuthorName_WhenLongerThanMaxLength()
    {
        var authorName = new string('b', ReviewsConstraints.AuthorNameMaxLength + 20);

        var reviewResult = CreateReview(authorName: authorName);

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal(ReviewsConstraints.AuthorNameMaxLength, reviewResult.Value.AuthorName.Length);
        Assert.Equal(authorName[..ReviewsConstraints.AuthorNameMaxLength], reviewResult.Value.AuthorName);
    }

    [Fact]
    public void KeepAuthorName_WhenExactlyAtMaxLength()
    {
        var authorName = new string('b', ReviewsConstraints.AuthorNameMaxLength);

        var reviewResult = CreateReview(authorName: authorName);

        Assert.True(reviewResult.IsSuccess);
        Assert.Equal(authorName, reviewResult.Value.AuthorName);
    }

    [Fact]
    public void Update_ChangesRatingTextAndUpdatedAt()
    {
        var review = CreateValidReview();

        var result = review.Update(5, "  Обновлённый отзыв  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(5, review.Rating.Value);
        Assert.Equal("Обновлённый отзыв", review.Text);
        Assert.NotNull(review.UpdatedAt);
    }

    [Fact]
    public void Update_ClearsText_WhenTextIsBlank()
    {
        var review = CreateValidReview();

        var result = review.Update(CorrectRating, "   ");

        Assert.True(result.IsSuccess);
        Assert.Null(review.Text);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(6)]
    public void Update_Fails_WithoutMutatingReview_WhenRatingOutOfRange(int rating)
    {
        var review = CreateValidReview();
        var originalRating = review.Rating.Value;
        var originalText = review.Text;

        var result = review.Update(rating, "Новый текст");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("review.rating.invalid", Assert.Single(result.Error.Messages).Code);
        Assert.Equal(originalRating, review.Rating.Value);
        Assert.Equal(originalText, review.Text);
        Assert.Null(review.UpdatedAt);
    }

    [Fact]
    public void Update_Fails_WithoutMutatingReview_WhenTextExceedsMaxLength()
    {
        var review = CreateValidReview();
        var originalRating = review.Rating.Value;
        var originalText = review.Text;
        var text = new string('a', ReviewsConstraints.TextMaxLength + 1);

        var result = review.Update(5, text);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("review.text.too.long", Assert.Single(result.Error.Messages).Code);
        Assert.Equal(originalRating, review.Rating.Value);
        Assert.Equal(originalText, review.Text);
        Assert.Null(review.UpdatedAt);
    }

    [Fact]
    public void Hide_SetsHiddenStatusAndUpdatedAt()
    {
        var review = CreateValidReview();

        review.Hide();

        Assert.Equal(ReviewStatus.Hidden, review.Status);
        Assert.NotNull(review.UpdatedAt);
    }

    [Fact]
    public void Publish_SetsPublishedStatusAndUpdatedAt()
    {
        var review = CreateValidReview();
        review.Hide();

        review.Publish();

        Assert.Equal(ReviewStatus.Published, review.Status);
        Assert.NotNull(review.UpdatedAt);
    }

    private static void AssertValidationFailure(Result<Review, Error> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal(expectedCode, Assert.Single(result.Error.Messages).Code);
    }

    private static Review CreateValidReview()
    {
        var reviewResult = CreateReview();
        Assert.True(reviewResult.IsSuccess);

        return reviewResult.Value;
    }

    private static Result<Review, Error> CreateReview(
        int rating = CorrectRating,
        string? text = CorrectText,
        string authorName = CorrectAuthorName,
        Guid? reviewId = null,
        Guid? venueId = null,
        Guid? authorId = null)
    {
        return Review.Create(
            reviewId ?? CorrectReviewId,
            venueId ?? CorrectVenueId,
            authorId ?? CorrectAuthorId,
            authorName,
            rating,
            text);
    }
}
