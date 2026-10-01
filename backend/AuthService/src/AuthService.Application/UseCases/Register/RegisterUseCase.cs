using AuthService.Application.Interfaces;
using AuthService.Application.Validation;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.Register;

public class RegisterUseCase
{
    private readonly ILogger<RegisterUseCase> _logger;
    private readonly IJwtService _jwtService;
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly IValidator<RegisterRequest> _validator;


    public RegisterUseCase(
        ILogger<RegisterUseCase> logger,
        IJwtService jwtService,
        IIdentityService identityService,
        IRefreshTokensRepository refreshTokensRepository, 
        IValidator<RegisterRequest> validator)
    {
        _logger = logger;
        _jwtService = jwtService;
        _identityService = identityService;
        _refreshTokensRepository = refreshTokensRepository;
        _validator = validator;
    }

    public async Task<Result<RegisterResponse, Error>> Handle(RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        AppUser? user = await _identityService.GetByEmailAsync(request.Email);
        if (user != null)
            return AuthServiceErrors.EmailAlreadyExist();

        user = new AppUser()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.FirstName + request.LastName,
            CreatedAt = DateTime.UtcNow
        };

        Result<AppUser, Error> result = await _identityService.CreateAsync(user, request.Password);
        if (result.IsFailure)
            return result.Error;

        (var isSuccess, var isFailure, var accessToken, Error? error) = _jwtService.GenerateAccessToken(user);
        if (isFailure)
            return error;

        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

        await _refreshTokensRepository.AddAsync(refreshToken, cancellationToken);
        var savingResult = await _refreshTokensRepository.SaveAsync(cancellationToken);
        if (savingResult.IsFailure)
            return savingResult.Error;

        _logger.LogInformation("User with id: {UserId} was registered", user.Id);

        return new RegisterResponse(
            new AuthUserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString(), user.UserName),
            refreshToken.Token,
            accessToken);
    }
}