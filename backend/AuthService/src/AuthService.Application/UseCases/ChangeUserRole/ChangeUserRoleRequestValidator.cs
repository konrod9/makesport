using FluentValidation;

namespace AuthService.Application.UseCases.ChangeUserRole;

public class ChangeUserRoleRequestValidator : AbstractValidator<ChangeUserRoleRequest>
{
    public ChangeUserRoleRequestValidator()
    {
        RuleFor(request => request.TargetUserId).NotEmpty();
        RuleFor(request => request.NewRole).IsInEnum();
    }
}