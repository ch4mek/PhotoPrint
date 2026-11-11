using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

[assembly: InternalsVisibleTo("Modsen.Portal.TimeTrack.UseCases")]
namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql;

internal interface ITimeTrackDbContext
{
    DbSet<TimeTrackUser> TimeTrackUsers { get; }
    DbSet<Timesheet> Timesheets { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectUser> ProjectUsers { get; }
    DbSet<Client> Clients { get; }
    DbSet<ReportRequest> ReportRequests { get; set; }
    DbSet<TimesheetSubmitStatus> TimesheetSubmitStatuses { get; set; }
    DbSet<TimeTrackResourceGroup> TimeTrackResourceGroups { get; set; }
    DbSet<JiraSetting>  JiraSettings { get; set; }
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
