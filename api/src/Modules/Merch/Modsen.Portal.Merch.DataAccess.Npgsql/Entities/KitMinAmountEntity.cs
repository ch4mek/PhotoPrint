using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class KitMinAmountEntity
{
    public int KitId { get; set; }
    public KitEntity? Kit { get; set; }

    public int WarehouseId { get; set; }
    public WarehouseEntity? Warehouse { get; set; }

    public int MinAmount { get; set; }
}

internal sealed class KitMinAmountEntityConfiguration : IEntityTypeConfiguration<KitMinAmountEntity>
{
    public void Configure(EntityTypeBuilder<KitMinAmountEntity> builder)
    {
        builder.ToTable("KitMinAmounts");

        builder.HasKey(minAmount => new {minAmount.KitId, minAmount.WarehouseId});

        builder.HasOne(minAmount => minAmount.Kit)
            .WithMany(kit => kit.KitMinAmounts)
            .HasForeignKey(minAmount => minAmount.KitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(minAmount => minAmount.Warehouse)
            .WithMany(warehouse => warehouse.KitMinAmounts)
            .HasForeignKey(minAmount => minAmount.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
