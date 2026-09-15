using AuthService.API.Configuration;
using AuthService.Application.UseCases.Register;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController
{
    [HttpPost("register")]
    public async Task<EndpointResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        [FromServices] RegisterUseCase useCase,
        CancellationToken cancellationToken)
    {
        return await useCase.Handle(request, cancellationToken);
    }
}