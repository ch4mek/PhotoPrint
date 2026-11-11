using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

internal class EquipmentReportRelevancePeriod
{
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public int EquipmentReportId { get; set; }

    public EquipmentReport? EquipmentReport { get; set; }

}

internal class EquipmentReportRelevancePeriodConfiguration :
    IEntityTypeConfiguration<EquipmentReportRelevancePeriod>
{
    public void Configure(EntityTypeBuilder<EquipmentReportRelevancePeriod> builder)
    {
        builder.ToTable("EquipmentReportRelevancePeriods");

        builder.HasKey(reportRelevancePeriod => new
        {
            reportRelevancePeriod.EquipmentReportId,
            reportRelevancePeriod.FromUtc
        });

        builder.Property(reportRelevancePeriod => reportRelevancePeriod.ToUtc)
            .IsRequired();

        builder.HasOne(reportRelevancePeriod => reportRelevancePeriod.EquipmentReport)
            .WithMany(equipmentReport => equipmentReport.EquipmentReportRelevancePeriods)
            .HasForeignKey(reportRelevancePeriod => reportRelevancePeriod.EquipmentReportId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
