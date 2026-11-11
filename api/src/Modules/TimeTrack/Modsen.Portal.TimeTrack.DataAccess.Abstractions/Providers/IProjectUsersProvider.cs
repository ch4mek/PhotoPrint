using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;

internal interface IProjectUsersProvider
{
    Task<List<ProjectUser>> GetProjectUsersByProjectIdAsync(
        int projectId,
        List<int> userIds,
        CancellationToken cancellationToken);

    Task<List<ProjectUser>> GetProjectUsersByProjectIdsAsync(
        List<int> projectIds,
        CancellationToken cancellationToken);
}
