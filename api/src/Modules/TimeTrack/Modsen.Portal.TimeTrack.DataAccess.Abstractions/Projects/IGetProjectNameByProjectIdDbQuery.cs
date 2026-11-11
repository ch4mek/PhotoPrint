namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

public interface IGetProjectNameByProjectIdDbQuery
{
    Task<string?> GetProjectNameByProjectIdAsync(int projectId, CancellationToken cancellationToken);
}
