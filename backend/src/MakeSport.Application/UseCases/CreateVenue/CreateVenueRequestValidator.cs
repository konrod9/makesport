using FluentValidation;
using MakeSport.Application.Exceptions;
using MakeSport.Contracts.Requests;
using MakeSport.Domain.Venues.ValueObjects;

namespace MakeSport.Application.UseCases.CreateVenue;

public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
{
    public CreateVenueRequestValidator()
    {
        // TODO: Сделать Title и Description value object-ами и поменять валидацию на MustBeValueObject
        RuleFor(c => c.Title)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
            .MaximumLength(50).WithErrorCode(ValidationErrorCode.TooLong);
        
        RuleFor(c => c.Description)
            .MaximumLength(100).WithErrorCode(ValidationErrorCode.TooLong);

        RuleFor(v => v.Address)
            .MustBeValueObject(fm => Address.Create(fm.City, fm.Street, fm.Building));

        RuleFor(v => v.Coordinates)
            .MustBeValueObject(fm => Coordinates.Create(fm.Latitude, fm.Longitude));
    }
}