using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using TimesheetSnapshotCoreDto = Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto.TimesheetSnapshotCoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Loaders;

internal interface IPageableUserTimesheetSnapshotsLoader
{
    Task<PageableListOfItems<UserTimesheetsSnapshotCoreDto>> LoadAsync(GetTimesheetsDto request,
        CancellationToken cancellationToken);
}

internal class PageableUserTimesheetSnapshotsLoader : IPageableUserTimesheetSnapshotsLoader
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    public PageableUserTimesheetSnapshotsLoader(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<PageableListOfItems<UserTimesheetsSnapshotCoreDto>> LoadAsync(GetTimesheetsDto request,
        CancellationToken cancellationToken)
    {
        var inactiveGroups = await _timeTrackDbContext.TimeTrackResourceGroups
            .Where(g => !g.IsActive)
            .Select(g => g.ResourceGroupId).ToListAsync(cancellationToken);

        var timesheetsQuery = GetTimesheetsQuery(request, inactiveGroups);
        var query = await GetUserTimesheetsQueryAsync(timesheetsQuery, request, inactiveGroups, cancellationToken);

        var resultWithoutPaging = GroupQuery(query);
        var result = await resultWithoutPaging
            .Skip(request.Skip)
            .Take(request.Top)
            .ToListAsync(cancellationToken);

        var totalCount = await resultWithoutPaging.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<UserTimesheetsSnapshotCoreDto>.Value;
        }

        return result.ToPageable()
                .WithPaging((request.Skip / request.Top) + 1, request.Top)
                .WithTotal(totalCount);
    }

    private IQueryable<Timesheet> GetTimesheetsQuery(GetTimesheetsDto request, List<int> inactiveGroupsIds)
    {
        var query = _timeTrackDbContext.Timesheets
            .Include(timesheet => timesheet.Project)
            .Include(timesheet => timesheet.User)
            .Where(timesheet => timesheet.ReportForDate >= request.ReportForDateFrom
                    && timesheet.ReportForDate <= request.ReportForDateTo);

        query = request.ProjectId is not null
            ? query.Where(t => t.ProjectId == request.ProjectId)
            : query;

        query = request.ClientId is not null
            ? query.Where(t => t.Project!.ClientId == request.ClientId)
            : query;

        query = request.UserId is not null
            ? query.Where(t => t.UserId == request.UserId)
            : query;

        query = request.DepartmentId is not null
            ? query.Where(t => t.User!.DepartmentId == request.DepartmentId)
            : query;

        if (request.ResourceGroupId is not null)
        {
            query = request.ResourceGroupId == 0
                ? query.Where(timesheet => timesheet.User!.ResourceGroupId == null || inactiveGroupsIds.Any(group => group == timesheet.User!.ResourceGroupId))
                : query.Where(timesheet => timesheet.User!.ResourceGroupId == request.ResourceGroupId);
        }

        query = request.ManagerId is not null
            ? query.Where(t => t.Project!.ManagerUserId == request.ManagerId)
            : query;

        return query;
    }

    private async Task<IQueryable<GroupedUserTimesheetCoreDto>> GetUserTimesheetsQueryAsync(IQueryable<Timesheet> timesheetsQuery,
        GetTimesheetsDto request, List<int> inactiveGroupsIds,
        CancellationToken cancellationToken)
    {
        var timesheetIds = await GetTimesheetIdsAsync(timesheetsQuery, cancellationToken);

        var baseQuery = GetBaseUserQuery();
        var query = baseQuery.Where(u => u.Timesheets.Any(timesheet => timesheetIds.Contains(timesheet.Id)));

        query = query.Union(FilterUsersQuery(baseQuery, request, inactiveGroupsIds));
        query = await UnionSpecificUserWhenManagerHasPublicProjectsAsync(query, baseQuery, request, cancellationToken);
        query = ApplyActiveOrHasTimesheetsInRangeFilter(query, request);

        return BuildGroupedQuery(query, timesheetsQuery, request);
    }

    private static Task<List<int>> GetTimesheetIdsAsync(
        IQueryable<Timesheet> timesheetsQuery,
        CancellationToken cancellationToken)
    {
        return timesheetsQuery
            .Select(timesheet => timesheet.Id)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<TimeTrackUser> GetBaseUserQuery()
    {
        return _timeTrackDbContext.TimeTrackUsers
            .Include(user => user.Projects)
            .ThenInclude(userProject => userProject.Project)
            .Include(user => user.Timesheets)
            .Include(user => user.TimesheetSubmitStatuses)
            .AsQueryable();
    }

    private async Task<IQueryable<TimeTrackUser>> UnionSpecificUserWhenManagerHasPublicProjectsAsync(
        IQueryable<TimeTrackUser> query,
        IQueryable<TimeTrackUser> baseQuery,
        GetTimesheetsDto request,
        CancellationToken cancellationToken)
    {
        if (!ShouldUnionSpecificUserWhenManagerHasPublicProjects(request))
        {
            return query;
        }

        var isAnyPublicProjectManagedByUser = await _timeTrackDbContext.Projects
            .AnyAsync(project => project.ManagerUserId == request.ManagerId && project.IsPublic, cancellationToken);

        return isAnyPublicProjectManagedByUser
            ? query.Union(baseQuery.Where(user => user.UserId == request.UserId))
            : query;
    }

    private static bool ShouldUnionSpecificUserWhenManagerHasPublicProjects(GetTimesheetsDto request)
    {
        return request.ManagerId is not null
            && request.UserId is not null
            && request.ProjectId is null
            && request.ClientId is null;
    }

    private static IQueryable<TimeTrackUser> ApplyActiveOrHasTimesheetsInRangeFilter(
        IQueryable<TimeTrackUser> query,
        GetTimesheetsDto request)
    {
        return query.Where(u => u.IsActive || !u.IsActive && u.Timesheets.Any(t =>
            t.ReportForDate >= request.ReportForDateFrom
            && t.ReportForDate <= request.ReportForDateTo));
    }

    private static IQueryable<GroupedUserTimesheetCoreDto> BuildGroupedQuery(
        IQueryable<TimeTrackUser> query,
        IQueryable<Timesheet> timesheetsQuery,
        GetTimesheetsDto request)
    {
        var isExactFullMonth = IsExactFullMonth(request.ReportForDateFrom, request.ReportForDateTo);
        var month = request.ReportForDateFrom.Month;
        var year = request.ReportForDateFrom.Year;

        return query
            .GroupJoin(
                timesheetsQuery,
                user => user.UserId,
                timesheet => timesheet.UserId,
                (user, timesheets) => new { user, timesheets })
            .SelectMany(
                group => group.timesheets.DefaultIfEmpty(),
                (group, timesheet) => new GroupedUserTimesheetCoreDto
                {
                    UserId = group.user.UserId,
                    UserFullName = group.user.LastName + " " + group.user.FirstName,
                    Position = group.user.Position,
                    DepartmentId = group.user.DepartmentId,
                    ResourceGroupId = group.user.ResourceGroupId,
                    UserColor = group.user.Color,
                    UserIsActive = group.user.IsActive,
                    TimesheetId = timesheet == null ? null : timesheet.Id,
                    TaskName = timesheet == null ? null : timesheet.TaskName,
                    FactTrackedInMinutes = timesheet == null ? null : timesheet.FactTrackedInMinutes,
                    BillableTrackedInMinutes = timesheet == null ? null : timesheet.BillableTrackedInMinutes,
                    ReportForDate = timesheet == null ? null : timesheet.ReportForDate,
                    Status = isExactFullMonth
                             && group.user.TimesheetSubmitStatuses.Any(status => status.Month == month && status.Year == year)
                        ? group.user.TimesheetSubmitStatuses.First(status => status.Month == month && status.Year == year).Status
                        : null
                });
    }

    private static IQueryable<TimeTrackUser> FilterUsersQuery(IQueryable<TimeTrackUser> query,
        GetTimesheetsDto request, List<int> inactiveGroupsIds)
    {
        query = ApplyManagerFilter(query, request);
        query = ApplyClientFilter(query, request);
        query = ApplyProjectFilter(query, request);
        query = ApplyUserFilter(query, request);
        query = ApplyDepartmentFilter(query, request);
        query = ApplyResourceGroupFilter(query, request, inactiveGroupsIds);
        query = ApplyAssignedForSelectedDateFilter(query, request);

        return query;
    }

    private static IQueryable<TimeTrackUser> ApplyManagerFilter(IQueryable<TimeTrackUser> query, GetTimesheetsDto request)
    {
        return request.ManagerId is not null
            ? query.Where(user => user.Projects.Any(projectUser =>
                projectUser.Project!.ManagerUserId == request.ManagerId && !projectUser.Project!.IsPublic))
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyClientFilter(IQueryable<TimeTrackUser> query, GetTimesheetsDto request)
    {
        return request.ClientId is not null
            ? query.Where(user => user.Projects.Any(projectUser =>
                projectUser.Project!.ClientId == request.ClientId
                && !projectUser.Project!.IsPublic
                && (request.ManagerId == null || projectUser.Project!.ManagerUserId == request.ManagerId)))
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyProjectFilter(IQueryable<TimeTrackUser> query, GetTimesheetsDto request)
    {
        return request.ProjectId is not null
            ? query.Where(user => user.Projects.Any(projectUser =>
                projectUser.ProjectId == request.ProjectId
                && !projectUser.Project!.IsPublic
                && (request.ManagerId == null || projectUser.Project!.ManagerUserId == request.ManagerId)))
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyUserFilter(IQueryable<TimeTrackUser> query, GetTimesheetsDto request)
    {
        return request.UserId is not null
            ? query.Where(user => user.UserId == request.UserId)
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyDepartmentFilter(IQueryable<TimeTrackUser> query, GetTimesheetsDto request)
    {
        return request.DepartmentId is not null
            ? query.Where(user => user.DepartmentId == request.DepartmentId)
            : query;
    }

    private static IQueryable<TimeTrackUser> ApplyResourceGroupFilter(
        IQueryable<TimeTrackUser> query,
        GetTimesheetsDto request,
        List<int> inactiveGroupsIds)
    {
        if (request.ResourceGroupId is null)
        {
            return query;
        }

        return request.ResourceGroupId == 0
            ? query.Where(user => user.ResourceGroupId == null || inactiveGroupsIds.Any(group => group == user.ResourceGroupId))
            : query.Where(user => user.ResourceGroupId == request.ResourceGroupId);
    }

    private static IQueryable<TimeTrackUser> ApplyAssignedForSelectedDateFilter(IQueryable<TimeTrackUser> query, GetTimesheetsDto request)
    {
        return request.AssignedForSelectedDate.HasValue && request.AssignedForSelectedDate.Value
            ? query.Where(user => user.Projects.Any(projectUser =>
                (request.ProjectId == null || projectUser.ProjectId == request.ProjectId)
                && (request.ClientId == null || projectUser.Project!.ClientId == request.ClientId)
                && (projectUser.UserOnProjectTo == null || projectUser.UserOnProjectTo >= request.ReportForDateFrom)
                && (projectUser.UserOnProjectFrom <= request.ReportForDateTo)))
            : query;
    }

    private static IQueryable<UserTimesheetsSnapshotCoreDto> GroupQuery(IQueryable<GroupedUserTimesheetCoreDto> query)
    {
        return query.GroupBy(t => new
        {
            t.UserId,
            t.UserFullName,
            t.UserColor,
            t.UserIsActive,
            t.DepartmentId,
            t.ResourceGroupId,
            t.Status,
            t.Position
        })
            .Select(group => new UserTimesheetsSnapshotCoreDto
            {
                UserId = group.Key.UserId,
                Color = group.Key.UserColor,
                UserName = group.Key.UserFullName,
                Position = group.Key.Position,
                IsActive = group.Key.UserIsActive,
                UserDepartmentId = group.Key.DepartmentId!,
                UserResourceGroupId = group.Key.ResourceGroupId,
                TotalFactTime = group.Any(t => t.TimesheetId != null) ? group.Sum(t => t.FactTrackedInMinutes!.Value) : 0,
                TotalBillableTime = group.Any(t => t.TimesheetId != null) ? group.Sum(t => t.BillableTrackedInMinutes!.Value) : 0,
                Status = group.Key.Status,
                Timesheets = group.Any(t => t.TimesheetId != null)
                    ? group.Select(t => new TimesheetSnapshotCoreDto
                    {
                        Id = t.TimesheetId,
                        ReportForDate = t.ReportForDate,
                        FactTrackedInMinutes = t.FactTrackedInMinutes,
                        BillableTrackedInMinutes = t.BillableTrackedInMinutes
                    }).ToList()
                    : new List<TimesheetSnapshotCoreDto>()
            }).OrderBy(user => user.UserName);
    }

    private static bool IsExactFullMonth(DateOnly startDate, DateOnly endDate)
    {
        if (startDate.Day != 1)
        {
            return false;
        }

        var lastDayOfMonth = startDate.AddMonths(1).AddDays(-1);

        return endDate == lastDayOfMonth;
    }

}
