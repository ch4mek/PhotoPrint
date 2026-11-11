using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetDashboardTasksRequest : IQueryHandler<GetDashboardTasksRequest.Query, List<DashboardTasksCoreDto>>
{
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _getAccessedDepartment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetDashboardTasksRequest(
        IGetAccessedDepartmentIdForCurrentUserBusinessRule getAccessedDepartment,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _getAccessedDepartment = getAccessedDepartment;
        _authenticatedUserContext = authenticatedUserContext;
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<List<DashboardTasksCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessedDepartmentId = await _getAccessedDepartment.Execute(request.DepartmentId, cancellationToken);
        var groupManagersResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;

        var query = _timesheetQueryBuilder
            .FromAsNoTracked(request.Year, request.MonthOfYear)
            .WithDepartmentFilter(accessedDepartmentId)
            .WithResourceGroupFilter(groupManagersResourceGroupId)
            .WithUserFilter(request.UserId)
            .WithProjectFilter(request.ProjectId)
            .WithBillableHoursFilter(false)
            .Build();

        return await query.Select(t => new DashboardTasksCoreDto
        {
            TrackedInMinutes = t.FactTrackedInMinutes,
            ReportForDate = t.ReportForDate,
            Color = t.User!.Color,
            TaskName = t.TaskName
        })
        .OrderBy(task => task.ReportForDate)
        .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int UserId, int ProjectId, int Year, int[] MonthOfYear, string? DepartmentId)
    : IQuery<List<DashboardTasksCoreDto>>;

}
