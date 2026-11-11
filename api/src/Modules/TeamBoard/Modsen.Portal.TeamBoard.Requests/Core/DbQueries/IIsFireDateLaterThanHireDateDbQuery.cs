using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IIsFireDateLaterThanHireDateDbQuery
{
    Task<bool> ExecuteAsync(int userId, DateOnly fireDate, CancellationToken cancellationToken);
}

internal class IsFireDateLaterThanHireDateDbQuery : IIsFireDateLaterThanHireDateDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public IsFireDateLaterThanHireDateDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<bool> ExecuteAsync(int userId, DateOnly fireDate, CancellationToken cancellationToken)
    {
        var userHireDate = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == userId)
            .Select(user => user.HireDate)
            .FirstOrDefaultAsync(cancellationToken);

        return fireDate > userHireDate;
    }
}
