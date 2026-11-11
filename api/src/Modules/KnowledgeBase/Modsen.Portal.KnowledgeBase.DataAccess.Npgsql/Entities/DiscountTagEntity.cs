using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class DiscountTagEntity
{
    public int Id { get; set; }

    public int DiscountId { get; set; }
    public DiscountEntity? Discount { get; set; }

    public int TagId { get; set; }
    public TagEntity? Tag { get; set; }
}

internal class DiscountTagConfiguration : IEntityTypeConfiguration<DiscountTagEntity>
{
    public void Configure(EntityTypeBuilder<DiscountTagEntity> builder)
    {
        builder.ToTable("DiscountTags");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DiscountId).IsRequired();
        builder.Property(x => x.TagId).IsRequired();

        builder.HasOne(x => x.Discount)
            .WithMany(x => x.DiscountTags)
            .HasForeignKey(x => x.DiscountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.DiscountTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
