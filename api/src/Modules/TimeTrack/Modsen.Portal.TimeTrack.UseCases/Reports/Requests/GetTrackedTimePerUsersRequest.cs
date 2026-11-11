using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetTrackedTimePerUsersRequest
    : IQueryHandler<GetTrackedTimePerUsersRequest.Query, PageableListOfItems<TrackedTimePerUserReportDto>>
{
    private readonly IUserAccessService _userAccessService;
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IGetTimesheetsByPeriodWithFilterDbQuery _getTimesheetsDbQuery;
    public GetTrackedTimePerUsersRequest(
        IUserAccessService userAccessService,
        IAuthenticatedUserContext authenticatedUser,
        ITimeTrackDbContext timeTrackDbContext,
        IGetTimesheetsByPeriodWithFilterDbQuery getTimesheetsDbQuery)
    {
        _userAccessService = userAccessService;
        _authenticatedUser = authenticatedUser;
        _timeTrackDbContext = timeTrackDbContext;
        _getTimesheetsDbQuery = getTimesheetsDbQuery;
    }

    public async Task<PageableListOfItems<TrackedTimePerUserReportDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.UserId.HasValue && !await _userAccessService.CanAccessUserReportInformation(request.UserId.Value, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.CannotAccessOtherUserData);
        }

        var timesheetsFilter = new TimesheetFilter
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

        var dbRequest = new GetTimesheetsByPeriodWithFilterDbRequest(request.Year, request.MonthOfYear,
            request.WeekOfYear,
            timesheetsFilter);

        var timesheets = await _getTimesheetsDbQuery.ExecuteAsync(dbRequest, cancellationToken);

        var isPublicProject = request.ProjectId.HasValue
            && (await _timeTrackDbContext.Projects
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId.Value, cancellationToken))!.IsPublic;

        var isClientWithAnyPublicProject = false;
        if (request.ClientId.HasValue)
        {
            var projects = await _timeTrackDbContext.Projects
                .Where(x => x.ClientId == request.ClientId.Value)
                .ToListAsync(cancellationToken);
            isClientWithAnyPublicProject = projects.Exists(p => p.IsPublic);
        }

        var usersFilter = new FilterParams
        {
            UserId = timesheetsFilter.UserId,
            DepartmentId = timesheetsFilter.DepartmentId,
            ClientId = timesheetsFilter.ClientId,
            ProjectId = timesheetsFilter.ProjectId,
            ResourceGroupId = timesheetsFilter.ResourceGroupId
        };

        if (!isPublicProject && !isClientWithAnyPublicProject)
        {
            usersFilter.ClientId = timesheetsFilter.ClientId;
            usersFilter.ProjectId = timesheetsFilter.ProjectId;
        }

        var pageableUsers = await GetPageableUsersForReportsAsync(
            request.Page,
            request.PageSize,
            timesheets.Select(t => t.UserId).Distinct().ToList(),
            usersFilter,
            cancellationToken);

        var trackedTimeSelector = request.TrackedTimeType.GetTrackedTimeSelector();

        return new PageableListOfItems<TrackedTimePerUserReportDto>
        {
            Page = pageableUsers.Page,
            PageSize = pageableUsers.PageSize,
            Total = pageableUsers.Total,
            Items = pageableUsers.Items
                .Select(u => new TrackedTimePerUserReportDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Color = u.Color,
                    IsActive = u.IsActive,
                    TotalTimeInMinutes = timesheets
                        .Where(t => t.UserId == u.UserId)
                        .Sum(trackedTimeSelector),
                    ProjectsCount = timesheets
                        .Where(t => t.UserId == u.UserId &&
                        (request.TrackedTimeType is TrackedTimeType.Fact && t.FactTrackedInMinutes > 0 ||
                         request.TrackedTimeType is TrackedTimeType.Billable && t.BillableTrackedInMinutes > 0))
                        .Select(t => t.ProjectId)
                        .Distinct().Count()
                }).ToArray()
        };
    }

    internal sealed record Query : IQuery<PageableListOfItems<TrackedTimePerUserReportDto>>, IPeriodRequest, ITimeTrackRequest
    {
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public int? Year { get; init; }
        public int[]? MonthOfYear { get; init; } = [];
        public int? WeekOfYear { get; init; }
        public string? DepartmentId { get; init; }
        public int? ResourceGroupId { get; init; }
        public int? UserId { get; init; }
        public int? ClientId { get; init; }
        public int? ProjectId { get; init; }
        public TrackedTimeType TrackedTimeType { get; set; }
    }

    internal sealed class Validator : PeriodValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0);
        }
    }

    private async Task<PageableListOfItems<TimeTrackUserDto>> GetPageableUsersForReportsAsync(
        int page,
        int pageSize,
        IReadOnlyCollection<int> userIds,
        FilterParams unionEmptyReportsParams,
        CancellationToken token)
    {
        var usersWithTrackedTime = _timeTrackDbContext.TimeTrackUsers
            .AsNoTracking()
            .Where(user => userIds.Contains(user.UserId));

        var usersWithoutTrackedTime = GetFilteredUsersQuery(unionEmptyReportsParams)
            .Where(u => !usersWithTrackedTime.Any(r => r.UserId == u.UserId));

        var allReportUsers = usersWithTrackedTime
            .Union(usersWithoutTrackedTime);

        var pageableUsers = (await allReportUsers
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token))
            .Select(source => new TimeTrackUserDto()
            {
                UserId = source.UserId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DepartmentId = source.DepartmentId,
                ResourceGroupId = source.ResourceGroupId,
                Color = source.Color,
                AvatarFileId = source.AvatarFileId,
                IsActive = source.IsActive,
            })
            .ToList();

        var totalCount = await allReportUsers.CountAsync(token);
        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<TimeTrackUserDto>.Value;
        }

        return pageableUsers.ToPageable()
            .WithPaging(page, pageSize)
            .WithTotal(totalCount);
    }

    private IQueryable<TimeTrackUser> GetFilteredUsersQuery(FilterParams filterParams)
    {
        var query = _timeTrackDbContext.TimeTrackUsers
            .AsNoTracking();

        query = filterParams.IsActive is not null ?
            query.Where(user => user.IsActive == filterParams.IsActive) :
            query;

        query = ApplyUsersFilter(query, filterParams);

        return query;
    }

    private static IQueryable<TimeTrackUser> ApplyUsersFilter(IQueryable<TimeTrackUser> query, FilterParams filterParams)
    {
        if (filterParams.UserId.HasValue)
        {
            query = query.Where(user => user.UserId == filterParams.UserId);
        }

        if (filterParams.DepartmentId is not null)
        {
            query = query.Where(user => user.DepartmentId == filterParams.DepartmentId);
        }

        if (filterParams.ClientId.HasValue)
        {
            query = query.Where(user => user.Projects.Any(proj => proj.Project!.ClientId == filterParams.ClientId));
        }

        if (filterParams.ProjectId.HasValue)
        {
            query = query.Where(user => user.Projects.Any(proj => proj.ProjectId == filterParams.ProjectId));
        }

        if (filterParams.ResourceGroupId.HasValue)
        {
            query = query.Where(user => filterParams.ResourceGroupId == 0
                ? user.ResourceGroupId == null
                : user.ResourceGroupId == filterParams.ResourceGroupId);
        }

        return query;
    }
}
