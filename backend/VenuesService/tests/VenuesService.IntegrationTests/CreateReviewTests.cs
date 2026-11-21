using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Shared.Enums;
using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.IntegrationTests.Infrastructure;

namespace VenuesService.IntegrationTests;

[Collection("Integration")]
public class CreateReviewTests : VenuesTestsBase
{
    public CreateReviewTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateReview_ShouldCreateAndUpdateVenueRating()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/venues/{venueId}/reviews")
        {
            Content = JsonContent.Create(new CreateReviewRequest(5, "Отличная площадка"))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        using HttpResponseMessage response = await AppHttpClient.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ReviewEnvelope>();
        Assert.NotNull(payload);
        Assert.False(payload.IsError);
        Assert.Null(payload.Error);

        var review = payload.Result;
        Assert.NotNull(review);
        Assert.NotEqual(Guid.Empty, review!.Id);
        Assert.Equal(venueId, review.VenueId);
        Assert.Equal(authorId, review.AuthorId);
        Assert.Equal("Тестовый пользователь", review.AuthorName);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Отличная площадка", review.Text);
        Assert.Equal(ReviewStatus.Published, review.Status);

        await ExecuteInDb(async db =>
        {
            var venue = await db.Venues.AsNoTracking().SingleAsync(v => v.Id == venueIdVo);
            Assert.NotNull(venue);
            Assert.Equal(5.0, venue.Rating);
            Assert.Equal(1, venue.ReviewCount);

            var reviewInDb = await db.Reviews.AsNoTracking().SingleAsync(r => r.VenueId == venueIdVo);
            Assert.Equal(review.Id, reviewInDb.Id.Value);
            Assert.Equal(authorId, reviewInDb.AuthorId);
            Assert.Equal(5, reviewInDb.Rating.Value);
            Assert.Equal(ReviewStatus.Published, reviewInDb.Status);
        });
    }

    [Fact]
    public async Task CreateReview_Duplicate_ShouldReturnConflict()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var reviewRating = 3;
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        using (var firstRequest = CreateReviewRequest(venueId, token, reviewRating, "Первый отзыв"))
        {
            using var firstResponse = await AppHttpClient.SendAsync(firstRequest);
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/venues/{venueId}/reviews")
        {
            Content = JsonContent.Create(new CreateReviewRequest(5, "Отличная площадка"))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        using HttpResponseMessage response = await AppHttpClient.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ReviewEnvelope>();
        Assert.NotNull(payload);
        Assert.True(payload!.IsError);
        Assert.Null(payload.Result);
        Assert.NotNull(payload.Error);
        Assert.Equal(ErrorType.Conflict, payload.Error!.Type);
        Assert.Contains(payload.Error.Messages, m => m.Code == "review.already.exists");

        await ExecuteInDb(async db =>
        {
            var venue = await db.Venues.AsNoTracking().SingleAsync(v => v.Id == venueIdVo);
            Assert.Equal(3.0, venue.Rating);
            Assert.Equal(1, venue.ReviewCount);

            var reviews = await db.Reviews.AsNoTracking()
                .Where(r => r.VenueId == venueIdVo)
                .ToListAsync();

            var review = Assert.Single(reviews);
            Assert.Equal(reviewRating, review.Rating.Value);
            Assert.Equal("Первый отзыв", review.Text);
        });
    }

    [Theory]
    [InlineData(6)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateReview_InvalidRating_ShouldReturnBadRequest(int rating)
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/venues/{venueId}/reviews")
        {
            Content = JsonContent.Create(new CreateReviewRequest(rating, "Отличная площадка"))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        using HttpResponseMessage response = await AppHttpClient.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ReviewEnvelope>();
        Assert.NotNull(payload);
        Assert.True(payload!.IsError);
        Assert.Null(payload.Result);
        Assert.NotNull(payload.Error);
        Assert.Equal(ErrorType.Validation, payload.Error!.Type);
        Assert.Contains(payload.Error.Messages, m => m.Code == "review.rating.invalid");

        await ExecuteInDb(async db =>
        {
            var venue = await db.Venues.AsNoTracking().SingleAsync(v => v.Id == venueIdVo);
            Assert.Equal(0.0, venue.Rating);
            Assert.Equal(0, venue.ReviewCount);

            Assert.False(await db.Reviews.AsNoTracking().AnyAsync(r => r.VenueId == venueIdVo));
        });
    }

    [Fact]
    public async Task CreateReview_TextTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);
        var token = TestJwtFactory.Create(authorId, "Тестовый пользователь");

        var text = new string('a', ReviewsConstraints.TextMaxLength + 1);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/venues/{venueId}/reviews");
        httpRequest.Content = JsonContent.Create(new CreateReviewRequest(4, text));
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        using HttpResponseMessage response = await AppHttpClient.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ReviewEnvelope>();
        Assert.NotNull(payload);
        Assert.True(payload!.IsError);
        Assert.Null(payload.Result);
        Assert.NotNull(payload.Error);
        Assert.Equal(ErrorType.Validation, payload.Error!.Type);
        Assert.Contains(payload.Error.Messages, m => m.Code == "review.text.too.long");

        await ExecuteInDb(async db =>
        {
            var venue = await db.Venues.AsNoTracking().SingleAsync(v => v.Id == venueIdVo);
            Assert.Equal(0.0, venue.Rating);
            Assert.Equal(0, venue.ReviewCount);

            Assert.False(await db.Reviews.AsNoTracking().AnyAsync(r => r.VenueId == venueIdVo));
        });
    }

