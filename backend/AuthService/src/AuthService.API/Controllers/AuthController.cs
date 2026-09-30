using AuthService.API.Configuration;
using AuthService.Application.UseCases.Login;
using AuthService.Application.UseCases.Refresh;
using AuthService.Application.UseCases.Register;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = AuthService.Application.UseCases.Register.RegisterRequest;

namespace AuthService.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController
{
    [HttpPost("register")]
    public async Task<EndpointResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        [FromServices] RegisterUseCase useCase,
        CancellationToken cancellationToken) => await useCase.Handle(request, cancellationToken);

    [HttpPost("login")]
    public async Task<EndpointResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginUseCase useCase,
        CancellationToken cancellationToken) => await useCase.Handle(request, cancellationToken);
    
    [HttpPost("refresh")]
    public async Task<EndpointResult<RefreshResponse>> Refresh(
        [FromBody] RefreshRequest request,
        [FromServices] RefreshUseCase useCase,
        CancellationToken cancellationToken) => await useCase.Handle(request, cancellationToken);
}