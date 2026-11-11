using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetTrackedTimePerPeriodRequest : IQueryHandler<GetTrackedTimePerPeriodRequest.Query, TrackedTimePerPeriodReportDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IUserAccessService _userAccessService;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IGetTimesheetsByPeriodWithFilterDbQuery _getTimesheetsDbQuery;

    public GetTrackedTimePerPeriodRequest(
        IAuthenticatedUserContext authenticatedUser,
        IUserAccessService userAccessService,
        ITimeTrackDbContext timeTrackDbContext,
        IGetTimesheetsByPeriodWithFilterDbQuery getTimesheetsDbQuery)
    {
        _authenticatedUser = authenticatedUser;
        _userAccessService = userAccessService;
        _timeTrackDbContext = timeTrackDbContext;
        _getTimesheetsDbQuery = getTimesheetsDbQuery;
    }

    public async Task<TrackedTimePerPeriodReportDto> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        if (request.UserId.HasValue && !await _userAccessService.CanAccessUserReportInformation(request.UserId.Value, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.CannotAccessOtherUserData);
        }

        var timesheetFilter = new TimesheetFilter
        {
            UserId = _authenticatedUser.GetRoles().IsEmployee() || _authenticatedUser.GetRoles().IsSystemAdministrator()
                        ? _authenticatedUser.UserId
                        : request.UserId,
            ResourceGroupId = _authenticatedUser.GetRoles().IsGroupManager()
                ? _authenticatedUser.ResourceGroupId
                : request.ResourceGroupId,
            DepartmentId = _authenticatedUser.GetRoles().IsHeadOfDepartment()
                        ? _authenticatedUser.DepartmentId
                        : request.DepartmentId,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
        };

        var dbRequest = new GetTimesheetsByPeriodWithFilterDbRequest
            (request.Year, request.MonthOfYear, request.WeekOfYear, timesheetFilter);

        var timesheets = await _getTimesheetsDbQuery.ExecuteAsync(dbRequest, cancellationToken);

        var barChartsInfo = timesheets.Select(x => new BarChartInfoDto
        {
            ProjectId = x.ProjectId,
            ProjectName = x.Project!.Name,
            ProjectColor = x.Project!.Color,
            FactTrackedInMinutes = x.FactTrackedInMinutes,
            BillableTrackedInMinutes = x.BillableTrackedInMinutes,
            ReportYear = x.ReportYear,
            ReportMonthOfYear = x.ReportMonthOfYear,
            ReportWeekOfYear = x.ReportWeekOfYear,
            ReportDayOfYear = x.ReportDayOfYear,
            ReportForDate = x.ReportForDate
        }).ToList();

        var resultBarChart = CalculateBarChartInfo(
            barChartsInfo, request.Year, request.MonthOfYear, request.WeekOfYear, request.TrackedTimeType);

        var totalTimeInMinutes = resultBarChart.Sum(x => x.TotalTimeInMinutes);

        var uniqueProjectIds = barChartsInfo.Select(x => x.ProjectId).Distinct().ToList();
        var billableProjectsIds = new List<int>();

        foreach (var projectId in uniqueProjectIds)
        {
            var project = await _timeTrackDbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project != null && project.IsBillable)
            {
                billableProjectsIds.Add(projectId);
            }
        }

        var updatedReports = UpdateIsBillableProjectStatus(resultBarChart, billableProjectsIds);

        var sortedResultBarChart = updatedReports
            .Select(report => new TimePerPeriodReportDto
            {
                Period = report.Period,
                TotalTimeInMinutes = report.TotalTimeInMinutes,
                Projects = report.Projects
                    .Where(p => p.TotalTime != 0)
                    .OrderByDescending(p => p.IsBillableProject)
                    .ThenBy(p => p.ProjectId)
                    .ToList()
            })
            .ToList();

        var result = new TrackedTimePerPeriodReportDto
        {
            TotalTimeInMinutes = totalTimeInMinutes,
            TimePerPeriod = sortedResultBarChart,
            Projects = timesheets
                .OrderBy(x => x.ProjectId)
                .Select(x => x.ProjectId)
                .Distinct()
                .ToList()
        };

        return result;
    }

    internal sealed record Query : IQuery<TrackedTimePerPeriodReportDto>, IPeriodRequest, ITimeTrackRequest
    {
        public int? Year { get; init; }
        public int[]? MonthOfYear { get; init; } = [];
        public int? WeekOfYear { get; init; }
        public string? DepartmentId { get; init; }
        public int? ResourceGroupId { get; init; }
        public int? ClientId { get; init; }
        public int? ProjectId { get; init; }
        public int? UserId { get; init; }
        public TrackedTimeType TrackedTimeType { get; init; }
    }

    private static List<TimePerPeriodReportDto> UpdateIsBillableProjectStatus(List<TimePerPeriodReportDto> reports, List<int> billableProjectIds)
    {
        var billableProjectIdsSet = new HashSet<int>(billableProjectIds);

        foreach (var report in reports)
        {
            foreach (var project in report.Projects)
            {
                project.IsBillableProject = billableProjectIdsSet.Contains(project.ProjectId);
            }
        }

        return reports;
    }

    private static List<TimePerPeriodReportDto> CalculateBarChartInfo(
        List<BarChartInfoDto> barChartInfoList,
        int? year,
        int[]? monthOfYear,
        int? weekOfYear,
        TrackedTimeType trackedTimeType)
    {
        Func<BarChartInfoDto, int> trackedTimeSelector = trackedTimeType switch
        {
            TrackedTimeType.Fact => x => x.FactTrackedInMinutes ?? 0,
            TrackedTimeType.Billable => x => x.BillableTrackedInMinutes ?? 0,
            _ => throw new InvalidOperationException("Unsupported tracked time type")
        };

        if (year is null)
        {
            return GroupBarChartInfoByYears(barChartInfoList, trackedTimeSelector);
        }

        if (monthOfYear is { Length: 0 } && weekOfYear is null)
        {
            return GroupBarChartInfoByMonthOfYear(barChartInfoList, trackedTimeSelector, year.Value);
        }

        if (monthOfYear is { Length: > 0 } && weekOfYear is null)
        {
            return monthOfYear.Length == 1
                ? GroupBarChartInfoByDaysInMonth(barChartInfoList, year.Value, monthOfYear[0], trackedTimeSelector)
                : GroupBarChartInfoByMonthOfYear(barChartInfoList, trackedTimeSelector, year.Value, monthOfYear);
        }

        return GroupBarChartInfoByDaysInWeek(barChartInfoList, year.Value, weekOfYear!.Value, trackedTimeSelector);
    }

    private static List<TimePerPeriodReportDto> GroupBarChartInfoByYears(
        List<BarChartInfoDto> barChartInfoList,
        Func<BarChartInfoDto, int> trackedTimeSelector)
    {
        var result = barChartInfoList
            .GroupBy(t => t.ReportYear)
            .Select(g => new TimePerPeriodReportDto
            {
                Period = new DateTime(g.Key == 0 ? 1 : g.Key, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                TotalTimeInMinutes = g.Sum(trackedTimeSelector),
                Projects = g.GroupBy(x => x.ProjectId)
                            .Select(p => new BarChartInfoResponseDto
                            {
                                ProjectId = p.Key,
                                ProjectName = p.First().ProjectName,
                                ProjectColor = p.First().ProjectColor,
                                TotalTime = p.Sum(trackedTimeSelector)
                            })
                            .ToList()
            })
            .OrderBy(x => x.Period)
            .ToList();

        return result;
    }

    private static List<TimePerPeriodReportDto> GroupBarChartInfoByMonthOfYear(
    List<BarChartInfoDto> barChartInfoList,
    Func<BarChartInfoDto, int> trackedTimeSelector,
    int year, int[]? months = null)
    {
        var periods = months ?? Enumerable.Range(1, 12);

        var reportPeriods = barChartInfoList
            .Where(t => periods.Contains(t.ReportMonthOfYear))
            .GroupBy(t => t.ReportMonthOfYear)
            .Select(g => new
            {
                g.Key,
                Projects = g.GroupBy(p => p.ProjectId)
                            .Select(p => new BarChartInfoResponseDto
                            {
                                ProjectId = p.Key,
                                ProjectName = p.First().ProjectName,
                                ProjectColor = p.First().ProjectColor,
                                TotalTime = p.Sum(trackedTimeSelector)
                            })
                            .ToList(),
                TotalTimeInMinutes = g.Sum(trackedTimeSelector)
            })
            .ToList();

        reportPeriods
            .AddRange(periods
                .Where(p => !reportPeriods.Exists(x => x.Key == p))
                .Select(x => new
                {
                    Key = x,
                    Projects = new List<BarChartInfoResponseDto>(),
                    TotalTimeInMinutes = 0
                }));

        return reportPeriods
            .Select(x => new TimePerPeriodReportDto
            {
                Period = new DateTime(year, x.Key, 1, 0, 0, 0, DateTimeKind.Utc),
                TotalTimeInMinutes = x.TotalTimeInMinutes,
                Projects = x.Projects
            })
            .OrderBy(x => x.Period)
            .ToList();
    }

    private static List<TimePerPeriodReportDto> GroupBarChartInfoByDaysInMonth(
        List<BarChartInfoDto> barChartInfoList,
        int year, int month,
        Func<BarChartInfoDto, int> trackedTimeSelector)
    {
        var periods = Enumerable.Range(1, DateTime.DaysInMonth(year, month));

        var reportPeriods = barChartInfoList
            .Where(t => t.ReportYear == year && t.ReportMonthOfYear == month)
            .GroupBy(t => t.ReportForDate.Day)
            .Select(g => new
            {
                Day = g.Key,
                Projects = g.GroupBy(p => p.ProjectId)
                            .Select(p => new BarChartInfoResponseDto
                            {
                                ProjectId = p.Key,
                                ProjectName = p.First().ProjectName,
                                ProjectColor = p.First().ProjectColor,
                                TotalTime = p.Sum(trackedTimeSelector)
                            })
                            .ToList(),
                TotalTimeInMinutes = g.Sum(trackedTimeSelector)
            })
            .ToList();

        reportPeriods.AddRange(periods
            .Where(p => !reportPeriods.Exists(x => x.Day == p))
            .Select(missingDay => new
            {
                Day = missingDay,
                Projects = new List<BarChartInfoResponseDto>(),
                TotalTimeInMinutes = 0
            }));

        return reportPeriods
            .Select(x => new TimePerPeriodReportDto
            {
                Period = new DateTime(year, month, x.Day, 0, 0, 0, DateTimeKind.Utc),
                TotalTimeInMinutes = x.TotalTimeInMinutes,
                Projects = x.Projects
            })
            .OrderBy(x => x.Period)
            .ToList();
    }

    private static List<TimePerPeriodReportDto> GroupBarChartInfoByDaysInWeek(
        List<BarChartInfoDto> barChartInfoList,
        int year, int week,
        Func<BarChartInfoDto, int> trackedTimeSelector)
    {
        var weekDates = Enumerable.Range(0, 7)
            .Select(dayOfWeek => ISOWeek.ToDateTime(year, week, (DayOfWeek)dayOfWeek))
            .ToHashSet();

        var groupedData = barChartInfoList
            .Where(t => ISOWeek.GetYear(t.ReportForDate.ToDateTime(TimeOnly.MinValue)) == year &&
                        ISOWeek.GetWeekOfYear(t.ReportForDate.ToDateTime(TimeOnly.MinValue)) == week)
            .GroupBy(t => (int)t.ReportForDate.DayOfWeek)
            .Select(g => new
            {
                DayOfWeek = g.Key,
                Projects = g.GroupBy(p => p.ProjectId)
                            .Select(p => new BarChartInfoResponseDto
                            {
                                ProjectId = p.Key,
                                ProjectName = p.First().ProjectName,
                                ProjectColor = p.First().ProjectColor,
                                TotalTime = p.Sum(trackedTimeSelector)
                            })
                            .ToList(),
                TotalTimeInMinutes = g.Sum(trackedTimeSelector)
            })
            .ToList();

        var result = weekDates
            .Select(date => new
            {
                DayOfWeek = (int)date.DayOfWeek,
                Date = date,
                Projects = groupedData
                    .FirstOrDefault(g => g.DayOfWeek == (int)date.DayOfWeek)?.Projects ?? [],
                TotalTimeInMinutes = groupedData
                    .FirstOrDefault(g => g.DayOfWeek == (int)date.DayOfWeek)?.TotalTimeInMinutes ?? 0
            })
            .Select(x => new TimePerPeriodReportDto
            {
                Period = x.Date,
                TotalTimeInMinutes = x.TotalTimeInMinutes,
                Projects = x.Projects
            })
            .OrderBy(x => x.Period)
            .ToList();

        return result;
    }
}
