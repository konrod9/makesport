using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Auth.JwtValidation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VenuesService.Application.Common;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Enums;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.UseCases.UpdateReview;

public class UpdateVenueReviewUseCase
{
    private readonly IValidator<UpdateReviewRequest> _validator;
    private readonly ILogger<UpdateVenueReviewUseCase> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UpdateVenueReviewUseCase(
        IValidator<UpdateReviewRequest> validator,
        ILogger<UpdateVenueReviewUseCase> logger,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _validator = validator;
    }

    public async Task<Result<ReviewDto, Error>> Handle(
        Guid venueId,
        Guid reviewId,
        UpdateReviewRequest request,
        CancellationToken ct = default)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        var userId = _currentUserService.UserId;
        if (userId is null)
            return GeneralErrors.Unauthorized();

        var reviewIdVo = ReviewId.Create(reviewId);
        var venueIdVo = VenueId.Create(venueId);

        return await ConcurrencyRetry.ExecuteAsync<ReviewDto>(async token =>
        {
            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewIdVo, token);
            if (review == null)
                return ReviewErrors.NotFound();

            if (review.VenueId != venueIdVo)
                return ReviewErrors.NotFound();

            if (review.AuthorId != userId)
                return ReviewErrors.NotAuthor();

            var isPublished = review.Status == ReviewStatus.Published;
            Venue? venue = null;
            var (sum, count) = (0, 0);

            if (isPublished)
            {
                venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueIdVo, token);
                if (venue is null)
                    return ReviewErrors.VenueNotFound(venueId);

                (sum, count) = await ReviewStats
                    .GetPublishedAsync(_dbContext.Reviews.AsNoTracking(), venueIdVo, token, excludeId: reviewIdVo);
            }

            var updateResult = review.Update(request.Rating, request.Text);
            if (updateResult.IsFailure)
                return updateResult.Error;

            if (isPublished)
            {
                var applyResult = venue!.ApplyRating(
                    new VenueRatingStats((double)(sum + request.Rating) / (count + 1), count + 1));
                if (applyResult.IsFailure)
                    return applyResult.Error;
            }
            
            var saveResult = await _dbContext.TrySaveChangesAsync(token);
            if (!saveResult.IsSuccess)
                return saveResult.Error;

            _logger.LogInformation(
                "ReviewUpdated {ReviewId} for venue {VenueId} by {AuthorId}",
                review.Id, review.VenueId, review.AuthorId);


            return new ReviewDto
            {
                Id = review.Id,
                AuthorId = review.AuthorId,
                VenueId = review.VenueId,
                Rating = review.Rating,
                Text = review.Text,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                AuthorName = review.AuthorName,
                Status = review.Status
            };
        }, _dbContext, _logger, ct);
    }
}