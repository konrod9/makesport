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
        
        builder.HasKey(v => v.VenueId).HasName("pk_venues");
        
        builder.Property(v => v.VenueId)
            .HasColumnName("id");
        
        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");
        
        builder.Property(v => v.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(v => v.Location)
            .HasConversion(new GeoCoordinateConverter())
            .HasColumnType("geography (Point,4326)")
            .HasColumnName("location");
    }
}