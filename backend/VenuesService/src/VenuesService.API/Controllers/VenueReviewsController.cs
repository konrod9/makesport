using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenuesService.API.Configuration;
using VenuesService.Application.UseCases.CreateReview;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Requests.Reviews;

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
}