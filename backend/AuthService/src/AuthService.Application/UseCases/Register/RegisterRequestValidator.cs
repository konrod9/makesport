using AuthService.Domain.Users;
using FluentValidation;

namespace AuthService.Application.UseCases.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty()
            .EmailAddress()
            .MaximumLength(UserConstraints.EmailMaxLength);
        RuleFor(r => r.Password).NotEmpty()
            .MinimumLength(UserConstraints.PasswordMinLength)
            .MaximumLength(UserConstraints.PasswordMaxLength);
        
        RuleFor(r => r.FirstName).NotEmpty().MaximumLength(UserConstraints.NameMaxLength);
        RuleFor(r => r.LastName).NotEmpty().MaximumLength(UserConstraints.NameMaxLength);
    }
}