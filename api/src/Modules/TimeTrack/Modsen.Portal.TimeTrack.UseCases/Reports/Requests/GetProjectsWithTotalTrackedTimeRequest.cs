using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetProjectsWithTotalTrackedTimeRequest
    : IQueryHandler<GetProjectsWithTotalTrackedTimeRequest.Query, List<ProjectWithTrackedTimeReportDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IProjectsWithTotalTrackedTimeResolver _projectsWithTotalTrackedTimeResolver;

    public GetProjectsWithTotalTrackedTimeRequest(
        IAuthenticatedUserContext authenticatedUser,
        IProjectsWithTotalTrackedTimeResolver projectsWithTotalTrackedTimeResolver)
    {
        _authenticatedUser = authenticatedUser;
        _projectsWithTotalTrackedTimeResolver = projectsWithTotalTrackedTimeResolver;
    }

    public async Task<List<ProjectWithTrackedTimeReportDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var resolverRequest = new ProjectsWithTotalTrackedTimeResolverRequest
        (
            request.Year,
            request.MonthOfYear,
            request.WeekOfYear,
            request.DepartmentId,
            request.ClientId,
            request.ProjectId,
            request.UserId,
            request.ResourceGroupId,
            request.TrackedTimeType
        );

        resolverRequest = _authenticatedUser.GetRoles() switch
        {
            var roles when roles.IsCeo() || roles.IsCto() => resolverRequest,
            var roles when roles.IsHeadOfDepartment() => resolverRequest with
            {
                DepartmentId = _authenticatedUser.DepartmentId
            },
            var roles when roles.IsGroupManager() => resolverRequest with
            {
                ResourceGroupId = _authenticatedUser.ResourceGroupId!.Value
            },
            var roles when roles.IsEmployee() || roles.IsSystemAdministrator() => resolverRequest with
            {
                UserId = _authenticatedUser.UserId
            },
            _ => throw new InvalidOperationException()
        };

        var projects = await _projectsWithTotalTrackedTimeResolver.ResolveAsync(resolverRequest, cancellationToken);

        return projects
            .ConvertAll(x => new ProjectWithTrackedTimeReportDto
                (x.Name, x.Color, x.TotalTimeInMinutes));
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
        : IQuery<List<ProjectWithTrackedTimeReportDto>>, IPeriodRequest, ITimeTrackRequest;
}
