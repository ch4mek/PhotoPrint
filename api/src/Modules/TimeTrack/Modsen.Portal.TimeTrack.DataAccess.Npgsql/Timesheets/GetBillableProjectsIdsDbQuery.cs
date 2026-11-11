using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal class GetBillableProjectsIdsDbQuery : IGetBillableProjectsIdsDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetBillableProjectsIdsDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<int>> ExecuteAsync(List<int> request, CancellationToken cancellationToken)
    {
        var billableProjectIds = new List<int>();

        foreach (var projectId in request)
        {
            var project = await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project != null && project.IsBillable)
            {
                billableProjectIds.Add(projectId);
            }
        }

        return billableProjectIds;
    }
}
