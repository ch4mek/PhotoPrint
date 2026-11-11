using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;

internal class ProjectUsersProvider : IProjectUsersProvider
{
    private readonly ITimeTrackDbContext _dbContext;

    public ProjectUsersProvider(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ProjectUser>> GetProjectUsersByProjectIdAsync(
        int projectId,
        List<int> userIds,
        CancellationToken cancellationToken)
    {
        var projectUsers = await _dbContext.ProjectUsers
            .AsNoTracking()
            .Include(projectUser => projectUser.User)
            .Where(projectUser => projectUser.ProjectId == projectId && userIds.Any(id => id == projectUser.UserId))
            .ToListAsync(cancellationToken);

        return projectUsers.Select(x => DatabaseEntities.ProjectUser.ToDomain(x)!).ToList();
    }

    public async Task<List<ProjectUser>> GetProjectUsersByProjectIdsAsync(
        List<int> projectIds,
        CancellationToken cancellationToken)
    {
        var projectUsers = await _dbContext.ProjectUsers
            .AsNoTracking()
            .Include(projectUser => projectUser.User)
            .Where(projectUser => projectIds.Any(id => id == projectUser.ProjectId))
            .ToListAsync(cancellationToken);

        return projectUsers.Select(x => DatabaseEntities.ProjectUser.ToDomain(x)!).ToList();
    }
}
