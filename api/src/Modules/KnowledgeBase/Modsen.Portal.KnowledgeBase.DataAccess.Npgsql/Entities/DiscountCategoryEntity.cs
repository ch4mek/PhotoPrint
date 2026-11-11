using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class DiscountCategoryEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OrderWeight { get; set; }

    public KnowledgeBaseUserEntity? CreatedByUser { get; set; }
    public KnowledgeBaseUserEntity? UpdatedByUser { get; set; }

    public List<DiscountEntity> Discounts { get; set; } = [];
    public List<DiscountCategoryTagEntity> DiscountCategoryTags { get; set; } = [];
}

internal class DiscountCategoryConfiguration : IEntityTypeConfiguration<DiscountCategoryEntity>
{
    public void Configure(EntityTypeBuilder<DiscountCategoryEntity> builder)
    {
        builder.ToTable("DiscountCategories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.Property(x => x.OrderWeight).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
              .WithMany()
              .HasForeignKey(x => x.CreatedById)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Discounts)
            .WithOne(x => x.DiscountCategory)
            .HasForeignKey(x => x.DiscountCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
