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