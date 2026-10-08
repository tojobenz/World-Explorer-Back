using AuthApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastructure.Configurations;

/// <summary>
/// Configuration EF Core for entity WikimediaFact
/// </summary>
public class WikimediaFactConfiguration : IEntityTypeConfiguration<WikimediaFact>
{
    public void Configure(EntityTypeBuilder<WikimediaFact> builder)
    {
        builder.ToTable("WikimediaFacts");

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

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.Category)
            .HasMaxLength(100);

        builder.Property(x => x.RelatedPeople)
            .HasMaxLength(2000);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.EventDate);
    }
}