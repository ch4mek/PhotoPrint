using Modsen.Portal.Core;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.StimulsoftReport.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;
using ReportRequest = Modsen.Portal.StimulsoftReport.Contract.ReportRequest;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;

internal abstract class GetTrackedTimeToExportRequestBase
{
    private readonly IDateService _dateService;
    private readonly IReportExporter _exporter;
    private readonly IPeriodService _periodService;

    protected GetTrackedTimeToExportRequestBase(
        IDateService dateService,
        IReportExporter exporter,
        IPeriodService periodService)
    {
        _dateService = dateService;
        _exporter = exporter;
        _periodService = periodService;
    }

    protected static List<ProjectToExport> GetProjectsToExport(List<Timesheet> timesheets, TrackedTimeType trackedTimeType)
    {
        var projectsToExport = new List<ProjectToExport>();

        if (timesheets.Count == 0)
        {
            return projectsToExport;
        }

        var clientsGroup = timesheets.GroupBy(timesheet => timesheet.Project!.Client!.Id);

        foreach (var clientGroup in clientsGroup)
        {
            var clientTimesheets = clientGroup.ToList();
            projectsToExport.AddRange(clientTimesheets
                .GroupBy(timesheet => timesheet.Project!.Name)
                .Select(projectGroup => GetProjectToExport(projectGroup.ToList(), trackedTimeType)));
        }

        return projectsToExport.OrderBy(project => project.ProjectName).ToList();
    }

    private static ProjectToExport GetProjectToExport(List<Timesheet> timesheets, TrackedTimeType trackedTimeType)
    {
        var usersToExport = GetUsersToExport(timesheets, trackedTimeType);

        var projectTotalMinutes = usersToExport.Sum(x => x.TotalTimeInMinutes);
        var projectTimeTotalHours = projectTotalMinutes / 60;
        var projectRemainedMinutes = projectTotalMinutes % 60;

        var projectToExport = new ProjectToExport
        {
            ProjectName = timesheets[0].Project!.Name,
            ClientName = timesheets[0].Project!.Client!.Name,
            Users = usersToExport,
            TotalTime = new Duration
            {
                Hours = projectTimeTotalHours,
                Minutes = projectRemainedMinutes
            }
        };

        return projectToExport;
    }

    private static List<UserToExport> GetUsersToExport(IEnumerable<Timesheet> timesheets, TrackedTimeType trackedTimeType)
    {
        var usersToExport = new List<UserToExport>();
        var userGroups = timesheets
            .GroupBy(timesheet => timesheet.UserId);

        foreach (var userGroup in userGroups)
        {
            var userTimesheets = userGroup.ToList();

            var tasksPerDates = GetDatesWithTasksToExport(userTimesheets, trackedTimeType);

            var userTimeTotalMinutes = tasksPerDates.Sum(x => x.TotalTimeInMinutes);
            var userTimeTotalHours = userTimeTotalMinutes / 60;
            var userTimeRemainedMinutes = userTimeTotalMinutes % 60;

            var userToExport = new UserToExport
            {
                FirstName = userTimesheets[0].User!.FirstName,
                LastName = userTimesheets[0].User!.LastName,
                TotalTime = new Duration
                {
                    Hours = userTimeTotalHours,
                    Minutes = userTimeRemainedMinutes
                },
                TotalTimeInMinutes = userTimeTotalMinutes,
                Dates = tasksPerDates
            };

            usersToExport.Add(userToExport);
        }

        return usersToExport
            .OrderBy(users => users.LastName)
            .ToList();
    }

    private static List<TasksPerDateToExport> GetDatesWithTasksToExport(IEnumerable<Timesheet> userTimesheets, TrackedTimeType trackedTimeType)
    {
        var datesWithTasks = userTimesheets
            .GroupBy(timesheet => timesheet.ReportForDate)
            .Select(dateGroup =>
            {
                var tasks = GetTasksToExport(dateGroup.OrderBy(timesheet => timesheet.Id), trackedTimeType);

                var totalMinutes = dateGroup.Sum(GetTrackedTimeSelector(trackedTimeType));
                var totalHours = totalMinutes / 60;
                var remainedMinutes = totalMinutes % 60;

                return new TasksPerDateToExport
                {
                    Date = dateGroup.Key,
                    TotalTimeInMinutes = totalMinutes,
                    TotalTime = new Duration
                    {
                        Hours = totalHours,
                        Minutes = remainedMinutes
                    },
                    Tasks = tasks
                };
            });

        return datesWithTasks
            .OrderBy(tasksPerDate => tasksPerDate.Date)
            .ToList();
    }

    private static List<TaskToExport> GetTasksToExport(IEnumerable<Timesheet> timesheets, TrackedTimeType trackedTimeType)
    {
        const string emptyTaskName = "-";

        var tasks = timesheets.Select(timesheet =>
        {
            var trackedHours = GetTrackedTimeByType(timesheet, trackedTimeType) / 60;
            var remainedMinutes = GetTrackedTimeByType(timesheet, trackedTimeType) % 60;

            return new TaskToExport
            {
                TrackedTime = new Duration
                {
                    Hours = trackedHours,
                    Minutes = remainedMinutes
                },
                TaskName = timesheet.TaskName.IsNullOrEmpty() ? emptyTaskName : timesheet.TaskName
            };
        });

        return tasks.ToList();
    }

    protected async Task<byte[]> GetReportAsync(ReportRequest reportRequest, DocumentType format, CancellationToken token)
    {
        return format switch
        {
            DocumentType.Pdf => await _exporter.ExportPdfAsync(reportRequest, token),
            DocumentType.Word => await _exporter.ExportWordAsync(reportRequest, token),
            _ => await _exporter.ExportPdfAsync(reportRequest, token)
        };
    }

    protected ReportTitle CreateReportTitle(string? subjectName, IPeriodRequest periodRequest)
    {
        var periodTypeDescription = GetReportPeriodType(periodRequest).GetDescription();
        var dateSpan = _dateService.GetDateSpan(periodRequest);

        var reportTitle = new ReportTitle
        {
            SubjectName = subjectName,
            PeriodInfo = subjectName is null
                ? periodTypeDescription.WithFirstCharToUpper()
                : periodTypeDescription,
            FromDate = dateSpan.StartDate,
            ToDate = dateSpan.EndDate
        };

        return reportTitle;
    }

    private ReportPeriodType GetReportPeriodType(IPeriodRequest periodRequest)
    {
        if (_periodService.IsFilterForEntireYear(periodRequest))
        {
            return ReportPeriodType.Annual;
        }

        if (_periodService.IsFilterForSelectedQuarter(periodRequest))
        {
            return ReportPeriodType.Quarter;
        }

        return _periodService.IsFilterForOneSelectedMonth(periodRequest)
            ? ReportPeriodType.Monthly
            : ReportPeriodType.Arbitrary;
    }

    private static int GetTrackedTimeByType(Timesheet timesheet, TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => timesheet.FactTrackedInMinutes,
            TrackedTimeType.Billable => timesheet.BillableTrackedInMinutes,
            _ => throw new InvalidOperationException()
        };
    }

    private static Func<Timesheet, int> GetTrackedTimeSelector(TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => x => x.FactTrackedInMinutes,
            TrackedTimeType.Billable => x => x.BillableTrackedInMinutes,
            _ => throw new InvalidOperationException()
        };
    }
}
