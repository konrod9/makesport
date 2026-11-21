using FluentValidation;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.UseCases.ChangeReviewStatus;

public class ChangeReviewStatusRequestValidator : AbstractValidator<ChangeReviewStatusRequest>
{
    public ChangeReviewStatusRequestValidator()
    {
        RuleFor(r => r.Status)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("status"))
            .IsInEnum().WithError(GeneralErrors.ValueIsInvalid("status"));
    }
}