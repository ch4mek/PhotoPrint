using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class DiscountReferenceEntity
{
    public int Id { get; set; }

    public int DiscountId { get; set; }
    public DiscountEntity? Discount { get; set; }

    public required string SocialMediaUrl { get; set; }
}

internal class DiscountReferenceConfiguration : IEntityTypeConfiguration<DiscountReferenceEntity>
{
    public void Configure(EntityTypeBuilder<DiscountReferenceEntity> builder)
    {
        builder.ToTable("DiscountReferences");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.SocialMediaUrl).IsRequired();

        builder.Property(x => x.DiscountId).IsRequired();

        builder.HasOne(x => x.Discount)
            .WithMany(x => x.DiscountReferences)
            .HasForeignKey(x => x.DiscountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
