using CSharpFunctionalExtensions;
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Application.UseCases.GetVenues;
using MakeSport.Contracts.Requests;
using MakeSport.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MakeSport.API.Controllers;

[ApiController]
[Route("venues")]
public class VenuesController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(CreateVenueRequest))]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateVenue(
        [FromBody] CreateVenueRequest request,
        [FromServices] CreateVenueUseCase useCase,
        CancellationToken cancellationToken)
    {
        var venue = await useCase.Handle(request, cancellationToken);
        
        /*var command = new CreateVenueCommand(request.Title, request.Description, request.Latitude, request.Longitude);
        var venue = await useCase.Handle(command, cancellationToken);

        return CreatedAtRoute(nameof(GetVenues), new VenueResponse()
        {
            Id = venue.Id,
            Name = venue.Name,
            Description = venue.Description,
            Latitude = venue.Latitude,
            Longitude = venue.Longitude
        });*/
    }
    
    [HttpGet(Name = nameof(GetVenues))]
    public async Task<EndpointResult<PaginationVenuesResponse>> GetVenues(
        [FromQuery] GetVenuesRequest request,
        [FromServices] GetVenuesUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.Handle(request, cancellationToken);
    }
}