using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

[assembly: InternalsVisibleTo("Modsen.Portal.Api")]
namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql;

internal class TimeTrackDbContext : DbContext, ITimeTrackDbContext
{
    public const string Schema = "timetrack";

    public DbSet<Client> Clients { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<Timesheet> Timesheets { get; set; }
    public DbSet<TimeTrackUser> TimeTrackUsers { get; set; }
    public DbSet<ReportRequest> ReportRequests { get; set; }
    public DbSet<TimesheetSubmitStatus> TimesheetSubmitStatuses { get; set; }
    public DbSet<TimeTrackResourceGroup> TimeTrackResourceGroups { get; set; }
    public DbSet<JiraSetting> JiraSettings { get; set; }

    public TimeTrackDbContext(DbContextOptions<TimeTrackDbContext> dbContextOptions)
        : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeTrackDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration("OutboxEvents", Schema));
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }
}
