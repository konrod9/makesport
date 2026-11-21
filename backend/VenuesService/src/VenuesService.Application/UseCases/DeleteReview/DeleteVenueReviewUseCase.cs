using CSharpFunctionalExtensions;
using MakeSport.Auth.JwtValidation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VenuesService.Application.Common;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.UseCases.DeleteReview;

public class DeleteVenueReviewUseCase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteVenueReviewUseCase> _logger;

    public DeleteVenueReviewUseCase(
        IApplicationDbContext dbContext, 
        ICurrentUserService currentUserService, 
        ILogger<DeleteVenueReviewUseCase> logger)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<UnitResult<Error>> Handle(Guid venueId, Guid reviewId, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return GeneralErrors.Unauthorized();
        
        var reviewIdVo = ReviewId.Create(reviewId);
        var venueIdVo = VenueId.Create(venueId);

        return await ConcurrencyRetry.ExecuteAsync<UnitResult<Error>>(async token =>
        {
            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewIdVo, token);
            if (review is null || review.VenueId != venueIdVo)
                return ReviewErrors.NotFound();

            if (review.AuthorId != userId)
                return ReviewErrors.NotAuthor();

            if (review.Status == ReviewStatus.Published)
            {
                var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueIdVo, token);
                if (venue is null)
                    return ReviewErrors.NotFound();

                var (sum, count) = await ReviewStats
                    .GetPublishedAsync(_dbContext.Reviews.AsNoTracking(), venueIdVo, token, reviewIdVo);
                
                var applyResult = venue.ApplyRating(count == 0
                    ? VenueRatingStats.Empty 
                    : new VenueRatingStats((double)sum / count, count));
                if (applyResult.IsFailure)
                    return applyResult.Error;
            }
            
            _dbContext.Reviews.Remove(review);

            var saveResult = await _dbContext.TrySaveChangesAsync(token);
            if (saveResult.IsFailure)
                return saveResult.Error;
            
            _logger.LogInformation(
                "ReviewDeleted {ReviewId} for venue {VenueId} by {AuthorId}",
                review.Id, review.VenueId, review.AuthorId);

            return UnitResult.Success<Error>();

        }, _dbContext, _logger, ct);
    }
}