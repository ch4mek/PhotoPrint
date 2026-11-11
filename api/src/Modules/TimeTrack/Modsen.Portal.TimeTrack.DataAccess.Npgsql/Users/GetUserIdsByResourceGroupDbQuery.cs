using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class GetUserIdsByResourceGroupDbQuery : IGetUserIdsByResourceGroupDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetUserIdsByResourceGroupDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<int>> ExecuteAsync(int resourceGroupId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.TimeTrackUsers.Where(user => user.ResourceGroupId == resourceGroupId)
            .Select(user => user.UserId).ToListAsync(cancellationToken);
    }
}
