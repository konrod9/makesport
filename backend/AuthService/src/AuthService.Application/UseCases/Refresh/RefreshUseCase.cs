using AuthService.Application.Interfaces;
using AuthService.Application.Validation;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.Refresh;

public class RefreshUseCase
{
    private readonly IValidator<RefreshRequest> _validator;
    private readonly IRefreshTokensRepository _tokensRepository;
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RefreshUseCase> _logger;

    public RefreshUseCase(
        IValidator<RefreshRequest> validator,
        IRefreshTokensRepository tokensRepository, 
        IIdentityService identityService, 
        IJwtService jwtService, 
        ILogger<RefreshUseCase> logger)
    {
        _validator = validator;
        _tokensRepository = tokensRepository;
        _identityService = identityService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<Result<RefreshResponse, Error>> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();
        
        var refreshTokenResult = await _tokensRepository.GetByAsync(t => t.Token == request.RefreshToken, cancellationToken);
        if (refreshTokenResult.IsFailure)
            return refreshTokenResult.Error;

        var refreshToken = refreshTokenResult.Value;
        if (refreshToken == null || refreshToken.IsExpired || refreshToken.IsRevoked)
            return AuthServiceErrors.InvalidToken();
        
        var user = await _identityService.GetByIdAsync(refreshToken.UserId);
        if (user == null)
            return AuthServiceErrors.InvalidToken();
        
        refreshToken.Revoke();

        var accessTokenResult = _jwtService.GenerateAccessToken(user);
        if (accessTokenResult.IsFailure)
            return accessTokenResult.Error;
        
        var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);
        
        await _tokensRepository.AddAsync(newRefreshToken, cancellationToken);
        var result = await _tokensRepository.SaveAsync(cancellationToken);
        if (result.IsFailure)
            return result.Error;

        _logger.LogInformation("Refresh access token saved to database for user with ID: {UserId}", user.Id);
        
        return new RefreshResponse(
            accessTokenResult.Value, 
            newRefreshToken.Token, 
            new AuthUserDto(
                user.Id,
                user.Email ?? string.Empty, 
                user.FirstName, 
                user.LastName, 
                user.Role.ToString(), 
                user.UserName ?? string.Empty));
    }
}