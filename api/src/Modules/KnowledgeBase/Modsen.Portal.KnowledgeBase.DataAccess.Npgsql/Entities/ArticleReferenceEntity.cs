using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class ArticleReferenceEntity
{
    public int Id { get; set; }

    public int ArticleId { get; set; }
    public ArticleEntity? Article { get; set; }

    public string? InternalFileId { get; set; }
    public string? ExternalUrl { get; set; }
}

internal class ArticleReferenceConfiguration : IEntityTypeConfiguration<ArticleReferenceEntity>
{
    public void Configure(EntityTypeBuilder<ArticleReferenceEntity> builder)
    {
        builder.ToTable("ArticleReferences");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.InternalFileId).IsRequired(false);

        builder.Property(x => x.ExternalUrl).IsRequired(false);

        builder.Property(x => x.ArticleId).IsRequired();

        builder.HasOne(x => x.Article)
            .WithMany(x => x.ArticleReferences)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
