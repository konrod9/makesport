using AuthService.Application.Interfaces;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.Register;

public class RegisterUseCase
{
    private readonly ILogger<RegisterUseCase> _logger;
    private readonly IJwtService _jwtService;
    private readonly IUsersRepository _usersRepository;
    private IRefreshTokensRepository _refreshTokensRepository;

    public RegisterUseCase(
        ILogger<RegisterUseCase> logger, 
        IJwtService jwtService, 
        IUsersRepository usersRepository, 
        IRefreshTokensRepository refreshTokensRepository)
    {
        _logger = logger;
        _jwtService = jwtService;
        _usersRepository = usersRepository;
        _refreshTokensRepository = refreshTokensRepository;
    }

    public async Task<Result<RegisterResult, Error>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        //TODO: Валидация входных данных RegisterRequest

        AppUser? user = await _usersRepository.GetByEmailAsync(request.Email);
        if (user != null)
            return AuthServiceErrors.EmailAlreadyExist();

        user = new AppUser()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.FirstName + request.LastName
        };
        
        Result<AppUser, Error> result = await _usersRepository.CreateAsync(user, request.Password);
        if (result.IsFailure)
            return result.Error;
        
        (var isSuccess, var isFailure, var accessToken, Error? error) = _jwtService.GenerateAccessToken(user);
        if (isFailure)
            return error;
        
        (_, var isRefreshFailure, RefreshToken? refreshToken, Error? refreshTokenError) = 
            _jwtService.GenerateRefreshToken(user.Id);
        if (isRefreshFailure)
            return refreshTokenError;

        await _refreshTokensRepository.AddAsync(refreshToken, cancellationToken);

        return new RegisterResult(
            new AuthUserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString(), user.UserName),
            refreshToken.Token,
            accessToken);
    }
}