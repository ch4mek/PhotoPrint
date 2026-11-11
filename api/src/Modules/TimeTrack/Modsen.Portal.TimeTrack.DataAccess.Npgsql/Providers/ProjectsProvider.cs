using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;

internal class ProjectsProvider : IProjectsProvider
{
    private readonly ITimeTrackDbContext _dbContext;

    public ProjectsProvider(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Project?> GetProjectByIdAsync(
        int projectId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectUsers)
            .ThenInclude(user => user.User)
            .Select(DatabaseEntities.Project.Map)
            .FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);
    }

    public Task<Project?> GetProjectByNameAsync(string name, CancellationToken cancellationToken)
    {
        var lowerCaseName = name.ToLower();

        return _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.ProjectUsers)
            .ThenInclude(user => user.User)
            .Select(DatabaseEntities.Project.Map)
            .FirstOrDefaultAsync(project => project.Name.ToLower() == lowerCaseName, cancellationToken);
    }

    public Task<List<Project>> GetProjectsByClientIdAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AsNoTracking()
            .Include(x => x.ProjectUsers)
            .ThenInclude(x => x.User)
            .Where(x => x.IsActive && x.ClientId == clientId)
            .OrderBy(x => x.Id)
            .Select(DatabaseEntities.Project.Map)
            .ToListAsync(cancellationToken);
    }


    public Task<bool> IsUserAssignedToProjectAsPmAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.Projects
            .AnyAsync(p => p.ManagerUserId == userId, cancellationToken);
    }
}
