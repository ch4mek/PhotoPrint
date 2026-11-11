using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal class ItemEntity : AuditableEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public bool IsClothing { get; set; }

    public string? Supplier { get; set; }

    public MerchItemStatus Status { get; set; }

    public string? Description { get; set; }

    public string? PhotoFileId { get; set; }

    public MerchUserEntity? CreatedByUser { get; set; }

    public MerchUserEntity? UpdatedByUser { get; set; }

    public int? ArchivedById { get; set; }

    public DateTime? ArchivedAtUtc { get; set; }

    public MerchUserEntity? ArchivedByUser { get; set; }

    public List<KitItemEntity> KitItems { get; set; } = [];

    public List<ItemTransactionEntity> ItemTransactions { get; set; } = [];

    public List<ItemSizeMinAmountEntity> ItemSizeMinAmounts { get; set; } = [];
}

internal class ItemConfiguration : IEntityTypeConfiguration<ItemEntity>
{
    public void Configure(EntityTypeBuilder<ItemEntity> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id)
            .ValueGeneratedOnAdd();

        builder.Property(item => item.IsClothing)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(item => item.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(item => item.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(item => item.Description)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(item => item.Supplier)
            .IsRequired(false)
            .HasMaxLength(256);

        builder.Property(item => item.PhotoFileId)
           .IsRequired(false)
           .HasMaxLength(32);

        builder.Property(item => item.CreatedAtUtc)
            .IsRequired();

        builder.Property(item => item.UpdatedAtUtc)
            .IsRequired(false);

        builder.Property(item => item.ArchivedAtUtc)
            .IsRequired(false);

        builder.Property(x => x.CreatedById).IsRequired();
        builder.HasOne(item => item.CreatedByUser)
            .WithMany()
            .HasForeignKey(item => item.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.UpdatedByUser)
            .WithMany()
            .HasForeignKey(item => item.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.ArchivedByUser)
            .WithMany()
            .HasForeignKey(item => item.ArchivedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
