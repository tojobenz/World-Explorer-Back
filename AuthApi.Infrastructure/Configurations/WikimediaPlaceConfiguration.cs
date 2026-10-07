using AuthApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastructure.Configurations;

/// <summary>
/// Configuration EF Core for entity wikimediaPlace
/// </summary>
public class WikimediaPlaceConfiguration : IEntityTypeConfiguration<WikimediaPlace>
{
    public void Configure(EntityTypeBuilder<WikimediaPlace> builder)
    {
        builder.ToTable("WikimediaPlaces");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PageId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.PageId)
            .IsUnique();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Extract)
            .HasMaxLength(5000);

        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.FullUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Type)
            .HasMaxLength(50);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Country);
        builder.HasIndex(x => x.City);
        builder.HasIndex(x => x.Type);
    }
}