namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

internal interface IRemoveProjectManagerByUserIdDbQuery
{
    Task<int> Execute(int userId, CancellationToken cancellationToken);
}
