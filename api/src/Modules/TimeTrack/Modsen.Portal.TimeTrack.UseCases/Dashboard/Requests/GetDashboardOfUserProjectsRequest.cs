using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetDashboardOfUserProjectsRequest
    : IQueryHandler<GetDashboardOfUserProjectsRequest.Query, List<DashboardOfUserProjectsCoreDto>>
{

    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _getAccessedDepartment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetDashboardOfUserProjectsRequest(
        IGetAccessedDepartmentIdForCurrentUserBusinessRule getAccessedDepartment,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _getAccessedDepartment = getAccessedDepartment;
        _authenticatedUserContext = authenticatedUserContext;
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<List<DashboardOfUserProjectsCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessedDepartmentId = await _getAccessedDepartment.Execute(request.DepartmentId, cancellationToken);
        var groupManagersResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;

        var query = _timesheetQueryBuilder
            .FromAsNoTracked(request.Year, request.MonthOfYear)
            .WithDepartmentFilter(accessedDepartmentId)
            .WithResourceGroupFilter(groupManagersResourceGroupId)
            .WithUserFilter(request.UserId)
            .WithBillableHoursFilter(false)
            .Build();

        var result = await query
            .GroupBy(timesheet =>
                new
                {
                    timesheet.ProjectId,
                    ProjectName = timesheet.Project!.Name,
                    timesheet.Project.ClientId,
                    ClientName = timesheet.Project.Client!.Name,
                    ProjectColor = timesheet.Project.Color,
                    UserColor = default(string),
                    IsUserActive = timesheet.User!.IsActive
                })
            .Select(groupingResult => new DashboardOfUserProjectsCoreDto
                {
                    ProjectId = groupingResult.Key.ProjectId,
                    ProjectName = groupingResult.Key.ProjectName,
                    UserColor = groupingResult.Key.UserColor,
                    TotalTrackedInMinutes = groupingResult.Sum(x => x.FactTrackedInMinutes),
                    TasksCount = groupingResult.Count(),
                    ClientId = groupingResult.Key.ClientId,
                    ClientName = groupingResult.Key.ClientName,
                    ProjectColor = groupingResult.Key.ProjectColor,
                    UserIsActive = groupingResult.Key.IsUserActive
                })
            .ToListAsync(cancellationToken);

        return result;
    }

    internal sealed record Query(int UserId, int Year, int[] MonthOfYear, string? DepartmentId)
    : IQuery<List<DashboardOfUserProjectsCoreDto>>;
}
