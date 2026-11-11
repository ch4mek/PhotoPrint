using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class TagEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<DiscountCategoryTagEntity> DiscountCategoryTags { get; set; } = [];
    public List<DiscountTagEntity> DiscountTags { get; set; } = [];
    public List<ArticleTagEntity> ArticleTags { get; set; } = [];
}

internal class TagConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
