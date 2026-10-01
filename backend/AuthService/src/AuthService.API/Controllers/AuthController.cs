using AuthService.API.Configuration;
using AuthService.Application;
using AuthService.Application.UseCases.GetCurrentUser;
using AuthService.Application.UseCases.Login;
using AuthService.Application.UseCases.Logout;
using AuthService.Application.UseCases.Refresh;
using AuthService.Application.UseCases.Register;
using Microsoft.AspNetCore.Authorization;
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
    
    [HttpPost("logout")]
    public async Task<EndpointResult> Logout(
        [FromBody] LogoutRequest request,
        [FromServices] LogoutUseCase useCase,
        CancellationToken cancellationToken) => await useCase.Handle(request, cancellationToken);
    
    [Authorize]
    [HttpGet("me")]
    public async Task<EndpointResult<AuthUserDto>> Me(
        [FromServices] GetCurrentUserUseCase useCase,
        CancellationToken ct) => await useCase.Handle(ct);
}