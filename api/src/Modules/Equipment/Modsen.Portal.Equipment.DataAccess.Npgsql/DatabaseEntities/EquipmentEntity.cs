using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public EquipmentLocationType Location { get; set; }
    public required EquipmentBrokenStatus BrokenStatus { get; set; }
    public required string SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }
    public decimal PurchasePrice { get; set; }
    public MoneyCurrencyType PurchaseCurrency { get; set; }
    public decimal PurchasePriceUsd { get; set; }
    public DateTime PurchaseDate { get; set; }
    public required string PurchasePlace { get; set; }
    public DateTime GuaranteeDate { get; set; }
    public required string Characteristics { get; set; }
    public string? Comment { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? JiraTask { get; set; }

    public ICollection<EquipmentAssignEntity> Assigns { get; set; } = [];

    public int? ApproverId { get; set; }
    public EquipmentUser? Approver { get; set; }

    public string? TypeId { get; set; }
    public EquipmentTypeEntity? Type { get; set; }

    public LocationType? Storage { get; set; }
    public string? RepairPlace { get; set; }
    public EquipmentUser? CreatedByUser { get; set; }
    public EquipmentUser? UpdatedByUser { get; set; }
}

internal class EquipmentConfiguration : IEntityTypeConfiguration<EquipmentEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentEntity> builder)
    {
        builder.ToTable("Equipments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
        builder.Property(x => x.CreatedById)
            .IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired(false);
        builder.Property(x => x.UpdatedById)
            .IsRequired(false);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Location)
            .IsRequired()
            .HasConversion(new EnumConverter<EquipmentLocationType>());

        builder.Property(x => x.BrokenStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.SerialNumber)
            .IsRequired()
            .HasMaxLength(250);
        builder.HasIndex(x => x.SerialNumber);

        builder.Property(x => x.InventoryNumber)
            .HasMaxLength(250);
        builder.HasIndex(x => x.InventoryNumber)
            .IsUnique();

        builder.Property(x => x.PurchasePrice)
            .IsRequired();
        builder.Property(x => x.PurchasePriceUsd)
            .IsRequired();

        builder.Property(x => x.PurchaseCurrency)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.PurchaseDate)
            .IsRequired();

        builder.Property(x => x.PurchasePlace)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.GuaranteeDate)
            .IsRequired();

        builder.Property(x => x.Characteristics)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Comment)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(x => x.InvoiceNumber)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.HasOne(x => x.Approver)
            .WithMany(x => x.ApprovedEquipments)
            .HasForeignKey(x => x.ApproverId);

        builder.HasOne(x => x.Type)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(x => x.TypeId);

        builder.Property(x => x.TypeId)
            .HasMaxLength(64);

        builder.Property(x => x.TypeId)
            .HasMaxLength(64);

        builder.Property(x => x.Storage).HasConversion<string>();

        builder.Property(x => x.RepairPlace).HasMaxLength(250);

        builder.Property(x => x.JiraTask)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
