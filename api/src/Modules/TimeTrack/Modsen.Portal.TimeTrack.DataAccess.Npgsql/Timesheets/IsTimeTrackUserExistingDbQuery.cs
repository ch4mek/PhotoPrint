using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal class IsTimeTrackUserExistingDbQuery : IIsTimeTrackUserExistingDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public IsTimeTrackUserExistingDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.TimeTrackUsers.AnyAsync(x => x.UserId == userId, cancellationToken);
    }
}
