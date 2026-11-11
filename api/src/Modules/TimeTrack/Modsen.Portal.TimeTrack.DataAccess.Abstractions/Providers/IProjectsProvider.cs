using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;

internal interface IProjectsProvider
{
    Task<Project?> GetProjectByIdAsync(
        int projectId,
        CancellationToken cancellationToken);

    Task<Project?> GetProjectByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task<List<Project>> GetProjectsByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken);


    Task<bool> IsUserAssignedToProjectAsPmAsync(
        int userId,
        CancellationToken cancellationToken
    );
}
