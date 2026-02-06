using MakeSport.API.Requests;
using MakeSport.API.Responses;
using MakeSport.Application.UseCases;
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
        var command = new CreateVenueCommand(request.Name, request.Description);
        var venue = await useCase.Handle(command, cancellationToken);
        
        // TODO: поменять на CreateAtRoute после того как будет добавлен GetVenue
        return Ok(new CreateVenueResponse()
        {
            Id = venue.Id,
            Name = venue.Name,
            Description = venue.Description
        });
    }
}