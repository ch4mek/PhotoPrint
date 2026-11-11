using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class TimesheetSubmitStatus
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TimesheetSubmitStatusType Status { get; set; }
    public int DecisionBy { get; set; }
    public DateOnly Date { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int FactHoursSum { get; set; }
    public int BillableHoursSum { get; set; }
    public required string Snapshot { get; set; }
    public TimeTrackUser? TimeTrackUser { get; set; }
    public TimeTrackUser? DecisionByUser { get; set; }
}

internal class SubmitStatusConfiguration : IEntityTypeConfiguration<TimesheetSubmitStatus>
{
    public virtual void Configure(EntityTypeBuilder<TimesheetSubmitStatus> builder)
    {
        builder.ToTable("TimesheetSubmitStatuses", TimeTrackDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasConversion<string>();
        builder.Property(x => x.DecisionBy).IsRequired();
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Month).IsRequired();
        builder.Property(x => x.FactHoursSum).IsRequired();
        builder.Property(x => x.BillableHoursSum).IsRequired();
        builder.Property(x => x.Snapshot).IsRequired();

        builder.HasOne(x => x.TimeTrackUser)
            .WithMany(x => x.TimesheetSubmitStatuses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.Year, x.Month })
            .IsUnique();

        builder.HasOne(x => x.DecisionByUser)
            .WithMany()
            .HasForeignKey(x => x.DecisionBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
