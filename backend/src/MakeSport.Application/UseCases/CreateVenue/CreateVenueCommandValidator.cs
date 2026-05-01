using FluentValidation;
using MakeSport.Application.Exceptions;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
            .MaximumLength(50).WithErrorCode(ValidationErrorCode.TooLong);
        
        RuleFor(c => c.Description)
            .MaximumLength(100).WithErrorCode(ValidationErrorCode.TooLong);
    }
}