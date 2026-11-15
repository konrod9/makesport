using FluentValidation;
using VenuesService.Application.Exceptions;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.UseCases.CreateReview;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(r => r.Rating)
            .MustBeValueObject(ReviewRating.Create);

        RuleFor(r => r.Text)
            .Must(t => string.IsNullOrWhiteSpace(t) || t.Trim().Length <= ReviewsConstraints.TextMaxLength)
            .WithError(ReviewErrors.TextTooLong());
    }
}