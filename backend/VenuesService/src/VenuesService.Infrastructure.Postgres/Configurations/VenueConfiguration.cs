using VenuesService.Domain.Venues;
using VenuesService.Domain.Venues.ValueObjects;
using VenuesService.Infrastructure.Postgres.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Coordinates = VenuesService.Domain.Venues.ValueObjects.Coordinates;

namespace VenuesService.Infrastructure.Postgres.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        // TODO: Вынести в константы ограничения длины
        builder.ToTable("venues");

        builder.HasKey(v => v.Id).HasName("pk_venues");

        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => VenueId.Create(value));

        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.Property(v => v.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(v => v.IsOpen).HasColumnName("is_open");
        builder.Property(v => v.HasLighting).HasColumnName("has_lighting");
        builder.Property(v => v.IsFree).HasColumnName("is_free");

        builder.OwnsOne(v => v.WorkingHours, sa =>
        {
            sa.Property(w => w.WorkingStart).HasColumnName("working_start");

            sa.Property(w => w.WorkingEnd).HasColumnName("working_end");
        });

        builder.Property(v => v.Rating).HasColumnName("rating");
        builder.Property(v => v.ReviewCount).HasColumnName("review_count");

        builder.Property(v => v.SportType).HasColumnName("sport_type");
        builder.Property(v => v.Surface).HasColumnName("surface");

        builder.OwnsOne(v => v.Address, sa =>
        {
            sa.Property(a => a.Street).HasMaxLength(100)
                .HasColumnName("street");

            sa.Property(a => a.City).HasMaxLength(50)
                .HasColumnName("city");

            sa.Property(a => a.Building)
                .HasColumnName("building")
                .IsRequired(false);
        });

        builder.Property(v => v.Coordinates)
            .HasConversion(
                c => new Point(c.Latitude, c.Longitude),
                p => Coordinates.Create(p.X, p.Y).Value)
            .HasColumnType("geography (Point,4326)")
            .HasColumnName("coordinates");
    }
}