using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;

internal class GetResourceGroupIdsByClientDbQuery : IGetResourceGroupIdsByClientDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetResourceGroupIdsByClientDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<int>> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.ProjectUsers
            .Where(projectUser => projectUser.Project!.ClientId == id && projectUser.User!.ResourceGroupId != null)
            .Select(projectUser => projectUser.User!.ResourceGroupId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
