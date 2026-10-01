using AuthService.Application.Interfaces;
using AuthService.Application.Validation;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.Logout;

public class LogoutUseCase
{
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly ILogger<LogoutUseCase> _logger;
    private readonly IValidator<LogoutRequest> _validator;

    public LogoutUseCase(
        IRefreshTokensRepository refreshTokensRepository,
        ILogger<LogoutUseCase> logger, 
        IValidator<LogoutRequest> validator)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<UnitResult<Error>> Handle(LogoutRequest logoutRequest,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(logoutRequest, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();
        
        var refreshTokenResult = await _refreshTokensRepository.GetByAsync(t =>
            t.Token == logoutRequest.RefreshToken, cancellationToken);
        if (refreshTokenResult.Value == null)
            return UnitResult.Success<Error>();
        
        var refreshToken = refreshTokenResult.Value;
        refreshToken.Revoke();
        
        var result = await _refreshTokensRepository.SaveAsync(cancellationToken);
        if (result.IsFailure)
            return result.Error;
        
        _logger.LogInformation("Logout successful for user: {UserId}", refreshToken.UserId);
        return UnitResult.Success<Error>();
    }
}