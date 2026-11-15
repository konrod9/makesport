using CSharpFunctionalExtensions;
using FluentValidation;
using MakeSport.Auth.JwtValidation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VenuesService.Application.Common;
using VenuesService.Application.Validation;
using VenuesService.Contracts.Dtos.Reviews;
using VenuesService.Contracts.Requests.Reviews;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Application.UseCases.CreateReview;

public class CreateReviewUseCase
{
    private readonly IValidator<CreateReviewRequest> _validator;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<CreateReviewUseCase> _logger;
    private readonly ICurrentUserService _currentUserService;

    public CreateReviewUseCase(IValidator<CreateReviewRequest> validator, IApplicationDbContext dbContext,
        ILogger<CreateReviewUseCase> logger, ICurrentUserService currentUserService)
    {
        _validator = validator;
        _dbContext = dbContext;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ReviewDto, Error>> Handle(CreateReviewRequest request, Guid venueId, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToError();

        var authorId = _currentUserService.UserId;
        if (authorId is null)
            return GeneralErrors.Unauthorized();

        var authorName = _currentUserService.DisplayName ?? "Пользователь";

        return await ConcurrencyRetry.ExecuteAsync<ReviewDto>(async ct =>
        {
            var venueIdVo = VenueId.Create(venueId);
            var venue = await _dbContext.Venues.FirstOrDefaultAsync(v => v.Id == venueIdVo, ct);
            if (venue == null)
                return ReviewErrors.VenueNotFound(venueId);
            
            var (sum, count) = await ReviewStats
                .GetPublishedAsync(_dbContext.Reviews, venue.Id, ct);

            if (await _dbContext.Reviews.AnyAsync(r => r.VenueId == venueIdVo && r.AuthorId == authorId, ct))
                return ReviewErrors.AlreadyExists();
            
            var reviewId = ReviewId.NewId();
            
            var reviewResult = Review.Create(reviewId, venueId, (Guid)authorId, authorName, request.Rating, request.Text);
            if (reviewResult.IsFailure)
                return reviewResult.Error;
            
            var review = reviewResult.Value;
            _dbContext.Reviews.Add(review);

            var applyRatingResult = venue.ApplyRating(new VenueRatingStats(
                (double)(sum + request.Rating) / (count + 1), count + 1));
            if (applyRatingResult.IsFailure)
                return applyRatingResult.Error;

            var saveChangesResult = await _dbContext.TrySaveChangesAsync(ct);
            if (saveChangesResult.IsFailure)
                return saveChangesResult.Error;

            _logger.LogInformation("ReviewCreated with id: {ReviewId} for venue: {VenueId} by user: {AuthorId}",
                review.Id, review.VenueId, review.AuthorId);
            
            return new ReviewDto()
            {
                Id = review.Id,
                AuthorId = review.AuthorId,
                AuthorName = review.AuthorName,
                CreatedAt = review.CreatedAt,
                Rating = review.Rating,
                Text = review.Text,
                VenueId = review.VenueId,
                Status = review.Status,
            };
        }, _dbContext, _logger, cancellationToken);
    }
}