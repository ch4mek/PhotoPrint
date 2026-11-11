using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class GetUserDetailsDbQuery : IGetUserDetailsDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetUserDetailsDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<GetUserDetailsDbQueryResponse?> Execute(int userId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.TimeTrackUsers.Where(user => user.UserId == userId)
            .Select(user => new GetUserDetailsDbQueryResponse
            {
                UserId = user.UserId,
                DepartmentId = user.DepartmentId,
                Email = user.Email,
                Role = user.Position ?? PortalRoleNames.Employee,
                ResourceGroupId = user.ResourceGroupId,
                IsActive = user.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
