using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetDashboardOverviewRequest : IQueryHandler<GetDashboardOverviewRequest.Query, DashboardOverviewCoreDto>
{
    private readonly IDashboardOverviewLoader _dashboardOverviewLoader;
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _getAccessedDepartment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    public GetDashboardOverviewRequest(
        IGetAccessedDepartmentIdForCurrentUserBusinessRule getAccessedDepartment,
        IAuthenticatedUserContext authenticatedUserContext,
        IDashboardOverviewLoader dashboardOverviewLoader)
    {
        _getAccessedDepartment = getAccessedDepartment;
        _authenticatedUserContext = authenticatedUserContext;
        _dashboardOverviewLoader = dashboardOverviewLoader;
    }

    public async Task<DashboardOverviewCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessedDepartmentId = await _getAccessedDepartment.Execute(request.DepartmentId, cancellationToken);
        var groupManagersResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;

        return await _dashboardOverviewLoader.LoadAsync(
            request.Year,
            request.MonthOfYear,
            request.WeekOfYear,
            accessedDepartmentId,
            groupManagersResourceGroupId,
            cancellationToken);
    }
    internal sealed record Query(int Year, int[] MonthOfYear, int? WeekOfYear, string? DepartmentId)
    : IQuery<DashboardOverviewCoreDto>;

}
