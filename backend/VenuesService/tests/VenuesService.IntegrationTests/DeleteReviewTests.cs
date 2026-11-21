using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Shared.Enums;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.IntegrationTests.Infrastructure;

namespace VenuesService.IntegrationTests;

[Collection("Integration")]
public class DeleteReviewTests : VenuesTestsBase
{
    public DeleteReviewTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task DeleteReview_ShouldRecalculateRating()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");
        
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var reviewA = await CreateReviewAsync(venueIdVo, 1, authorId: authorId);
        var reviewB = await CreateReviewAsync(venueIdVo, 3);

        await SetVenueRatingAsync(venueIdVo, 2.0, 2);
        
        // Act
        using var response = await SendDeleteAsync(venueId, reviewA.Id.Value, token);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        await ExecuteInDb(async db =>
        {
            var review = await db.Reviews.FindAsync(reviewA.Id);
            Assert.Null(review);
            
            var otherReview = await db.Reviews.FindAsync(reviewB.Id);
            Assert.NotNull(otherReview);
            Assert.Equal(reviewB.Id.Value, otherReview.Id.Value);
            Assert.Equal(3, otherReview.Rating.Value);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(3.0, venue.Rating);
            Assert.Equal(1, venue.ReviewCount);
        });
    }

    [Fact]
    public async Task DeleteReview_LastReview_ShouldResetVenueRating()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 4, authorId: authorId);

        await SetVenueRatingAsync(venueIdVo, 4.0, 1);

        // Act
        using var response = await SendDeleteAsync(venueId, review.Id.Value, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        await ExecuteInDb(async db =>
        {
            Assert.Null(await db.Reviews.FindAsync(review.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(0, venue.ReviewCount);
            Assert.Equal(0.0, venue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_ShouldRecomputeFromReviews_WhenCachedRatingIsStale()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var reviewA = await CreateReviewAsync(venueIdVo, 1, authorId: authorId);
        var reviewB = await CreateReviewAsync(venueIdVo, 3);

        // Кэш намеренно расходится с reviews. Пересчёт должен идти из reviews, а не дельтой от venues.rating.
        await SetVenueRatingAsync(venueIdVo, 4.0, 2);

        // Act
        using var response = await SendDeleteAsync(venueId, reviewA.Id.Value, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await ExecuteInDb(async db =>
        {
            Assert.Null(await db.Reviews.FindAsync(reviewA.Id));
            Assert.NotNull(await db.Reviews.FindAsync(reviewB.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_NotAuthor_ShouldReturnForbidden()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var token = TestJwtFactory.Create(requesterId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3, authorId: authorId);
        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        // Act
        using var response = await SendDeleteAsync(venueId, review.Id.Value, token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);

        var error = payload.Error!;
        Assert.Equal(ErrorType.Authorization, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.not.author");

        await ExecuteInDb(async db =>
        {
            Assert.NotNull(await db.Reviews.FindAsync(review.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_NotFound_ShouldReturnNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3, authorId: authorId);
        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        // Act
        using var response = await SendDeleteAsync(venueId, Guid.NewGuid(), token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);

        var error = payload.Error!;
        Assert.Equal(ErrorType.NotFound, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.not.found");

        await ExecuteInDb(async db =>
        {
            Assert.NotNull(await db.Reviews.FindAsync(review.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_VenueMismatch_ShouldReturnNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var otherVenueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3, authorId: authorId);
        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        // Act
        using var response = await SendDeleteAsync(otherVenueId, review.Id.Value, token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);

        var error = payload.Error!;
        Assert.Equal(ErrorType.NotFound, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.not.found");

        await ExecuteInDb(async db =>
        {
            Assert.NotNull(await db.Reviews.FindAsync(review.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);

            var otherVenue = await db.Venues.FindAsync(VenueId.Create(otherVenueId));
            Assert.NotNull(otherVenue);
            Assert.Equal(0, otherVenue.ReviewCount);
            Assert.Equal(0.0, otherVenue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_WhenReviewHidden_ShouldNotChangeVenueRating()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var published = await CreateReviewAsync(venueIdVo, 5);
        var hidden = await CreateReviewAsync(
            venueId: venueIdVo,
            rating: 3,
            text: "Скрытый отзыв",
            authorId: authorId,
            status: ReviewStatus.Hidden);

        await SetVenueRatingAsync(venueIdVo, 5.0, 1);

        // Act
        using var response = await SendDeleteAsync(venueId, hidden.Id.Value, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await ExecuteInDb(async db =>
        {
            Assert.Null(await db.Reviews.FindAsync(hidden.Id));
            Assert.NotNull(await db.Reviews.FindAsync(published.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(5.0, venue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_OnlyHiddenReview_ShouldSucceed()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var hidden = await CreateReviewAsync(
            venueId: venueIdVo,
            rating: 3,
            text: "Скрытый отзыв",
            authorId: authorId,
            status: ReviewStatus.Hidden);

        // Act
        using var response = await SendDeleteAsync(venueId, hidden.Id.Value, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await ExecuteInDb(async db =>
        {
            Assert.Null(await db.Reviews.FindAsync(hidden.Id));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(0, venue.ReviewCount);
            Assert.Equal(0.0, venue.Rating);
        });
    }

    [Fact]
    public async Task DeleteReview_Unauthorized_ShouldReturn401()
    {
        // Arrange
        var venueId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();

        // Act
        using var response = await SendDeleteAsync(venueId, reviewId);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Bearer", response.Headers.WwwAuthenticate.ToString());
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    private async Task<HttpResponseMessage> SendDeleteAsync(Guid venueId, Guid reviewId, string? token = null)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/venues/{venueId}/reviews/{reviewId}");

        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await AppHttpClient.SendAsync(request);
    }
}