using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class ReportRequest
{
    public Guid Id { get; set; }
    public string? ReportName { get; set; }
    public double SizeInKilobytes { get; set; }
    public DateTime RequestedAtUtc { get; set; }
    public DateTime? RequestIsValidTill { get; set; }
    public int RequestedById { get; set; }
    public string? ReportFileId { get; set; }
    public string? FilterHash { get; set; }
    public string? FilterArgs { get; set; }
}

internal class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
{
    public void Configure(EntityTypeBuilder<ReportRequest> builder)
    {
        builder.HasKey(request => request.Id);

        builder.Property(request => request.ReportName)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(request => request.SizeInKilobytes)
            .IsRequired();

        builder.Property(request => request.RequestedAtUtc)
            .IsRequired();

        builder.Property(request => request.RequestIsValidTill)
            .IsRequired(false);

        builder.Property(request => request.RequestedById)
            .IsRequired();

        builder.Property(request => request.ReportFileId)
            .IsRequired();

        builder.Property(equipmentReport => equipmentReport.FilterHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(equipmentReport => equipmentReport.FilterArgs)
            .HasMaxLength(2048)
            .IsRequired();
    }
}
