using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class KitItemEntity
{
    public int Id { get; set; }

    public int KitId { get; set; }

    public KitEntity? Kit { get; set; }

    public int ItemId { get; set; }

    public ItemEntity? Item { get; set; }

    public int Count { get; set; }
}

internal sealed class KitItemConfiguration : IEntityTypeConfiguration<KitItemEntity>
{
    public void Configure(EntityTypeBuilder<KitItemEntity> builder)
    {
        builder.ToTable("KitItems");

        builder.HasKey(kitItem => kitItem.Id);
        builder.Property(kitItem => kitItem.Id)
            .ValueGeneratedOnAdd();

        builder.HasOne(kitItem => kitItem.Kit)
            .WithMany(kit => kit.KitItems)
            .HasForeignKey(kitItem => kitItem.KitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(kitItem => kitItem.Item)
            .WithMany(item => item.KitItems)
            .HasForeignKey(kitItem => kitItem.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(kitItem => kitItem.Count)
            .IsRequired(); 
    }
}
