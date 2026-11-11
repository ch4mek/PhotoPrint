using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

internal sealed class GetTimeStackRequest : IQueryHandler<GetTimeStackRequest.Query, List<TimeStackCoreDto>>
{
    private readonly IGetAccessedDepartmentIdForCurrentUserBusinessRule _getAccessedDepartment;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimesheetQueryBuilder _timesheetQueryBuilder;
    public GetTimeStackRequest(
        IGetAccessedDepartmentIdForCurrentUserBusinessRule getAccessedDepartment,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimesheetQueryBuilder timesheetQueryBuilder)
    {
        _getAccessedDepartment = getAccessedDepartment;
        _authenticatedUserContext = authenticatedUserContext;
        _timesheetQueryBuilder = timesheetQueryBuilder;
    }

    public async Task<List<TimeStackCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessedDepartmentId = await _getAccessedDepartment.Execute(request.DepartmentId, cancellationToken);
        var groupManagersResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;

        return await _timesheetQueryBuilder
            .FromAsNoTracked(request.Year, request.MonthOfYear)
            .WithDepartmentFilter(accessedDepartmentId)
            .WithResourceGroupFilter(groupManagersResourceGroupId)
            .Build()
            .GroupBy(timesheet => timesheet.ReportForDate)
            .OrderBy(groupingResult => groupingResult.Key)
            .Select(orderResult => new TimeStackCoreDto(orderResult.Key, orderResult.Sum(e => e.FactTrackedInMinutes)))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int Year, int[] MonthOfYear, string? DepartmentId)
    : IQuery<List<TimeStackCoreDto>>;
}
