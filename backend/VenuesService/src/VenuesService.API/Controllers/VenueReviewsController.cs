using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenuesService.API.Configuration;
using VenuesService.Application.UseCases.CreateReview;
using VenuesService.Application.UseCases.GetVenueReviews;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Contracts.Responses.Reviews;

namespace VenuesService.API.Controllers;

[ApiController]
[Route("venues")]
public class VenueReviewsController : ControllerBase
{
    [HttpPost("{venueId:guid}/reviews")]
    [Authorize]
    public async Task<EndpointResult<ReviewDto>> CreateReview(
        [FromBody] CreateReviewRequest request,
        [FromRoute] Guid venueId,
        [FromServices] CreateReviewUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, venueId, ct);

    [HttpGet("{venueId:guid}/reviews")]
    [ProducesResponseType(200, Type = typeof(PaginationReviewsResponse))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<EndpointResult<PaginationReviewsResponse>> GetReviews(
        [FromQuery] GetReviewsRequest request,
        [FromRoute] Guid venueId,
        [FromServices] GetVenueReviewsUseCase useCase,
        CancellationToken ct) => await useCase.Handle(request, venueId, ct);
}