using AuthService.Domain.Users;
using FluentValidation;

namespace AuthService.Application.UseCases.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty()
            .EmailAddress()
            .MaximumLength(UserConstraints.EmailMaxLength)
            .WithMessage("Email length must be less 50 characters");
        RuleFor(r => r.Password).NotEmpty()
            .MinimumLength(UserConstraints.PasswordMinLength)
            .MaximumLength(UserConstraints.PasswordMaxLength)
            .WithMessage("Password length must be between 8 to 100 characters");
        
        RuleFor(r => r.FirstName).NotEmpty().MaximumLength(UserConstraints.NameMaxLength);
        RuleFor(r => r.LastName).NotEmpty().MaximumLength(UserConstraints.NameMaxLength);
        
        RuleFor(r => r.UserName)
            .NotEmpty()
            .MaximumLength(UserConstraints.UserNameMaxLength)
            .Matches(UserConstraints.UserNamePattern)
            .WithMessage("Имя пользователя должно начинаться с английской буквы и может содержать только английские буквы и цифры.");;
    }
}