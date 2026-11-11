using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class ArticleTagEntity
{
    public int Id { get; set; }

    public int ArticleId { get; set; }
    public ArticleEntity? Article { get; set; }

    public int TagId { get; set; }
    public TagEntity? Tag { get; set; }
}

internal class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTagEntity>
{
    public void Configure(EntityTypeBuilder<ArticleTagEntity> builder)
    {
        builder.ToTable("ArticleTags");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ArticleId).IsRequired();
        builder.Property(x => x.TagId).IsRequired();

        builder.HasOne(x => x.Article)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
