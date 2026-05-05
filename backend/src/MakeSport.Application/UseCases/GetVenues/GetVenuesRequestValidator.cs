using FluentValidation;
using MakeSport.Application.Validation;
using MakeSport.Contracts.Requests;
using MakeSport.Domain.Shared;

namespace MakeSport.Application.UseCases.GetVenues;

public class GetVenuesRequestValidator : AbstractValidator<GetVenuesRequest>
{
    public GetVenuesRequestValidator()
    {
        RuleFor(x => x.Search).MaximumLength(1000).WithError(GeneralErrors.ValueIsInvalid("search"));

        RuleFor(x => x.Page)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("page"))
            .GreaterThan(0).WithError(GeneralErrors.ValueIsInvalid("page"));

        RuleFor(x => x.PageSize)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("page"))
            .GreaterThan(0).WithError(GeneralErrors.ValueIsInvalid("page size"));
    }
}