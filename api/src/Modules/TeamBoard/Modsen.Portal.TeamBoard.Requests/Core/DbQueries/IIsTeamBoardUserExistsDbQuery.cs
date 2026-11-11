using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IIsTeamBoardUserExistsDbQuery
{
    Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken);
}

internal class IsTeamBoardUserExistsDbQuery : IIsTeamBoardUserExistsDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public IsTeamBoardUserExistsDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.TeamBoardUsers.AnyAsync(user => user.UserId == userId, cancellationToken);
    }
}
