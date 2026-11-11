using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetUsersWithTotalTrackedTimeRequest
    : IQueryHandler<GetUsersWithTotalTrackedTimeRequest.Query, List<UserWithTrackedTimeReportDto>>
{
    private readonly IUserAccessService _userAccessService;
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetUsersWithTotalTrackedTimeRequest(
        IUserAccessService userAccessService,
        IAuthenticatedUserContext authenticatedUser,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _userAccessService = userAccessService;
        _authenticatedUser = authenticatedUser;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<List<UserWithTrackedTimeReportDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.UserId.HasValue &&
            !await _userAccessService.CanAccessUserReportInformation(request.UserId.Value, cancellationToken))
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
            TrackedTimeType = request.TrackedTimeType,
        };

        var timesheets = _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
            .ApplyTimesheetFilter(timesheetsFilter)
            .OrderBy(timesheet => timesheet.Id);

        var userIds = await timesheets.Select(t => t.UserId).Distinct().ToListAsync(cancellationToken);
        var users = await _timeTrackDbContext.TimeTrackUsers
            .AsNoTracking()
            .Where(user => userIds.Contains(user.UserId))
            .ToListAsync(cancellationToken);

        var timeTypeSelector = request.TrackedTimeType.GetTrackedTimeSelector();

        return users
            .Select(u => new UserWithTrackedTimeReportDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Color = u.Color,
                IsActive = u.IsActive,
                TotalTimeInMinutes = timesheets
                    .Where(t => t.UserId == u.UserId)
                    .Sum(timeTypeSelector)
            })
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList();
    }

    internal sealed record Query(
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            string? DepartmentId,
            int? ResourceGroupId,
            int? ClientId,
            int? ProjectId,
            int? UserId,
            TrackedTimeType TrackedTimeType)
        : IQuery<List<UserWithTrackedTimeReportDto>>, IPeriodRequest, ITimeTrackRequest;
}
