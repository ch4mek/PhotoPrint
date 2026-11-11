using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

internal sealed class KitEntity : AuditableEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? PhotoFileId { get; set; }

    public MerchItemStatus Status { get; set; }

    public MerchUserEntity? CreatedByUser { get; set; }

    public MerchUserEntity? UpdatedByUser { get; set; }

    public int? ArchivedById { get; set; }

    public DateTime? ArchivedAtUtc { get; set; }

    public MerchUserEntity? ArchivedByUser { get; set; }

    public List<KitItemEntity> KitItems { get; set; } = [];

    public List<KitMinAmountEntity> KitMinAmounts { get; set; } = [];
}

internal sealed class KitConfiguration : IEntityTypeConfiguration<KitEntity>
{
    public void Configure(EntityTypeBuilder<KitEntity> builder)
    {
        builder.ToTable("Kits");

        builder.HasKey(kit => kit.Id);
        builder.Property(kit => kit.Id)
            .ValueGeneratedOnAdd();

        builder.Property(kit => kit.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(kit => kit.Description)
            .IsRequired(false)
            .HasMaxLength(2048);

        builder.Property(kit => kit.PhotoFileId)
           .IsRequired(false)
           .HasMaxLength(32);

        builder.Property(item => item.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(MerchItemStatus.Available);

        builder.Property(kit => kit.CreatedAtUtc)
            .IsRequired();

        builder.Property(kit => kit.UpdatedAtUtc)
            .IsRequired(false);

        builder.Property(kit => kit.ArchivedAtUtc)
            .IsRequired(false);

        builder.Property(kit => kit.CreatedById).IsRequired();
        builder.HasOne(kit => kit.CreatedByUser)
            .WithMany()
            .HasForeignKey(kit => kit.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(kit => kit.UpdatedByUser)
            .WithMany()
            .HasForeignKey(kit => kit.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(kit => kit.ArchivedByUser)
            .WithMany()
            .HasForeignKey(kit => kit.ArchivedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(item => item.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(MerchItemStatus.Available);
    }
}
