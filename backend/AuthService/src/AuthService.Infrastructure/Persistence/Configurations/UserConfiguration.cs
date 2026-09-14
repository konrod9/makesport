using AuthService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasColumnType("text")
            .HasConversion<string>();

        builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(50);

        builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(50);

        builder.Property(u => u.AvatarId).HasColumnName("avatar_id");

        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
    }
}