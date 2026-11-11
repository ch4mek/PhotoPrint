using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class GetProjectNameByProjectIdDbQuery : IGetProjectNameByProjectIdDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetProjectNameByProjectIdDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetProjectNameByProjectIdAsync(
        int projectId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Projects
            .Where(project => project.Id == projectId)
            .Select(project => project.Name)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
