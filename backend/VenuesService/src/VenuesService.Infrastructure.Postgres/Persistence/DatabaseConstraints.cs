namespace VenuesService.Infrastructure.Postgres.Persistence;

public static class DatabaseConstraints
{
    public const string ReviewsUnique          = "ux_reviews_venue_id_author_id";
    
    public const string ReviewsVenueFk         = "fk_reviews_venues_venue_id";
    
    public const string ReviewsRatingCheck     = "ck_reviews_rating";
    
    public const string ReviewsTextLengthCheck = "ck_reviews_text_length";
}