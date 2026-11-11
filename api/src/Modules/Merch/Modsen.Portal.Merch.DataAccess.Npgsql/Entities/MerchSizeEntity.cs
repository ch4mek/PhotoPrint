using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class MerchSizeEntity
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public int OrderWeight { get; set; }

    public bool IsForClothing { get; set; }

    public List<ItemTransactionEntity> ItemTransactions { get; set; } = [];

    public List<ItemSizeMinAmountEntity> ItemSizeMinAmounts { get; set; } = [];
}

internal sealed class MerchSizeConfiguration : IEntityTypeConfiguration<MerchSizeEntity>
{
    public void Configure(EntityTypeBuilder<MerchSizeEntity> builder)
    {
        builder.ToTable("MerchSizes");

        builder.HasKey(merchSize => merchSize.Id);
        builder.Property(merchSize => merchSize.Id)
            .HasMaxLength(3)
            .ValueGeneratedNever();

        builder.Property(merchSize => merchSize.Name)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(merchSize => merchSize.OrderWeight)
            .IsRequired();

        builder.Property(merchSize => merchSize.IsForClothing)
            .IsRequired();
    }
}
