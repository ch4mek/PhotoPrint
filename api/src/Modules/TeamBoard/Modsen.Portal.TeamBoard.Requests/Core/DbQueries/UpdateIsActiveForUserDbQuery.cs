using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IUpdateIsActiveForUserDbQuery
{
    Task Set(int userId, bool isActive, CancellationToken cancellationToken);
}

internal class UpdateIsActiveForUserDbQuery : IUpdateIsActiveForUserDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateIsActiveForUserDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task Set(int userId, bool isActive, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (user != null)
        {
            user.IsActive = isActive;

            _teamBoardDbContext.TeamBoardUsers.Update(user);
            await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
