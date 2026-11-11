using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetClientsWithTotalTrackedTimeRequest
    : IQueryHandler<GetClientsWithTotalTrackedTimeRequest.Query, List<ClientWithTrackedTimeReportDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IClientsWithTotalTrackedTimeResolver _clientWithTotalTrackedTimeResolver;

    public GetClientsWithTotalTrackedTimeRequest(
        IAuthenticatedUserContext authenticatedUser,
        IClientsWithTotalTrackedTimeResolver clientsWithTotalTrackedTimeResolver)
    {
        _authenticatedUser = authenticatedUser;
        _clientWithTotalTrackedTimeResolver = clientsWithTotalTrackedTimeResolver;
    }

    public async Task<List<ClientWithTrackedTimeReportDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var resolverRequest = new ClientsWithTotalTrackedTimeResolverRequest
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
            var roles when roles.IsCto() || roles.IsCeo() => resolverRequest,
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

        var clientsWithTotalTime = await _clientWithTotalTrackedTimeResolver.ResolveAsync(resolverRequest,
            cancellationToken);
        var result = clientsWithTotalTime.ConvertAll(x => new ClientWithTrackedTimeReportDto()
        {
            Name = x.Name,
            Color = x.Color,
            TotalTimeInMinutes = x.TotalTimeInMinutes,
        });

        return result;
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
        : IQuery<List<ClientWithTrackedTimeReportDto>>, IPeriodRequest, ITimeTrackRequest;
}
