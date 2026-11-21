using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Shared.Enums;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.IntegrationTests.Infrastructure;

namespace VenuesService.IntegrationTests;

public class UpdateReviewTests : VenuesTestsBase
{
    public UpdateReviewTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateReview_ShouldRecalculateRating()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var reviewA = await CreateReviewAsync(venueIdVo, 1);
        var reviewB = await CreateReviewAsync(
            venueId: venueIdVo,
            rating: 3,
            text: "Отличная площадка",
            authorId: authorId);

        await SetVenueRatingAsync(venueIdVo, 2.0, 2);

        // Act
        using var response = await SendUpdateAsync(venueId, reviewB.Id.Value, 5, "Хорошая площадка", token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        var updated = payload.Result;
        Assert.NotNull(updated);
        Assert.Equal(reviewB.Id.Value, updated.Id);
        Assert.Equal(venueId, updated.VenueId);
        Assert.Equal(authorId, updated.AuthorId);
        Assert.Equal("Test author", updated.AuthorName);
        Assert.Equal(5, updated.Rating);
        Assert.Equal("Хорошая площадка", updated.Text);
        Assert.Equal(ReviewStatus.Published, updated.Status);
        Assert.NotNull(updated.UpdatedAt);

        await ExecuteInDb(async db =>
        {
            var review = await db.Reviews.FindAsync(reviewB.Id);
            Assert.NotNull(review);
            Assert.Equal(5, review.Rating.Value);
            Assert.Equal("Хорошая площадка", review.Text);
            Assert.NotNull(review.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(2, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);

            var reviewAAfter = await db.Reviews.FindAsync(reviewA.Id);
            Assert.NotNull(reviewAAfter);
            Assert.Equal(1, reviewAAfter.Rating.Value);
            Assert.Equal("Отличная площадка", reviewAAfter.Text);
            Assert.Null(reviewAAfter.UpdatedAt);
        });
    }

    [Fact]
    public async Task UpdateReview_NotAuthor_ShouldReturnForbidden()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var otherAuthorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        await CreateReviewAsync(venueIdVo, 1, authorId: authorId);
        var foreignReview = await CreateReviewAsync(
            venueId: venueIdVo,
            rating: 3,
            text: "Отличная площадка",
            authorId: otherAuthorId);

        await SetVenueRatingAsync(venueIdVo, 2.0, 2);

        // Act
        using var response = await SendUpdateAsync(
            venueId, foreignReview.Id.Value, 5, "Чужой отзыв", token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.Authorization, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.not.author");

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(foreignReview.Id);
            Assert.NotNull(stored);
            Assert.Equal(3, stored.Rating.Value);
            Assert.Equal("Отличная площадка", stored.Text);
            Assert.Null(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(2, venue.ReviewCount);
            Assert.Equal(2.0, venue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_NotFound_ShouldReturnNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);

        // Act
        using var response = await SendUpdateAsync(
            venueId, Guid.NewGuid(), 5, "Хорошая площадка", token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.NotFound, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.not.found");

        await ExecuteInDb(async db =>
        {
            Assert.False(await db.Reviews.AnyAsync(r => r.VenueId == venueIdVo));

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(0, venue.ReviewCount);
            Assert.Equal(0.0, venue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_VenueMismatch_ShouldReturnNotFound()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var otherVenueId = await CreateVenueAsync();
        var review = await CreateReviewAsync(VenueId.Create(venueId), 3, authorId: authorId);

        // Act
        using var response = await SendUpdateAsync(
            otherVenueId, review.Id.Value, 5, "Хорошая площадка", token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.NotFound, error.Type);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(3, stored.Rating.Value);
            Assert.Equal("Отличная площадка", stored.Text);
            Assert.Null(stored.UpdatedAt);

            var otherVenue = await db.Venues.FindAsync(VenueId.Create(otherVenueId));
            Assert.NotNull(otherVenue);
            Assert.Equal(0, otherVenue.ReviewCount);
            Assert.Equal(0.0, otherVenue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_ForeignAuthorAndVenue_ShouldReturnNotFound()
    {
        // Arrange
        var reviewAuthorId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var token = TestJwtFactory.Create(requesterId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var otherVenueId = await CreateVenueAsync();
        var review = await CreateReviewAsync(VenueId.Create(venueId), 3, authorId: reviewAuthorId);

        // Act
        using var response = await SendUpdateAsync(
            otherVenueId, review.Id.Value, 5, "Хорошая площадка", token);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task UpdateReview_InvalidRating_ShouldReturnBadRequest(int rating)
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3, authorId: authorId);
        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        // Act
        using var response = await SendUpdateAsync(
            venueId, review.Id.Value, rating, "Хорошая площадка", token);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.rating.invalid");

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(3, stored.Rating.Value);
            Assert.Equal("Отличная площадка", stored.Text);
            Assert.Null(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_TextTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3, authorId: authorId);
        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        var text = new string('a', ReviewsConstraints.TextMaxLength + 1);

        // Act
        using var response = await SendUpdateAsync(
            venueId, review.Id.Value, 4, text, token);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.Validation, error.Type);
        Assert.Contains(error.Messages, m => m.Code == "review.text.too.long");

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(3, stored.Rating.Value);
            Assert.Equal("Отличная площадка", stored.Text);
            Assert.Null(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_EmptyText_ShouldStoreNull()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3, authorId: authorId);
        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        // Act
        using var response = await SendUpdateAsync(venueId, review.Id.Value, 4, "   ", token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        var updated = payload.Result;
        Assert.NotNull(updated);
        Assert.Equal(4, updated.Rating);
        Assert.Null(updated.Text);
        Assert.NotNull(updated.UpdatedAt);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(4, stored.Rating.Value);
            Assert.Null(stored.Text);
            Assert.NotNull(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(4.0, venue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_WhenReviewHidden_ShouldNotChangeVenueRating()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        await CreateReviewAsync(venueIdVo, 5);
        var hidden = await CreateReviewAsync(
            venueId: venueIdVo,
            rating: 3,
            text: "Отличная площадка",
            authorId: authorId,
            status: ReviewStatus.Hidden);

        await SetVenueRatingAsync(venueIdVo, 5.0, 1);

        // Act
        using var response = await SendUpdateAsync(
            venueId, hidden.Id.Value, 1, "Скрытый отзыв", token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        var updated = payload.Result;
        Assert.NotNull(updated);
        Assert.Equal(1, updated.Rating);
        Assert.Equal("Скрытый отзыв", updated.Text);
        Assert.Equal(ReviewStatus.Hidden, updated.Status);
        Assert.NotNull(updated.UpdatedAt);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(hidden.Id);
            Assert.NotNull(stored);
            Assert.Equal(1, stored.Rating.Value);
            Assert.Equal("Скрытый отзыв", stored.Text);
            Assert.Equal(ReviewStatus.Hidden, stored.Status);
            Assert.NotNull(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(5.0, venue.Rating);
        });
    }

    [Fact]
    public async Task UpdateReview_Unauthorized_ShouldReturn401()
    {
        // Arrange
        var venueId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/venues/{venueId}/reviews/{reviewId}")
        {
            Content = JsonContent.Create(new UpdateReviewRequest(5, "Хорошая площадка"))
        };

        // Act
        using var response = await AppHttpClient.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Bearer", response.Headers.WwwAuthenticate.ToString());
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    private async Task<HttpResponseMessage> SendUpdateAsync(
        Guid venueId,
        Guid reviewId,
        int rating,
        string? text,
        string? token = null)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/venues/{venueId}/reviews/{reviewId}")
        {
            Content = JsonContent.Create(new UpdateReviewRequest(rating, text))
        };

        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await AppHttpClient.SendAsync(request);
    }
}
