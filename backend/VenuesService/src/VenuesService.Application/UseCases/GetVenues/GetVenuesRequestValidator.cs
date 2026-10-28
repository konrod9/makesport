using FluentValidation;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests;
using VenuesService.Domain.Shared;

namespace VenuesService.Application.UseCases.GetVenues;

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
        
        RuleFor(x => x.SortBy)
            .IsInEnum().WithError(GeneralErrors.ValueIsInvalid("sortBy"));
        
        RuleFor(x => x.SortDirection)
            .IsInEnum().WithError(GeneralErrors.ValueIsInvalid("sortDirection"));

        When(x => x.SortBy == VenueSortField.Distance, () =>
        {
            RuleFor(x => x.Latitude)
                .NotNull()
                .WithError(GeneralErrors.ValueIsInvalid("latitude"))
                .Must(lat => lat.HasValue && lat.Value >= -90 && lat.Value <= 90)
                .WithError(GeneralErrors.ValueIsInvalid("latitude"));

            RuleFor(x => x.Longitude)
                .NotNull()
                .WithError(GeneralErrors.ValueIsInvalid("longitude"))
                .Must(lon => lon.HasValue && lon.Value >= -180 && lon.Value <= 180)
                .WithError(GeneralErrors.ValueIsInvalid("longitude"));
        });
    }
}