using MakeSport.API.Requests;
using MakeSport.API.Responses;
using MakeSport.Application.UseCases.CreateVenue;
using MakeSport.Application.UseCases.GetVenues;
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
        [FromServices] ICreateVenueUseCase useCase,
        CancellationToken cancellationToken)
    {
        var command = new CreateVenueCommand(request.Name, request.Description, request.Latitude, request.Longitude);
        var venue = await useCase.Handle(command, cancellationToken);

        return CreatedAtRoute(nameof(GetVenues), new VenueResponse()
        {
            Id = venue.Id,
            Name = venue.Name,
            Description = venue.Description
        });
    }
    
    [HttpGet(Name = nameof(GetVenues))]
    [ProducesResponseType(200, Type = typeof(VenueResponse[]))]
    public async Task<IActionResult> GetVenues(
        [FromServices] IGetVenuesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var query = new GetVenuesQuery();
        var venues = await useCase.Handle(query, cancellationToken);
        
        return Ok(venues.Select(v => new VenueResponse()
        {
                Id = v.Id,
                Name = v.Name,
                Description = v.Description,
                Latitude = v.Latitude,
                Longitude = v.Longitude
        }));
    }
}