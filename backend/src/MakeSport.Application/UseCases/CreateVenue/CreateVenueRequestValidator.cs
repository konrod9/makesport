using FluentValidation;
using MakeSport.Application.Exceptions;
using MakeSport.Contracts.Requests;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
{
    public CreateVenueRequestValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
            .MaximumLength(50).WithErrorCode(ValidationErrorCode.TooLong);
        
        RuleFor(c => c.Description)
            .MaximumLength(100).WithErrorCode(ValidationErrorCode.TooLong);
    }
}