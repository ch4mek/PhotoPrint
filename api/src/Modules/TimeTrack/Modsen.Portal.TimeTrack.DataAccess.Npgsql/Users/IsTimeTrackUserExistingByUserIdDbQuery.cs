using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class IsTimeTrackUserExistingByUserIdDbQuery : IIsTimeTrackUserExistingByUserIdDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public IsTimeTrackUserExistingByUserIdDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        return _dbContext.TimeTrackUsers.AnyAsync(user => user.UserId == id, cancellationToken);
    }
}
