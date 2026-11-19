using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Contracts.Responses.Reviews;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.UseCases.GetVenueReviews;

public class GetVenueReviewsUseCase
{
    private readonly IApplicationReadDbContext _readDbContext;
    private readonly IValidator<GetReviewsRequest> _validator;

    public GetVenueReviewsUseCase(
        IApplicationReadDbContext readDbContext, 
        IValidator<GetReviewsRequest> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
    }

    public async Task<Result<PaginationReviewsResponse, Error>> Handle(
        GetReviewsRequest request, 
        Guid venueId, 
        CancellationToken ct = default)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToError();
        
        var venueIdVo = VenueId.Create(venueId);
        if (!await _readDbContext.VenuesQuery.AnyAsync(v => v.Id == venueIdVo, cancellationToken: ct))
            return ReviewErrors.VenueNotFound(venueId);

        var query = _readDbContext.ReviewsQuery
            .Where(r => r.VenueId == venueIdVo && r.Status == ReviewStatus.Published);

        query = request.SortBy switch
        {
            ReviewSortField.Rating when request.SortDirection == SortDirection.Asc =>
                query.OrderBy(r => r.Rating).ThenBy(v => v.Id),

            ReviewSortField.Rating => query.OrderByDescending(r => r.Rating).ThenBy(v => v.Id),

            ReviewSortField.CreatedAt when request.SortDirection == SortDirection.Asc =>
                query.OrderBy(r => r.CreatedAt).ThenBy(v => v.Id),

            _ => query.OrderByDescending(r => r.CreatedAt).ThenBy(v => v.Id)
        };
        
        var reviewsCount = await query.CountAsync(ct);

        var reviews = await query
            .Select(r => new ReviewDto()
            {
                Id = r.Id,
                AuthorId = r.AuthorId,
                AuthorName = r.AuthorName,
                VenueId = r.VenueId,
                CreatedAt = r.CreatedAt,
                Rating = r.Rating,
                Text = r.Text,
                UpdatedAt = r.UpdatedAt,
                Status = r.Status
            })
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);
        
        var totalPages = (int)Math.Ceiling((double)reviewsCount / request.PageSize);
        
        return new PaginationReviewsResponse(reviews, reviewsCount, request.Page, request.PageSize, totalPages);
    }
}