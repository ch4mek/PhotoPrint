using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;

internal interface IGetTimeTrackUserDbQuery
{
    Task<TimeTrackUser?> ExecuteAsync(int userId, CancellationToken cancellationToken);
}

internal sealed class GetTimeTrackUserDbQuery : IGetTimeTrackUserDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetTimeTrackUserDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<TimeTrackUser?> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.TimeTrackUsers
            .FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);
    }
}
