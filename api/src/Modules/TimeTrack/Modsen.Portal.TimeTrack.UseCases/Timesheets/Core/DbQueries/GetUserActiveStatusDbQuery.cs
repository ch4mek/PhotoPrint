using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

internal interface IGetUserActiveStatusDbQuery
{
    public Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken);
}

internal class GetUserActiveStatusDbQuery : IGetUserActiveStatusDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetUserActiveStatusDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        var status = await _dbContext.TimeTrackUsers
            .Where(user => user.UserId == userId)
            .Select(user => user.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        return status;
    }
}
