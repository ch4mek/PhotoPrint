using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.UserNames;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class GetUserNamesResponseByUserIdDbQuery : IGetUserNamesResponseByUserIdDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetUserNamesResponseByUserIdDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserNamesResponse?> GetUserNamesDtoByUserIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.TimeTrackUsers
                .Where(user => user.UserId == userId)
                .Select(user => new UserNamesResponse(user.FirstName, user.LastName))
                .FirstOrDefaultAsync(cancellationToken);
    }
}