    [Fact]
    public async Task CreateReview_Unauthorized_ShouldReturn401()
    {
        // Arrange
        var venueId = Guid.NewGuid();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/venues/{venueId}/reviews");
        httpRequest.Content = JsonContent.Create(new CreateReviewRequest(5, "Отличная площадка"));

        // Act
        using HttpResponseMessage response = await AppHttpClient.SendAsync(httpRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("Bearer", response.Headers.WwwAuthenticate.ToString());
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ConcurrentCreate_ShouldKeepRatingConsistent()
    {
        // Arrange
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);

        var authorA = Guid.NewGuid();
        var authorB = Guid.NewGuid();

        // Act
        var responses = await Task.WhenAll(
            SendCreateReviewAsync(venueId, authorA, "Автор А", 5),
            SendCreateReviewAsync(venueId, authorB, "Автор Б", 3));

        try
        {
            // Assert
            foreach (var response in responses)
            {
                var body = await response.Content.ReadAsStringAsync();
                Assert.True(
                    response.StatusCode == HttpStatusCode.OK,
                    $"Ожидали 200, получили {(int)response.StatusCode}. Тело: {body}");
            }

            await ExecuteInDb(async db =>
            {
                var venue = await db.Venues.AsNoTracking().SingleAsync(v => v.Id == venueIdVo);
                Assert.Equal(2, venue.ReviewCount);
                Assert.Equal(4.0, venue.Rating);

                var reviews = await db.Reviews.AsNoTracking().Where(r => r.VenueId == venueIdVo).ToListAsync();
                Assert.Equal(2, reviews.Count);
                Assert.Equal(2, reviews.Select(r => r.Id.Value).Distinct().Count());
                Assert.Equal([3, 5], reviews.Select(r => r.Rating.Value).Order().ToArray());
                Assert.Equal(
                    new[] { authorA, authorB }.Order().ToArray(),
                    reviews.Select(r => r.AuthorId).Order().ToArray());
            });
        }
        finally
        {
            foreach (var response in responses)
                response.Dispose();
        }
    }

    private async Task<Guid> CreateVenueAsync()
    {
        var venueId = Guid.NewGuid();

        var venue = Venue.Create(
            id: VenueId.Create(venueId),
            title: $"Test venue {venueId}",
            address: Address.Create("Одинцов", "Можайское шоссе", "12А").Value,
            location: Coordinates.Create(55.670916, 37.267876).Value,
            isOpen: true,
            isFree: true,
            hasLighting: true,
            workingHours: WorkingHours.Create("08:00", "22:00").Value,
            surface: "Резиновое",
            sportType: "Баскетбол",
            userId: null,
            description: null);

        await ExecuteInDb(async dbContext =>
        {
            dbContext.Venues.Add(venue);
            await dbContext.SaveChangesAsync();
        });

        return venueId;
    }

    private async Task<HttpResponseMessage> SendCreateReviewAsync(
        Guid venueId, Guid authorId, string authorName, int rating)
    {
        var token = TestJwtFactory.Create(authorId, authorName);

        using var request = CreateReviewRequest(venueId, token, rating, $"Отзыв: {authorName}");

        return await AppHttpClient.SendAsync(request);
    }

    private static HttpRequestMessage CreateReviewRequest(Guid venueId, string token, int rating, string? text)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/venues/{venueId}/reviews")
        {
            Content = JsonContent.Create(new CreateReviewRequest(rating, text))
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private sealed record ReviewEnvelope(ReviewDto? Result, Error? Error, bool IsError);
}