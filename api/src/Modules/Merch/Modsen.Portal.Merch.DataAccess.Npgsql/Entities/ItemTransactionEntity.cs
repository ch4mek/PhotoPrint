using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class ItemTransactionEntity : IPortalEntity
{
    public int Id { get; set; }

    public MerchTransactionType Type { get; set; }

    public int OrderWeight { get; set; }
    public Guid GroupCode { get; set; }

    public ItemTransactionGroupEntity? Group { get; set; }

    public int ItemId { get; set; }

    public ItemEntity? Item { get; set; }

    public required string SizeId { get; set; }
    public MerchSizeEntity? Size { get; set; }

    public int WarehouseId { get; set; }
    public WarehouseEntity? Warehouse { get; set; }

    public int? SenderId { get; set; }
    public WarehouseEntity? Sender { get; set; }

    public int? UserRecipientId { get; set; }
    public MerchUserEntity? UserRecipient { get; set; }

    public int? WarehouseRecipientId { get; set; }
    public WarehouseEntity? WarehouseRecipient { get; set; }

    public int Count { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public MerchUserEntity? CreatedByUser { get; set; }
}

internal sealed class ItemTransactionConfiguration : IEntityTypeConfiguration<ItemTransactionEntity>
{
    public void Configure(EntityTypeBuilder<ItemTransactionEntity> builder)
    {
        builder.ToTable("ItemTransactions");

        builder.HasKey(itemTransaction => itemTransaction.Id);
        builder.Property(itemTransaction => itemTransaction.Id)
            .ValueGeneratedOnAdd();

        builder.Property(itemTransaction => itemTransaction.OrderWeight)
            .IsRequired()
            .HasReverseOrderEnumComputedColumn<ItemTransactionEntity, MerchTransactionType>(c => c.Type);

        builder.Property(itemTransaction => itemTransaction.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(itemTransaction => itemTransaction.Group)
            .WithMany(group => group.ItemTransactions)
            .HasForeignKey(itemTransaction => itemTransaction.GroupCode)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(itemTransaction => itemTransaction.Item)
            .WithMany(item => item.ItemTransactions)
            .HasForeignKey(itemTransaction => itemTransaction.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(itemTransaction => itemTransaction.Size)
            .WithMany(size => size.ItemTransactions)
            .HasForeignKey(itemTransaction => itemTransaction.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(itemTransaction => itemTransaction.Sender)
            .WithMany(warehouse => warehouse.SenderItemTransactions)
            .HasForeignKey(itemTransaction => itemTransaction.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(itemTransaction => itemTransaction.Warehouse)
            .WithMany(warehouse => warehouse.ItemTransactions)
            .HasForeignKey(itemTransaction => itemTransaction.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(itemTransaction => itemTransaction.Count)
            .IsRequired();

        builder.Property(itemTransaction => itemTransaction.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedById).IsRequired();
        builder.HasOne(itemTransaction => itemTransaction.CreatedByUser)
            .WithMany()
            .HasForeignKey(itemTransaction => itemTransaction.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
