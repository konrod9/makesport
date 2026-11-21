using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Auth.JwtValidation;
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
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.UseCases.ChangeReviewStatus;

public class ChangeReviewStatusUseCase
{
    private readonly IValidator<ChangeReviewStatusRequest> _validator;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ChangeReviewStatusUseCase> _logger;

    public ChangeReviewStatusUseCase(
        IValidator<ChangeReviewStatusRequest> validator, 
        IApplicationDbContext dbContext, 
        ICurrentUserService currentUserService,
        ILogger<ChangeReviewStatusUseCase> logger)
    {
        _validator = validator;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<ReviewDto, Error>> Handle(Guid reviewId, ChangeReviewStatusRequest request,
        CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToError();
        
        var userId = _currentUserService.UserId;
        if (userId is null
            || (!_currentUserService.IsInRole(AuthRoles.Moderator) && !_currentUserService.IsInRole(AuthRoles.Admin)))
            return Error.Authorization("auth.forbidden", "Недостаточно прав для изменения статуса отзыва");
        
        var reviewIdVo = ReviewId.Create(reviewId);

        return await ConcurrencyRetry.ExecuteAsync<ReviewDto>(async token =>
        {
            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewIdVo, token);
            if (review is null)
                return ReviewErrors.NotFound();
        
            if (review.Status == request.Status)
                return new ReviewDto
                {
                    Id = review.Id,
                    AuthorId = review.AuthorId,
                    AuthorName = review.AuthorName,
                    CreatedAt = review.CreatedAt,
                    Rating = review.Rating,
                    Text = review.Text,
                    VenueId = review.VenueId,
                    Status = review.Status,
                    UpdatedAt = review.UpdatedAt
                };
            
            var oldStatus = review.Status;
            
            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == review.VenueId, token);
            if (venue is null)
                return ReviewErrors.VenueNotFound(review.VenueId.Value);
            
            var (sum, count) = await ReviewStats.GetPublishedAsync(
                _dbContext.Reviews.AsNoTracking(), review.VenueId, token, excludeId: reviewIdVo);
            
            switch (request.Status)
            {
                case ReviewStatus.Hidden:
                    review.Hide();
                    var hideResult = venue.ApplyRating(count == 0
                        ? VenueRatingStats.Empty
                        : new VenueRatingStats((double)sum / count, count));
                    if (hideResult.IsFailure)
                        return hideResult.Error;
                    break;
                
                case ReviewStatus.Published:
                    review.Publish();
                    var publishResult = venue.ApplyRating(
                        new VenueRatingStats((double)(sum + review.Rating.Value) / (count + 1), count + 1));
                    if (publishResult.IsFailure)
                        return publishResult.Error;
                    break;
                
                default:
                    return GeneralErrors.ValueIsInvalid(nameof(request.Status), "Некорректный статус");
            }

            var saveResult = await _dbContext.TrySaveChangesAsync(token);
            if (saveResult.IsFailure)
                return saveResult.Error;
            
            _logger.LogInformation(
                "ReviewStatusChanged {ReviewId} {VenueId} {OldStatus}->{NewStatus} by {ModeratorId}",
                review.Id.Value, review.VenueId.Value, oldStatus, request.Status, userId);
            
            return new ReviewDto
            {
                Id = review.Id,
                AuthorId = review.AuthorId,
                AuthorName = review.AuthorName,
                CreatedAt = review.CreatedAt,
                Rating = review.Rating,
                Text = review.Text,
                VenueId = review.VenueId,
                Status = review.Status,
                UpdatedAt = review.UpdatedAt,
            };

        }, _dbContext, _logger, ct);
    }
}