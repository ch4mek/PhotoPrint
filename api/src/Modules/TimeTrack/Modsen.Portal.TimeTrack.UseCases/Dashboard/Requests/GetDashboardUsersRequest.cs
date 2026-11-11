using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetDashboardUsersRequest : IQueryHandler<GetDashboardUsersRequest.Query, DashboardUsersCoreDto>
{
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _getAccessedDepartment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IDashboardUsersLoader _dashboardUsersLoader;
    public GetDashboardUsersRequest(
        IGetAccessedDepartmentIdForCurrentUserBusinessRule getAccessedDepartment,
        IAuthenticatedUserContext authenticatedUserContext,
        IDashboardUsersLoader dashboardUsersLoader)
    {
        _getAccessedDepartment = getAccessedDepartment;
        _authenticatedUserContext = authenticatedUserContext;
        _dashboardUsersLoader = dashboardUsersLoader;
    }

    public async Task<DashboardUsersCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessedDepartmentId = await _getAccessedDepartment.Execute(request.DepartmentId, cancellationToken);
        var groupManagersResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;

        return await _dashboardUsersLoader.LoadAsync(
            request.PageNo,
            request.RowsCount,
            request.Year,
            request.MonthOfYear,
            accessedDepartmentId,
            groupManagersResourceGroupId,
            cancellationToken);
    }

    

    internal sealed record Query(int Year, int[] MonthOfYear, int PageNo, int RowsCount, string? DepartmentId)
    : IQuery<DashboardUsersCoreDto>;

}
