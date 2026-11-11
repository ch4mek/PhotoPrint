using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectManagersLookupRequest : IQueryHandler<GetProjectManagersLookupRequest.Query, List<ProjectManagerLookupDto>>
{
    private readonly IIdentityContract _identityContract;

    public GetProjectManagersLookupRequest(IIdentityContract identityContract)
    {
        _identityContract = identityContract;
    }

    public async Task<List<ProjectManagerLookupDto>> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        var projectManagers = await _identityContract.GetProjectManagersLookup(cancellationToken);

        var headOfManagers = projectManagers.FirstOrDefault(x => x.Role == PortalRoleNames.HeadOfDepartment);

        if (headOfManagers is not null)
        {
            projectManagers.Remove(headOfManagers);

            projectManagers = [headOfManagers, .. projectManagers];
        }

        return projectManagers.ConvertAll(x => new ProjectManagerLookupDto(x.UserId, $"{x.LastName} {x.FirstName}"));
    }

    internal sealed record Query : IQuery<List<ProjectManagerLookupDto>>, ITimeTrackRequest;
}
