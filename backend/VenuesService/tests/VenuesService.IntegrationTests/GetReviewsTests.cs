using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Contracts.Responses.Reviews;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.IntegrationTests.Infrastructure;

namespace VenuesService.IntegrationTests;

public class GetReviewsTests : VenuesTestsBase
{
    public GetReviewsTests(IntegrationTestsWebFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GetReviews_ShouldReturnOnlyPublishedWithPagination()
    {
        // Arrange
        var venueId = await CreateVenueAsync();
        var venueIdVo = VenueId.Create(venueId);

        var lowest = await CreateReviewAsync(venueIdVo, 3);
        var highest = await CreateReviewAsync(venueIdVo, 5);
        var hidden = await CreateReviewAsync(venueIdVo, 4, status: ReviewStatus.Hidden);

        var otherVenueId = VenueId.Create(await CreateVenueAsync());
        var foreignReview = await CreateReviewAsync(otherVenueId, 4);

        var url = QueryHelpers.AddQueryString($"/venues/{venueId}/reviews",
            new Dictionary<string, string?>
            {
                ["page"] = "1",
                ["pageSize"] = "1",
                ["sortBy"] = nameof(ReviewSortField.Rating),
                ["sortDirection"] = nameof(SortDirection.Desc)
            });

        // Act
        using HttpResponseMessage response = await AppHttpClient.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var envelope = await response.Content.ReadFromJsonAsync<PagedEnvelope>();
        Assert.NotNull(envelope);
        Assert.False(envelope!.IsError);
        Assert.NotNull(envelope.Result);

        var page = envelope.Result!;
        Assert.Equal(1, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(2, page.TotalPages);

        var only = Assert.Single(page.Reviews);
        Assert.Equal(highest.Id.Value, only.Id);
        Assert.Equal(5, only.Rating);
        Assert.Equal(ReviewStatus.Published, only.Status);

        Assert.DoesNotContain(hidden.Id.Value, page.Reviews.Select(r => r.Id));
        Assert.DoesNotContain(foreignReview.Id.Value, page.Reviews.Select(r => r.Id));

        // вторая страница
        var secondUrl = QueryHelpers.AddQueryString($"/venues/{venueId}/reviews",
            new Dictionary<string, string?>
            {
                ["page"] = "2",
                ["pageSize"] = "1",
                ["sortBy"] = nameof(ReviewSortField.Rating),
                ["sortDirection"] = nameof(SortDirection.Desc)
            });

        using var secondResponse = await AppHttpClient.GetAsync(secondUrl);
        var secondEnvelope = await secondResponse.Content.ReadFromJsonAsync<PagedEnvelope>();

        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var second = Assert.Single(secondEnvelope!.Result!.Reviews);
        Assert.Equal(lowest.Id.Value, second.Id);
    }

    private sealed record PagedEnvelope(
        PaginationReviewsResponse? Result, Error? Error, bool IsError);
}