using AuthService.Application.Interfaces;
using AuthService.Application.Validation;
using AuthService.Domain.Users;
using CSharpFunctionalExtensions;
using FileService.Contracts.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.UseCases.ChangeUserRole;

public class ChangeUserRoleUseCase
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<ChangeUserRoleRequest> _validator;
    private readonly ILogger<ChangeUserRoleUseCase> _logger;

    public ChangeUserRoleUseCase(IIdentityService identityService, ICurrentUserService currentUserService,
        IValidator<ChangeUserRoleRequest> validator, ILogger<ChangeUserRoleUseCase> logger)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<AuthUserDto, Error>> Handle(ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        var currentUserId = _currentUserService.UserId;
        if (currentUserId is null)
            return AuthServiceErrors.Unauthorized();

        if (currentUserId == request.TargetUserId && request.NewRole != UserRole.Admin)
            return AuthServiceErrors.CannotDemoteSelf();

        var targetUser = await _identityService.GetByIdAsync(request.TargetUserId);
        if (targetUser == null)
            return AuthServiceErrors.TargetUserNotFound();

        var result = await _identityService.UpdateRoleAsync(targetUser, request.NewRole);
        if (result.IsFailure)
            return result.Error;

        _logger.LogInformation("Admin {CurrentUserId} changed {TargetUserId} role {Role}", currentUserId, targetUser.Id,
            request.NewRole);

        return new AuthUserDto(
            targetUser.Id,
            targetUser.Email ?? string.Empty,
            targetUser.FirstName,
            targetUser.LastName,
            targetUser.Role.ToString(),
            targetUser.UserName ?? string.Empty);
    }
}