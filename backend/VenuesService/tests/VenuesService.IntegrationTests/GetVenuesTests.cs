using CSharpFunctionalExtensions;
using FileService.Contracts.HttpCommunication;
using FileService.Contracts.Shared;
using Microsoft.AspNetCore.WebUtilities;
using VenuesService.Contracts.Requests;
using VenuesService.Contracts.Responses;
using VenuesService.IntegrationTests.Infrastructure;

namespace VenuesService.IntegrationTests;

public class GetVenuesTests : VenuesTestsBase
{
    private readonly IntegrationTestsWebFactory _factory;

    public GetVenuesTests(IntegrationTestsWebFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetVenues_ShouldReturnVenues()
    {
        // Arrange
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var getVenuesRequest = new GetVenuesRequest(null, null, null, null, null, null, null, 1, 3);

        var queryParams = new Dictionary<string, string>()
        {
            {
                "page", getVenuesRequest.Page.ToString()
            },
            {
                "pageSize", getVenuesRequest.PageSize.ToString()
            }  
        };
        
        var url = QueryHelpers.AddQueryString("/api/venues", queryParams!);
        
        // Act
        HttpResponseMessage venuesResponse =
            await AppHttpClient.GetAsync(url, cancellationToken);

        Result<PaginationVenuesResponse, Error> venuesResult = await venuesResponse
            .HandleResponseAsync<PaginationVenuesResponse>(cancellationToken: cancellationToken);
        
        // Assert
        Assert.True(venuesResult.IsSuccess);
        Assert.Equal(3, venuesResult.Value.Venues.Count);
        Assert.Equal(3, venuesResult.Value.Venues.Select(v => v.Video).Count());
    }
}