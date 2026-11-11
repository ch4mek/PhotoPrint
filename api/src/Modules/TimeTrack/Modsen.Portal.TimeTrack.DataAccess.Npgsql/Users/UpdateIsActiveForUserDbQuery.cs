using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class UpdateIsActiveForUserDbQuery : IUpdateIsActiveForUserDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public UpdateIsActiveForUserDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task Set(int userId, bool isActive, CancellationToken cancellationToken)
    {
        var user = await _timeTrackDbContext.TimeTrackUsers
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (user != null)
        {
            user.IsActive = isActive;

            _timeTrackDbContext.TimeTrackUsers.Update(user);
            await _timeTrackDbContext.SaveChanges(cancellationToken);
        }
    }
}
