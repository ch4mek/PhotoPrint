using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class RemoveProjectManagerByUserIdDbQuery : IRemoveProjectManagerByUserIdDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public RemoveProjectManagerByUserIdDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<int> Execute(int userId, CancellationToken cancellationToken)
    {
        var projects = await _timeTrackDbContext.Projects
            .Where(project => project.ManagerUserId == userId)
            .ToListAsync(cancellationToken);

        foreach (var project in projects)
        {
            project.ManagerUserId = null;
        }

        _timeTrackDbContext.Projects.UpdateRange(projects);
        return await _timeTrackDbContext.SaveChanges(cancellationToken);
    }
}
