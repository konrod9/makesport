using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using MakeSport.Auth.JwtValidation;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Shared.Enums;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.IntegrationTests.Infrastructure;

namespace VenuesService.IntegrationTests;

[Collection("Integration")]
public class ChangeReviewStatusTests : VenuesTestsBase
{
    public ChangeReviewStatusTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ChangeStatus_Hide_ShouldHideReviewAndRecalculateRating()
    {
        // Arrange
        var token = CreateModeratorToken();

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var target = await CreateReviewAsync(venueIdVo, 5);
        var other = await CreateReviewAsync(venueIdVo, 3);

        await SetVenueRatingAsync(venueIdVo, 4.0, 2);

        // Act
        using var response = await SendChangeStatusAsync(target.Id.Value, ReviewStatus.Hidden, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        var updated = payload.Result;
        Assert.NotNull(updated);
        Assert.Equal(target.Id.Value, updated.Id);
        Assert.Equal(venueId, updated.VenueId);
        Assert.Equal(5, updated.Rating);
        Assert.Equal(ReviewStatus.Hidden, updated.Status);
        Assert.NotNull(updated.UpdatedAt);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(target.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Hidden, stored.Status);
            Assert.NotNull(stored.UpdatedAt);

            var otherStored = await db.Reviews.FindAsync(other.Id);
            Assert.NotNull(otherStored);
            Assert.Equal(ReviewStatus.Published, otherStored.Status);
            Assert.Null(otherStored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_Publish_ShouldPublishReviewAndRecalculateRating()
    {
        // Arrange
        var token = CreateModeratorToken();

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        await CreateReviewAsync(venueIdVo, 5);
        var hidden = await CreateReviewAsync(venueIdVo, 3, status: ReviewStatus.Hidden);

        // Скрытый отзыв не участвует в рейтинге: в кэше только опубликованный.
        await SetVenueRatingAsync(venueIdVo, 5.0, 1);

        // Act
        using var response = await SendChangeStatusAsync(hidden.Id.Value, ReviewStatus.Published, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        var updated = payload.Result;
        Assert.NotNull(updated);
        Assert.Equal(hidden.Id.Value, updated.Id);
        Assert.Equal(venueId, updated.VenueId);
        Assert.Equal(3, updated.Rating);
        Assert.Equal(ReviewStatus.Published, updated.Status);
        Assert.NotNull(updated.UpdatedAt);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(hidden.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Published, stored.Status);
            Assert.NotNull(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(2, venue.ReviewCount);
            Assert.Equal(4.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_HideLastPublishedReview_ShouldResetVenueRating()
    {
        // Arrange
        var token = CreateModeratorToken();

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 4);

        await SetVenueRatingAsync(venueIdVo, 4.0, 1);

        // Act
        using var response = await SendChangeStatusAsync(review.Id.Value, ReviewStatus.Hidden, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);
        Assert.Equal(ReviewStatus.Hidden, payload.Result!.Status);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Hidden, stored.Status);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(0, venue.ReviewCount);
            Assert.Equal(0.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_ShouldRecomputeFromReviews_WhenCachedRatingIsStale()
    {
        // Arrange
        var token = CreateModeratorToken();

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var target = await CreateReviewAsync(venueIdVo, 1);
        await CreateReviewAsync(venueIdVo, 3);

        // Кэш намеренно расходится с reviews (реальное среднее — 2.0).
        // Пересчёт должен идти из reviews, а не дельтой от venues.rating.
        await SetVenueRatingAsync(venueIdVo, 4.0, 2);

        // Act
        using var response = await SendChangeStatusAsync(target.Id.Value, ReviewStatus.Hidden, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await ExecuteInDb(async db =>
        {
            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_SameStatus_ShouldNotWrite()
    {
        // Arrange
        var token = CreateModeratorToken();

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 4);

        await SetVenueRatingAsync(venueIdVo, 4.0, 1);

        // Act
        using var response = await SendChangeStatusAsync(review.Id.Value, ReviewStatus.Published, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);
        Assert.Equal(ReviewStatus.Published, payload.Result!.Status);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Published, stored.Status);
            Assert.Null(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(4.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_NotFound_ShouldReturnNotFound()
    {
        // Arrange
        var token = CreateModeratorToken();

        // Act
        using var response = await SendChangeStatusAsync(Guid.NewGuid(), ReviewStatus.Hidden, token);

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
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"status\":null}")]
    [InlineData("{\"status\":99}")]
    public async Task ChangeStatus_InvalidStatus_ShouldReturnBadRequest(string body)
    {
        // Arrange
        var token = CreateModeratorToken();

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 3);

        await SetVenueRatingAsync(venueIdVo, 3.0, 1);

        // Act
        using var response = await SendRawChangeStatusAsync(review.Id.Value, body, token);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TestEnvelope<ReviewDto>>();
        Assert.NotNull(payload);
        Assert.True(payload.IsError);
        Assert.Null(payload.Result);

        var error = payload.Error;
        Assert.NotNull(error);
        Assert.Equal(ErrorType.Validation, error.Type);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Published, stored.Status);
            Assert.Null(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(3.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_ForbiddenForRegularUser()
    {
        // Arrange
        var token = TestJwtFactory.Create(Guid.NewGuid(), "Обычный пользователь", AuthRoles.User);

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var review = await CreateReviewAsync(venueIdVo, 5);

        await SetVenueRatingAsync(venueIdVo, 5.0, 1);

        // Act
        using var response = await SendChangeStatusAsync(review.Id.Value, ReviewStatus.Hidden, token);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(review.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Published, stored.Status);
            Assert.Null(stored.UpdatedAt);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(5.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_AdminCanChangeStatus()
    {
        // Arrange
        var token = TestJwtFactory.Create(Guid.NewGuid(), "Администратор", AuthRoles.Admin);

        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var target = await CreateReviewAsync(venueIdVo, 2);
        await CreateReviewAsync(venueIdVo, 4);

        await SetVenueRatingAsync(venueIdVo, 3.0, 2);

        // Act
        using var response = await SendChangeStatusAsync(target.Id.Value, ReviewStatus.Hidden, token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await ExecuteInDb(async db =>
        {
            var stored = await db.Reviews.FindAsync(target.Id);
            Assert.NotNull(stored);
            Assert.Equal(ReviewStatus.Hidden, stored.Status);

            var venue = await db.Venues.FindAsync(venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(1, venue.ReviewCount);
            Assert.Equal(4.0, venue.Rating);
        });
    }

    [Fact]
    public async Task ChangeStatus_Unauthorized_ShouldReturn401()
    {
        // Arrange
        var reviewId = Guid.NewGuid();

        // Act
        using var response = await SendChangeStatusAsync(reviewId, ReviewStatus.Hidden);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Bearer", response.Headers.WwwAuthenticate.ToString());
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    private static string CreateModeratorToken() =>
        TestJwtFactory.Create(Guid.NewGuid(), "Модератор", AuthRoles.Moderator);

    private async Task<HttpResponseMessage> SendChangeStatusAsync(
        Guid reviewId,
        ReviewStatus? status,
        string? token = null)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/venues/reviews/{reviewId}/status")
        {
            Content = JsonContent.Create(new ChangeReviewStatusRequest(status))
        };

        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await AppHttpClient.SendAsync(request);
    }

    private async Task<HttpResponseMessage> SendRawChangeStatusAsync(
        Guid reviewId,
        string body,
        string? token = null)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"/venues/reviews/{reviewId}/status")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await AppHttpClient.SendAsync(request);
    }
}
