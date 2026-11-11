using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetPieChartUsersRequest : IQueryHandler<GetPieChartUsersRequest.Query, List<PieChartUsersCoreDto>>
{
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _getAccessedDepartment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
    private readonly ITimeTrackDbContext _dbContext;

    public GetPieChartUsersRequest(
        IGetAccessedDepartmentIdForCurrentUserBusinessRule getAccessedDepartment,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimesheetQueryBuilder timesheetQueryBuilder,
        ITimeTrackDbContext dbContext)
    {
        _getAccessedDepartment = getAccessedDepartment;
        _authenticatedUserContext = authenticatedUserContext;
        _timesheetQueryBuilder = timesheetQueryBuilder;
        _dbContext = dbContext;
    }

    public async Task<List<PieChartUsersCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessedDepartmentId = await _getAccessedDepartment.Execute(request.DepartmentId, cancellationToken);
        var groupManagersResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;

        var users = _dbContext.TimeTrackUsers.AsNoTracking()
            .Where(x => accessedDepartmentId == null || x.DepartmentId == accessedDepartmentId)
            .Where(x => groupManagersResourceGroupId == null || x.ResourceGroupId == groupManagersResourceGroupId);

        var query = _timesheetQueryBuilder
            .FromAsNoTracked(request.Year, request.MonthOfYear).Build();

        return await query
            .GroupBy(x => x.UserId, (userId, timesheets) =>
                new { UserId = userId, TrackedTime = timesheets.Sum(timesheet => timesheet.FactTrackedInMinutes) })
            .Where(userAndTime => userAndTime.TrackedTime != 0)
            .Join(users,
                userAndTime => userAndTime.UserId,
                user => user.UserId,
                (userAndTime, user) => new PieChartUsersCoreDto
                { 
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    Color = user.Color,
                    TrackedTime =  userAndTime.TrackedTime,
                    IsDeleted = user.IsActive
                }
            )
            .ToListAsync(cancellationToken);
    }
    internal sealed record Query(int? Year, int[] MonthOfYear, string? DepartmentId)
    : IQuery<List<PieChartUsersCoreDto>>;

}
