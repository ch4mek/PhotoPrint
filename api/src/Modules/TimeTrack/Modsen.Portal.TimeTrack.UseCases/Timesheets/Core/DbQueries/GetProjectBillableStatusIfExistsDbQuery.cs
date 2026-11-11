using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

public interface IGetProjectBillableStatusIfExistsDbQuery
{
    public Task<bool?> ExecuteAsync(int projectId, CancellationToken cancellationToken);
}

internal class GetProjectBillableStatusIfExistsDbQuery : IGetProjectBillableStatusIfExistsDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetProjectBillableStatusIfExistsDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool?> ExecuteAsync(int projectId, CancellationToken cancellationToken)
    {
        var status = await _dbContext.Projects
            .Where(project => project.Id == projectId)
            .Select(project => new { project.IsBillable })
            .FirstOrDefaultAsync(cancellationToken);

        return status?.IsBillable;
    }
}
