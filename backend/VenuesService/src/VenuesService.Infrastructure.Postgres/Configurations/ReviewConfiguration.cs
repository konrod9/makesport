using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VenuesService.Contracts.Enums;
using VenuesService.Domain.Reviews;
using VenuesService.Domain.Reviews.ValueObjects;
using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.Infrastructure.Postgres.Persistence;

namespace VenuesService.Infrastructure.Postgres.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews", t =>
        {
            t.HasCheckConstraint(
                DatabaseConstraints.ReviewsRatingCheck, "rating BETWEEN 1 AND 5");
            t.HasCheckConstraint(
                DatabaseConstraints.ReviewsTextLengthCheck,
                $"text IS NULL OR char_length(text) <= {ReviewsConstraints.TextMaxLength}");
        });

        builder.HasKey(r => r.Id).HasName("pk_reviews");
        
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => ReviewId.Create(value))
            .HasColumnName("id");
        
        builder.Property(r => r.VenueId)
            .HasConversion(id => id.Value, value => VenueId.Create(value))
            .HasColumnName("venue_id")
            .IsRequired();

        builder.Property(r => r.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(r => r.AuthorName)
            .HasColumnName("author_name")
            .HasMaxLength(ReviewsConstraints.AuthorNameMaxLength)
            .IsRequired();
        
        builder.Property(r => r.Rating)
            .HasConversion(id => id.Value, value => ReviewRating.Create(value).Value)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(r => r.Text).HasColumnName("text").HasMaxLength(ReviewsConstraints.TextMaxLength);
        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(ReviewStatus.Published)
            .IsRequired();
        
        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();
        
        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone");
        
        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(r => r.VenueId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName(DatabaseConstraints.ReviewsVenueFk);
        
        builder.HasIndex(r => new { r.VenueId, r.AuthorId })
            .IsUnique()
            .HasDatabaseName(DatabaseConstraints.ReviewsUnique);

        builder.HasIndex(r => new { r.VenueId, r.Status, r.CreatedAt })
            .HasDatabaseName("ix_reviews_venue_id_status_created_at");

        builder.HasIndex(r => new { r.AuthorId, r.CreatedAt })
            .HasDatabaseName("ix_reviews_author_id_created_at");

        builder.HasIndex(r => new { r.VenueId, r.Rating })
            .HasDatabaseName("ix_reviews_venue_id_rating");
    }
}
