using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class Timesheet : AuditableEntity
{
    public int Id { get; set; }
    public required string TaskName { get; set; }
    public int FactTrackedInMinutes { get; set; }
    public int BillableTrackedInMinutes { get; set; }
    public DateOnly ReportForDate { get; set; }
    public DateTime OrderByDateUtc { get; set; }

    public int ReportYear { get; set; } // for reports
    public int ReportMonthOfYear { get; set; } // for reports
    public int ReportWeekOfYear { get; set; } // for reports
    public int ReportDayOfYear { get; set; } // for reports

    public int UserId { get; set; }
    public int ProjectId { get; set; }

    public TimeTrackUser? User { get; set; }
    public Project? Project { get; set; }
    public TimeTrackUser? CreatedByUser { get; set; }
    public TimeTrackUser? UpdatedByUser { get; set; }

    public static readonly Expression<Func<Timesheet, DashboardActivity>> Map = t => new DashboardActivity
    {
        ProjectName = t.Project!.Name,
        TaskName = t.TaskName,
        ReportForDate = t.ReportForDate,
        TrackedInMinutes = t.FactTrackedInMinutes,
        CreatedAtUtc = t.CreatedAtUtc,
        UpdatedAtUtc = t.UpdatedAtUtc,
        OrderByDateUtc = t.OrderByDateUtc
    };
}

internal class TimesheetConfiguration : IEntityTypeConfiguration<Timesheet>
{
    public virtual void Configure(EntityTypeBuilder<Timesheet> builder)
    {
        builder.ToTable("Timesheets", TimeTrackDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TaskName).IsRequired().HasMaxLength(512);
        builder.Property(x => x.FactTrackedInMinutes).IsRequired();
        builder.Property(x => x.BillableTrackedInMinutes).IsRequired();
        builder.Property(x => x.ReportForDate).IsRequired();
        builder.Property(x => x.ReportYear).IsRequired();
        builder.Property(x => x.ReportMonthOfYear).IsRequired();
        builder.Property(x => x.ReportWeekOfYear).IsRequired();
        builder.Property(x => x.ReportDayOfYear).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);
        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Timesheets)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Timesheets)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.ReportYear, x.ReportWeekOfYear })
            .IsUnique(false);

        builder.HasIndex(x => new { x.UserId, x.ReportYear, x.ReportMonthOfYear })
            .IsUnique(false);

        builder.HasIndex(x => new { x.UserId, x.ReportYear, x.ReportMonthOfYear, x.ReportWeekOfYear })
            .IsUnique(false);

        builder.HasIndex(x => new { x.ReportYear, x.ReportMonthOfYear })
            .IsUnique(false);

        builder.HasIndex(x => new { x.ReportYear, x.ReportDayOfYear })
            .IsUnique(false);

        builder
            .HasIndex(x => new { x.UserId, x.ProjectId, x.ReportForDate, x.TaskName })
            .HasDatabaseName("IX_Timesheets_Unique_UserId_ProjectId_ReportForDate_TaskName")
            .IsUnique();

        builder
            .Property(x => x.OrderByDateUtc)
            .HasComputedColumnSql("COALESCE(`UpdatedAtUtc`, `CreatedAtUtc`)", stored: true);

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
