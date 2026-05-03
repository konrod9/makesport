using MakeSport.Domain.Venues;
using MakeSport.Infrastructure.Postgres.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeSport.Infrastructure.Postgres.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");
        
        builder.HasKey(v => v.Id).HasName("pk_venues");
        
        builder.Property(v => v.Id)
            .HasColumnName("id");
        
        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.Property(v => v.Description)
            .HasMaxLength(500)
            .HasColumnName("description")
            .IsRequired(false);

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

        // TODO: Переделать на OwnsOne
        builder.Property(v => v.Location)
            //.HasConversion(new GeoCoordinateConverter())
            .HasColumnType("geography (Point,4326)")
            .HasColumnName("location");
    }
}