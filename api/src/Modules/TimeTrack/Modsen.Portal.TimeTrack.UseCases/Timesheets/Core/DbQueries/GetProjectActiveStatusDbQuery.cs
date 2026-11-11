using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

public interface IGetProjectActiveStatusDbQuery
{
    public Task<bool> ExecuteAsync(int projectId, CancellationToken cancellationToken);
}

internal class GetProjectActiveStatusDbQuery : IGetProjectActiveStatusDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetProjectActiveStatusDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(int projectId, CancellationToken cancellationToken)
    {
        var status = await _dbContext.Projects
            .Where(project => project.Id == projectId)
            .Select(project => project.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return status;
    }
}
