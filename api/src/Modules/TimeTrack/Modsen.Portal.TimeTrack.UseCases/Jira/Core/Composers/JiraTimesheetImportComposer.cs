using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Core.Composers;

internal interface IJiraTimesheetImportComposer
{
    Task<JiraImportResultDto> Compose(
        IReadOnlyCollection<JiraImportedWorklog> worklogs,
        int userId,
        int projectId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}

internal sealed class JiraTimesheetImportComposer : IJiraTimesheetImportComposer
{
    private readonly ITimeTrackDbContext _dbContext;
    private readonly ITimesheetComposer _timesheetComposer;

    public JiraTimesheetImportComposer(
        ITimeTrackDbContext dbContext,
        ITimesheetComposer timesheetComposer)
    {
        _dbContext = dbContext;
        _timesheetComposer = timesheetComposer;
    }

    public async Task<JiraImportResultDto> Compose(
        IReadOnlyCollection<JiraImportedWorklog> worklogs,
        int userId,
        int projectId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var importedTimesheets = BuildImportedTimesheets(worklogs);

        if (importedTimesheets.Count == 0)
        {
            return new JiraImportResultDto();
        }

        var existingKeys = await GetExistingTimesheetKeys(
            importedTimesheets,
            userId,
            projectId,
            startDate,
            endDate,
            cancellationToken);

        var updated = 0;
        var created = 0;
        var skipped = 0;

        var projectsUsers = await _dbContext.ProjectUsers
            .Where(x => x.ProjectId == projectId && x.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var importedTimesheet in importedTimesheets)
        {
            var isUserAssigned = projectsUsers.Any(x =>
                x.UserOnProjectFrom <= importedTimesheet.ReportForDate &&
                (x.UserOnProjectTo == null || x.UserOnProjectTo >= importedTimesheet.ReportForDate));

            if (isUserAssigned)
            {
                if (existingKeys.Contains(new ExistingTimesheetKey(importedTimesheet.ReportForDate, importedTimesheet.TaskName)))
                {
                    updated++;
                }
                else
                {
                    created++;
                }

                await _timesheetComposer.Compose(new UpsertFactDto
                {
                    UserId = userId,
                    ProjectId = projectId,
                    ReportForDate = importedTimesheet.ReportForDate,
                    TaskName = importedTimesheet.TaskName,
                    FactTrackedInMinutes = importedTimesheet.FactTrackedInMinutes,
                    AllowAutomatic = true,
                }, cancellationToken);
            }
            else
            {
                skipped++;
            }
        }

        return new JiraImportResultDto
        {
            Created = created,
            Updated = updated,
            Skipped = skipped,
            Total = created + updated + skipped,
        };
    }

    private static List<ImportedTimesheet> BuildImportedTimesheets(
        IReadOnlyCollection<JiraImportedWorklog> worklogs)
    {
        return worklogs
            .Where(x => x.TimeSpentSeconds > 0)
            .GroupBy(x => new
            {
                x.StartedDate,
                TaskName = $"[{x.IssueKey}] {x.IssueSummary}"
            })
            .Select(x => new ImportedTimesheet(
                x.Key.StartedDate,
                x.Key.TaskName,
                x.Sum(worklog => worklog.TimeSpentSeconds) / 60))
            .Where(x => x.FactTrackedInMinutes > 0)
            .ToList();
    }

    private async Task<HashSet<ExistingTimesheetKey>> GetExistingTimesheetKeys(
        IReadOnlyCollection<ImportedTimesheet> importedTimesheets,
        int userId,
        int projectId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var taskNames = importedTimesheets.Select(x => x.TaskName).Distinct().ToList();
        var existingKeys = await _dbContext.Timesheets
            .AsNoTracking()
            .Where(x => x.UserId == userId &&
                x.ProjectId == projectId &&
                x.ReportForDate >= startDate &&
                x.ReportForDate <= endDate &&
                taskNames.Contains(x.TaskName))
            .Select(x => new ExistingTimesheetKey(x.ReportForDate, x.TaskName))
            .ToListAsync(cancellationToken);

        return existingKeys.ToHashSet();
    }

    private sealed record ImportedTimesheet(DateOnly ReportForDate, string TaskName, int FactTrackedInMinutes);

    private sealed record ExistingTimesheetKey(DateOnly ReportForDate, string TaskName);
}
