using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class DiscountEntity : AuditableEntity
{
    public int Id { get; set; }

    public int DiscountCategoryId { get; set; }
    public DiscountCategoryEntity? DiscountCategory { get; set; }

    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageFileId { get; set; }
    public int DiscountPercent { get; set; }
    public int? DiscountPercentTo { get; set; }
    public string? CompanyType { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public required string Text { get; set; }
    public bool IsActive { get; set; }
    public int OrderWeight { get; set; }

    public KnowledgeBaseUserEntity? CreatedByUser { get; set; }
    public KnowledgeBaseUserEntity? UpdatedByUser { get; set; }

    public List<DiscountReferenceEntity> DiscountReferences { get; set; } = [];
    public List<DiscountTagEntity> DiscountTags { get; set; } = [];
    public List<DiscountCityEntity> DiscountCities { get; set; } = [];
}

internal class DiscountConfiguration : IEntityTypeConfiguration<DiscountEntity>
{
    public void Configure(EntityTypeBuilder<DiscountEntity> builder)
    {
        builder.ToTable("Discounts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.Property(x => x.ImageUrl).IsRequired(false);

        builder.Property(x => x.ImageFileId).IsRequired(false).HasMaxLength(128);

        builder.Property(x => x.DiscountPercent).IsRequired();

        builder.Property(x => x.DiscountPercentTo).IsRequired(false);

        builder.Property(x => x.CompanyType).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.Website).IsRequired(false);

        builder.Property(x => x.PhoneNumber).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.Location).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.Text).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.OrderWeight).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.DiscountCategoryId).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DiscountCategory)
            .WithMany(x => x.Discounts)
            .HasForeignKey(x => x.DiscountCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
