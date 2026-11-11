using CSharpFunctionalExtensions;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Shared;
using VenuesService.Domain.Venues.ValueObjects;

namespace VenuesService.Domain.Reviews;

public class Review
{
    // EF Core
    private Review()
    {
    }

    private Review(
        ReviewId id, 
        string? text,
        VenueId venueId,
        Guid authorId,
        string authorName,
        ReviewRating reviewRating,
        ReviewStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        Text = text;
        VenueId = venueId;
        AuthorId = authorId;
        AuthorName = authorName;
        Rating = reviewRating;
        Status = status;
        CreatedAt = createdAt;
    }

    public ReviewId Id { get; private set; }

    public string? Text { get; private set; }

    public VenueId VenueId { get; private set; }

    public Guid AuthorId { get; private set; }

    public string AuthorName { get; private set; }

    public ReviewRating Rating { get; private set; }
    
    public ReviewStatus Status { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Result<Review, Error> Create(
        ReviewId id,
        VenueId venueId,
        Guid authorId,
        string authorName,
        int rating,
        string? text)
    {

        text = text?.Trim();
        if (string.IsNullOrEmpty(text)) 
            text = null;
        else if (text.Length > ReviewsConstraints.TextMaxLength) 
            return ReviewErrors.TextTooLong();

        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return ratingResult.Error;
        
        if (string.IsNullOrWhiteSpace(authorName))
            authorName = "Пользователь";
        authorName = authorName.Trim();
        if (authorName.Length > ReviewsConstraints.AuthorNameMaxLength)
            authorName = authorName[..ReviewsConstraints.AuthorNameMaxLength];
        
        var createdAt = DateTimeOffset.UtcNow;
        
        return new Review(
            id,
            text,
            venueId,
            authorId, 
            authorName, 
            ratingResult.Value, 
            ReviewStatus.Published,
            createdAt);
    }

    public UnitResult<Error> Update(int rating, string? text)
    {
        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return ratingResult.Error;
        
        text = text?.Trim();
        if (string.IsNullOrEmpty(text)) text = null;
        else if (text.Length > ReviewsConstraints.TextMaxLength) return ReviewErrors.TextTooLong();
        
        Rating = ratingResult.Value;
        UpdatedAt = DateTimeOffset.UtcNow;
        Text = text;

        return Result.Success<Error>();
    }

    public void Hide()
    {
        Status = ReviewStatus.Hidden;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Publish()
    {
        Status = ReviewStatus.Published;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}