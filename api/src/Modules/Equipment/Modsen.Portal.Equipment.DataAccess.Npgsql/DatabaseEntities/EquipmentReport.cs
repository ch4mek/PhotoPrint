using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentReport
{
    public int Id { get; set; }
    public required string SerialNumber { get; set; }
    public required string Data { get; set; }
    public required string DataHash { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime AssembledAtUtc { get; set; }

    public ICollection<EquipmentReportRelevancePeriod> EquipmentReportRelevancePeriods { get; set; } = [];
}

internal class EquipmentReportConfiguration : IEntityTypeConfiguration<EquipmentReport>
{
    public void Configure(EntityTypeBuilder<EquipmentReport> builder)
    {
        builder.ToTable("EquipmentReports");

        builder.HasKey(equipmentReport => equipmentReport.Id);

        builder.Property(equipmentReport => equipmentReport.AssembledAtUtc)
            .IsRequired();

        builder.Property(equipmentReport => equipmentReport.SerialNumber)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(equipmentReport => equipmentReport.Data)
            .IsRequired()
            .HasMaxLength(1048576)
            .HasColumnType("json");

        builder.Property(equipmentReport => equipmentReport.DataHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(equipmentReport => equipmentReport.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}
