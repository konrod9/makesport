using AuthService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        
        builder.HasKey(r => r.Id).HasName("pk_refresh_tokens");
        
        builder.Property(r => r.Id).HasColumnName("id");
        
        builder.Property(r => r.UserId).HasColumnName("user_id");
        
        builder.Property(r => r.Token).HasColumnName("token");
        
        builder.Property(r => r.IsRevoked).HasColumnName("is_revoked");
        
        builder.Property(r => r.ExpiresAt).HasColumnName("expires_at");
        
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        
        builder.HasIndex(r => r.Token).IsUnique().HasDatabaseName("ux_refresh_tokens_token");
        
        builder.HasIndex(r => r.UserId).HasDatabaseName("ix_refresh_tokens_user_id");
    }
}