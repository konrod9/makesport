using FluentValidation;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.UseCases.GetVenueReviews;

public class GetReviewsRequestValidator : AbstractValidator<GetReviewsRequest>
{
    public GetReviewsRequestValidator()
    {
        RuleFor(x => x.Page)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("page"))
            .GreaterThan(0).WithError(GeneralErrors.ValueIsInvalid("page"));

        RuleFor(x => x.PageSize)
            .NotNull().WithError(GeneralErrors.ValueIsInvalid("page size"))
            .GreaterThan(0).WithError(GeneralErrors.ValueIsInvalid("page size"));
        
        RuleFor(x => x.SortBy)
            .IsInEnum().WithError(GeneralErrors.ValueIsInvalid("sortBy"));
        
        RuleFor(x => x.SortDirection)
            .IsInEnum().WithError(GeneralErrors.ValueIsInvalid("sortDirection"));
    }
}