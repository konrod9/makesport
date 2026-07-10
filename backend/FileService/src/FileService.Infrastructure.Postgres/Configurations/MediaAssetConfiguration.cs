using System.Text.Json;
using FileService.Domain;
using FileService.Domain.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Postgres.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_assets");
        builder.HasKey(x => x.Id);

        builder.HasDiscriminator<string>("asset_type")
            .HasValue<VideoAsset>("video")
            .HasValue<ImageAsset>("image");

        builder.OwnsOne(m => m.MediaData, mb =>
        {
            mb.ToJson("media_data");

            mb.OwnsOne(md => md.ContentType, cb =>
            {
                cb.Property(x => x.Category)
                    .HasConversion<string>()
                    .HasJsonPropertyName("category");

                cb.Property(x => x.Value)
                    .HasJsonPropertyName("value");
            });

            mb.OwnsOne(md => md.FileName, fb =>
            {
                fb.Property(x => x.Extension)
                    .HasJsonPropertyName("extension");
                fb.Property(x => x.Name)
                    .HasJsonPropertyName("name");
                fb.Property(x => x.Value)
                    .HasJsonPropertyName("value");
            });

            mb.Property(md => md.Size)
                .HasJsonPropertyName("size");
            mb.Property(md => md.ExpectedChunksCount)
                .HasJsonPropertyName("expected_chunks_count");
        });

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Status).HasConversion<string>();

        builder.Property(x => x.AssetType).HasConversion<string>();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.Property(x => x.Key)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<StorageKey>(v, (JsonSerializerOptions?)null)!)
            .HasColumnName("key")
            .HasColumnType("jsonb");

        builder.HasIndex(x => new
        {
            x.Status,
            x.CreatedAt,
        });
    }
}
