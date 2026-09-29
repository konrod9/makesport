using AuthService.Application.Interfaces;
using AuthService.Application.Validation;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.Login;

public class LoginUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly ILogger<LoginUseCase> _logger;
    private readonly IValidator<LoginRequest> _validator;

    public LoginUseCase(
        IIdentityService identityService,
        IJwtService jwtService,
        IRefreshTokensRepository refreshTokensRepository, 
        ILogger<LoginUseCase> logger,
        IValidator<LoginRequest> validator)
    {
        _identityService = identityService;
        _jwtService = jwtService;
        _refreshTokensRepository = refreshTokensRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<LoginResponse, Error>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();
        
        AppUser? user = await _identityService.GetByEmailAsync(request.Email);
        if (user == null)
            return Error.Authentication("auth.failed", "Неверный Email или пароль");

        if (await _identityService.IsLockedOutAsync(user))
        {
            DateTimeOffset? endDate = await _identityService.GetLockoutEndDateAsync(user);
            return AuthServiceErrors.UserLockedOut(endDate);
        }

        if (!await _identityService.CheckPasswordAsync(user, request.Password))
            return Error.Authentication("auth.failed", "Неверный Email или пароль");
        
        var accessTokenResult = _jwtService.GenerateAccessToken(user);
        if (accessTokenResult.IsFailure)
            return accessTokenResult.Error;
        
        var refreshTokenResult = _jwtService.GenerateRefreshToken(user.Id);
        if (refreshTokenResult.IsFailure)
            return refreshTokenResult.Error;
        
        var result = await _refreshTokensRepository.AddAsync(refreshTokenResult.Value, cancellationToken);
        if (result.IsFailure)
            return result.Error;
        
        _logger.LogInformation("User {UserId} logged in.", user.Id);
        
        return new LoginResponse(accessTokenResult.Value, refreshTokenResult.Value.Token,
            new AuthUserDto(user.Id, user.Email ?? string.Empty, user.FirstName, user.LastName, user.Role.ToString(), user.UserName ?? string.Empty));
    }
}