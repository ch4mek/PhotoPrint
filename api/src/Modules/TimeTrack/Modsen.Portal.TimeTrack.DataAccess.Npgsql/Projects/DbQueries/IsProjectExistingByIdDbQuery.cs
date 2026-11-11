using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class IsProjectExistingByIdDbQuery : IIsProjectExistingByIdDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public IsProjectExistingByIdDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.Projects.AnyAsync(project => project.Id == id, cancellationToken);
    }
}
