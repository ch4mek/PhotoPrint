using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.TimeTrack.Contract.ContractDto;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Queries.Core.Dto;

namespace Modsen.Portal.TimeTrack.UseCases.Queries;

internal sealed class GetTrackedTimePayloadForUsersUseCase(
    ITimeTrackDbContext timeTrackDbContext,
    IClockService clockService,
    IIdentityContract identityContract) : IQueryHandler<GetTrackedTimePayloadForUsersUseCase.Query, PayloadContractDto>
{
    public async Task<PayloadContractDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var keyword = BuildKeyword(request.ProjectName);
        var assignments = await LoadAssignmentsAsync(request, keyword, cancellationToken);
        var timesheetRows = await LoadTimesheetRowsAsync(request, keyword, cancellationToken);
        EnsureAssignmentPairsInTimesheetData(assignments, timesheetRows);

        IReadOnlyDictionary<int, DateRangeDto[]>? usersActivePeriodsInDepartments = null;
        if (request.DepartmentIds != null)
        {
            usersActivePeriodsInDepartments = request.DepartmentIds.Count != 0
                ? await identityContract
                    .GetUsersActivePeriodsInDepartments(request.UserIds, request.DepartmentIds, cancellationToken)
                : request.UserIds.ToDictionary(
                    user => user,
                    user => Array.Empty<DateRangeDto>());
        }

        var userTimesheetDetails = BuildUserTimesheetDetails(assignments, timesheetRows, usersActivePeriodsInDepartments);
        var projectIds = userTimesheetDetails.Select(x => x.ProjectId).Distinct().ToArray();
        var projects = await LoadProjectsInfo(projectIds, cancellationToken);
        var clients = await LoadClientsInfo(projects, cancellationToken);

        return new PayloadContractDto
        {
            Year = request.Year,
            Projects = projects,
            Clients = clients,
            Users = BuildUsers(userTimesheetDetails, assignments)
        };
    }

    private static string? BuildKeyword(string? projectName)
    {
        return projectName == null ? null : $"%{projectName.Trim()}%";
    }

    private async Task<List<AssignmentRow>> LoadAssignmentsAsync(Query request, string? keyword, CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(clockService.UtcNow);
        var minDate = DateOnly.FromDateTime(DateTime.MinValue);

        return await timeTrackDbContext.ProjectUsers
            .Where(projectUser => !projectUser.Project!.IsPublic)
            .Where(up => request.UserIds.Contains(up.UserId) && !up.Project!.IsPublic &&
                (up.UserOnProjectFrom == minDate || up.UserOnProjectFrom.Year <= request.Year) &&
                (
                    (up.UserOnProjectTo == minDate || up.UserOnProjectTo == null) ||
                    up.UserOnProjectTo.HasValue && up.UserOnProjectTo.Value.Year >= request.Year
                ))
            .Where(up => keyword == null || EF.Functions.Like(up.Project!.Name, keyword))
            .OrderByDescending(up => up.UserOnProjectFrom)
            .Select(up => new AssignmentRow
            {
                UserId = up.UserId,
                ProjectId = up.ProjectId,
                UserOnProjectFrom = up.UserOnProjectFrom,
                UserOnProjectTo = up.UserOnProjectTo,
                IsActive = currentDate <= up.UserOnProjectFrom ||
                    (currentDate >= up.UserOnProjectFrom &&
                    (up.UserOnProjectTo == null || currentDate <= up.UserOnProjectTo))
            })
            .ToListAsync(cancellationToken);
    }

    private Task<List<TimesheetRow>> LoadTimesheetRowsAsync(Query request, string? keyword, CancellationToken cancellationToken)
    {
        return timeTrackDbContext.Timesheets
            .Where(timesheet => request.UserIds.Contains(timesheet.UserId))
            .Where(timesheet => timesheet.ReportYear == request.Year)
            .Where(timesheet => !timesheet.Project!.IsPublic)
            .Where(timesheet => keyword == null || EF.Functions.Like(timesheet.Project!.Name, keyword))
            .GroupBy(timesheet => new { timesheet.ReportMonthOfYear, timesheet.ReportForDate, timesheet.UserId, timesheet.ProjectId })
            .Select(group => new TimesheetRow
            {
                UserId = group.Key.UserId,
                ProjectId = group.Key.ProjectId,
                ReportForDate = group.Key.ReportForDate,
                MonthOfYear = group.Key.ReportMonthOfYear,
                TotalFactTrackedInMinutesByProject = group.Sum(x => x.FactTrackedInMinutes),
                TotalBillableTrackedInMinutesByProject = group.Sum(x => x.BillableTrackedInMinutes)
            })
            .ToListAsync(cancellationToken);
    }

    private static void EnsureAssignmentPairsInTimesheetData(
        IReadOnlyCollection<AssignmentRow> assignments,
        List<TimesheetRow> timesheetRows)
    {
        foreach (var assignment in assignments)
        {
            var hasTimesheet = timesheetRows.Any(timesheet =>
                timesheet.UserId == assignment.UserId &&
                timesheet.ProjectId == assignment.ProjectId);

            if (!hasTimesheet)
            {
                timesheetRows.Add(new TimesheetRow
                {
                    UserId = assignment.UserId,
                    ProjectId = assignment.ProjectId,
                    ReportForDate = new DateOnly(),
                    MonthOfYear = 0,
                    TotalFactTrackedInMinutesByProject = 0,
                    TotalBillableTrackedInMinutesByProject = 0
                });
            }
        }
    }

    private static List<UserTimesheetDetailRow> BuildUserTimesheetDetails(
        IReadOnlyCollection<AssignmentRow> assignments,
        IReadOnlyCollection<TimesheetRow> timesheetRows,
        IReadOnlyDictionary<int, DateRangeDto[]>? usersActivePeriodsInDepartments)
    {
        return assignments
            .Join(
                timesheetRows,
                assignment => new { assignment.UserId, assignment.ProjectId },
                timesheet => new { timesheet.UserId, timesheet.ProjectId },
                (assignment, timesheet) => new JoinRow
                {
                    UserOnProjectFrom = assignment.UserOnProjectFrom,
                    UserOnProjectTo = assignment.UserOnProjectTo,
                    UserId = timesheet.UserId,
                    ProjectId = timesheet.ProjectId,
                    MonthOfYear = timesheet.MonthOfYear,
                    ReportForDate = timesheet.ReportForDate,
                    TotalBillableTrackedInMinutesByProject = timesheet.TotalBillableTrackedInMinutesByProject,
                    TotalFactTrackedInMinutesByProject = timesheet.TotalFactTrackedInMinutesByProject
                })
            .Where(x =>
            {
                // no filter
                if (usersActivePeriodsInDepartments == null)
                {
                    return true;
                }

                // user does't have activity ranges in selected departments
                if (!usersActivePeriodsInDepartments.TryGetValue(x.UserId, out var ranges) ||
                    ranges == null || ranges.Length == 0)
                {
                    return false;
                }

                return true;
            })
            .GroupBy(joinResult => new
            {
                joinResult.UserOnProjectFrom,
                joinResult.UserOnProjectTo,
                joinResult.MonthOfYear,
                joinResult.UserId,
                joinResult.ProjectId
            })
            .Select(group =>
            {
                var validRows = group
                    .Where(x => x.UserOnProjectFrom <= x.ReportForDate)
                    .Where(x => x.UserOnProjectTo == null || x.UserOnProjectTo >= x.ReportForDate)
                    .Where(x => IsUserInDepartmentDuringReportPeriod(x.UserId, usersActivePeriodsInDepartments, x.ReportForDate));

                return new UserTimesheetDetailRow
                {
                    UserId = group.Key.UserId,
                    ProjectId = group.Key.ProjectId,
                    MonthOfYear = group.Key.MonthOfYear,
                    UserOnProjectFrom = group.Key.UserOnProjectFrom,
                    UserOnProjectTo = group.Key.UserOnProjectTo,
                    TotalFactTrackedInMinutesByProject = validRows.Sum(x => x.TotalFactTrackedInMinutesByProject),
                    TotalBillableTrackedInMinutesByProject = validRows.Sum(x => x.TotalBillableTrackedInMinutesByProject)
                };
            })
            .ToList();
    }

    private static PayloadUserContractDto[] BuildUsers(
        IReadOnlyCollection<UserTimesheetDetailRow> userTimesheetDetails,
        IReadOnlyCollection<AssignmentRow> assignments)
    {
        return userTimesheetDetails
            .GroupBy(details => details.UserId)
            .Select(userGroup => new PayloadUserContractDto
            {
                UserId = userGroup.Key,
                HourDetails = BuildHourDetails(userGroup, assignments)
            })
            .ToArray();
    }

    private static PayloadUserMonthHoursContractDto[] BuildHourDetails(
        IGrouping<int, UserTimesheetDetailRow> userGroup,
        IReadOnlyCollection<AssignmentRow> assignments)
    {
        return userGroup
            .GroupBy(x => x.MonthOfYear)
            .Select(monthHoursGroup => new PayloadUserMonthHoursContractDto
            {
                MonthOfYear = monthHoursGroup.Key,
                TotalBillableTrackedInMinutes = monthHoursGroup.Sum(x => x.TotalBillableTrackedInMinutesByProject),
                TotalFactTrackedInMinutes = monthHoursGroup.Sum(x => x.TotalFactTrackedInMinutesByProject),
                Projects = BuildProjectHours(userGroup.Key, monthHoursGroup, assignments)
            })
            .ToArray();
    }

    private static PayloadProjectHoursContractDto[] BuildProjectHours(
        int userId,
        IEnumerable<UserTimesheetDetailRow> monthHoursGroup,
        IReadOnlyCollection<AssignmentRow> assignments)
    {
        return monthHoursGroup
            .Select(projectGroup => new PayloadProjectHoursContractDto
            {
                UserOnProjectFrom = projectGroup.UserOnProjectFrom,
                UserOnProjectTo = projectGroup.UserOnProjectTo,
                IsActive = ResolveIsActive(assignments, userId, projectGroup.ProjectId),
                ProjectId = projectGroup.ProjectId,
                TotalBillableTrackedInMinutes = projectGroup.TotalBillableTrackedInMinutesByProject,
                TotalFactTrackedInMinutes = projectGroup.TotalFactTrackedInMinutesByProject
            })
            .ToArray();
    }

    private static bool ResolveIsActive(
        IEnumerable<AssignmentRow> assignments,
        int userId,
        int projectId)
    {
        return assignments
            .Where(a => a.ProjectId == projectId && a.UserId == userId)
            .Select(a => a.IsActive)
            .FirstOrDefault();
    }

    private static bool IsUserInDepartmentDuringReportPeriod(
        int userId,
        IReadOnlyDictionary<int, DateRangeDto[]>? usersActivePeriodsInDepartments,
        DateOnly reportForDate)
    {
        if (usersActivePeriodsInDepartments == null)
        {
            return true;
        }

        usersActivePeriodsInDepartments.TryGetValue(userId, out var ranges);

        foreach (var r in ranges!)
        {
            if (r.Start <= reportForDate && r.End >= reportForDate)
            {
                return true;
            }
        }

        return false;
    }

    private Task<PayloadClientContractDto[]> LoadClientsInfo(PayloadProjectContractDto[] projects, CancellationToken cancellationToken)
    {
        var clientIds = projects.Select(x => x.ClientId).Distinct().ToArray();
        return timeTrackDbContext.Clients
            .Where(client => clientIds.Contains(client.Id))
            .Select(client => new PayloadClientContractDto
            {
                Name = client.Name,
                ClientId = client.Id,
                Color = client.Color,
            })
            .ToArrayAsync(cancellationToken);
    }

    private Task<PayloadProjectContractDto[]> LoadProjectsInfo(int[] projectIds, CancellationToken cancellationToken)
    {
        return timeTrackDbContext.Projects
            .Where(project => projectIds.Contains(project.Id))
            .Select(project => new PayloadProjectContractDto
            {
                ProjectId = project.Id,
                Name = project.Name,
                ClientId = project.ClientId
            })
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Query(IReadOnlyCollection<int> UserIds, int Year, string? ProjectName, IReadOnlyCollection<string>? DepartmentIds) : IQuery<PayloadContractDto>, ITimeTrackRequest;
}
