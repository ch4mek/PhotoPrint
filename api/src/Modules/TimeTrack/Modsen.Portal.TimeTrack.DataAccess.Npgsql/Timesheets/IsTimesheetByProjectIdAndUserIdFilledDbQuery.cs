using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal class IsTimesheetByProjectIdAndUserIdFilledDbQuery : IIsTimesheetByProjectIdAndUserIdFilledDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public IsTimesheetByProjectIdAndUserIdFilledDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<bool> ExecuteAsync(IsTimesheetFilledModel request, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .AnyAsync(x => x.ProjectId == request.ProjectId && x.UserId == request.UserId, cancellationToken);
    }
}
