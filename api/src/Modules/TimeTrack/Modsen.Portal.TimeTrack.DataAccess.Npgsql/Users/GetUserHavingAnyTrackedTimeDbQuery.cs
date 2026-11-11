using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.GetUserHavingAnyTrackedTime;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class GetUserHavingAnyTrackedTimeDbQuery : IGetUserHavingAnyTrackedTimeDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetUserHavingAnyTrackedTimeDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<GetUserHavingAnyTrackedTimeResponse?> ExecuteAsync(
        int request,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.TimeTrackUsers
                .AsNoTracking()
                .Where(user => user.UserId == request &&
                               user.Timesheets.Any())
                .Select(user => new GetUserHavingAnyTrackedTimeResponse(user.UserId, user.DepartmentId, user.ResourceGroupId))
                .FirstOrDefaultAsync(cancellationToken);
    }
}
