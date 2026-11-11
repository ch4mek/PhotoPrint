using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal class ItemSizeMinAmountEntity
{
    public int ItemId { get; set; }

    public ItemEntity? Item { get; set; }

    public required string SizeId { get; set; } = null!;

    public MerchSizeEntity? MerchSize { get; set; }

    public int WarehouseId { get; set; }

    public WarehouseEntity? Warehouse { get; set; }

    public int MinAmount { get; set; }
}

internal class ItemSizeMinAmountConfiguration : IEntityTypeConfiguration<ItemSizeMinAmountEntity>
{
    public void Configure(EntityTypeBuilder<ItemSizeMinAmountEntity> builder)
    {
        builder.ToTable("ItemSizeMinAmounts");

        builder.HasKey(sizeMinAmount => new { sizeMinAmount.ItemId, sizeMinAmount.SizeId, sizeMinAmount.WarehouseId});

        builder.HasOne(sizeAmount => sizeAmount.Item)
            .WithMany(item => item.ItemSizeMinAmounts)
            .HasForeignKey(sizeMinAmount => sizeMinAmount.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sizeAmount => sizeAmount.MerchSize)
            .WithMany(merchSize => merchSize.ItemSizeMinAmounts)
            .HasForeignKey(sizeMinAmount => sizeMinAmount.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sizeAmount => sizeAmount.Warehouse)
            .WithMany(warehouse => warehouse.ItemSizeMinAmounts)
            .HasForeignKey(sizeAmount => sizeAmount.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
