using AuthService.Application.Interfaces;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.GetCurrentUser;

public class GetCurrentUserUseCase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<GetCurrentUserUseCase> _logger;

    public GetCurrentUserUseCase(ICurrentUserService currentUserService, IIdentityService identityService,
        ILogger<GetCurrentUserUseCase> logger)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result<AuthUserDto, Error>> Handle(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
            return Error.Authentication("auth.unauthorized", "Пользователь не авторизован");
        
        var user = await _identityService.GetByIdAsync(userId.Value);
        if (user == null)
            return Error.Authentication("auth.unauthorized", "Пользователь не найден");
        
        _logger.LogInformation("Get current user {UserId}.", user.Id);
        
        return new AuthUserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            user.UserName ?? string.Empty);
    }
}