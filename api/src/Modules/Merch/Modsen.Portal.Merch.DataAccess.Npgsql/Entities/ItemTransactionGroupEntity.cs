using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class ItemTransactionGroupEntity : IPortalEntity
{
    public Guid GroupCode { get; set; }

    public string? Notes { get; set; }

    public int? CreatedById { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public MerchUserEntity? CreatedByUser { get; set; }

    public List<ItemTransactionEntity> ItemTransactions { get; set; } = [];
}

internal sealed class ItemTransactionGroupConfiguration : IEntityTypeConfiguration<ItemTransactionGroupEntity>
{
    public void Configure(EntityTypeBuilder<ItemTransactionGroupEntity> builder)
    {
        builder.ToTable("ItemTransactionGroups");

        builder.HasKey(itemTransactionGroup => itemTransactionGroup.GroupCode);
        builder.Property(itemTransactionGroup => itemTransactionGroup.GroupCode)
            .ValueGeneratedOnAdd();

        builder.Property(itemTransactionGroup => itemTransactionGroup.Notes)
            .IsRequired(false)
            .HasMaxLength(1024);

        builder.Property(itemTransactionGroup => itemTransactionGroup.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.CreatedById).IsRequired();
        builder.HasOne(itemTransactionGroup => itemTransactionGroup.CreatedByUser)
            .WithMany()
            .HasForeignKey(itemTransactionGroup => itemTransactionGroup.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
