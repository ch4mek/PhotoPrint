using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class DiscountCategoryTagEntity
{
    public int Id { get; set; }

    public int DiscountCategoryId { get; set; }
    public DiscountCategoryEntity? DiscountCategory { get; set; }

    public int TagId { get; set; }
    public TagEntity? Tag { get; set; }
}

internal class DiscountCategoryTagConfiguration : IEntityTypeConfiguration<DiscountCategoryTagEntity>
{
    public void Configure(EntityTypeBuilder<DiscountCategoryTagEntity> builder)
    {
        builder.ToTable("DiscountCategoryTags");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DiscountCategoryId).IsRequired();
        builder.Property(x => x.TagId).IsRequired();

        builder.HasOne(x => x.DiscountCategory)
            .WithMany(x => x.DiscountCategoryTags)
            .HasForeignKey(x => x.DiscountCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.DiscountCategoryTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
