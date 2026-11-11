using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class WarehouseEntity
{
    public int Id { get; set; }

    public required string Location { get; set; }

    public List<ItemTransactionEntity> ItemTransactions { get; set; } = [];
    public List<ItemTransactionEntity> SenderItemTransactions { get; set; } = [];

    public List<ItemSizeMinAmountEntity> ItemSizeMinAmounts { get; set; } = [];

    public List<KitMinAmountEntity> KitMinAmounts { get; set; } = [];
}

internal sealed class WarehouseConfiguration : IEntityTypeConfiguration<WarehouseEntity>
{
    public void Configure(EntityTypeBuilder<WarehouseEntity> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(warehouse => warehouse.Id);
        builder.Property(warehouse => warehouse.Id)
            .ValueGeneratedOnAdd();

        builder.Property(warehouse => warehouse.Location)
            .IsRequired()
            .HasMaxLength(32);
    }
}
