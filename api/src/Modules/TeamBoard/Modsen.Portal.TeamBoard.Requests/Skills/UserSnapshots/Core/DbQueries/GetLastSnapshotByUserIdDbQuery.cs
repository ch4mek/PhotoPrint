using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.DbQueries;

internal interface IGetLastSnapshotByUserIdDbQuery
{
    Task<UserSkillsSnapshot?> ExecuteAsync(int userId, CancellationToken cancellationToken);
}

internal class GetLastSnapshotByUserIdDbQuery(ITeamBoardDbContext teamBoardDbContext) : IGetLastSnapshotByUserIdDbQuery
{
    public Task<UserSkillsSnapshot?> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
         return teamBoardDbContext.UserSkillsSnapshots
                .AsNoTracking()
                .Where(us => us.UserId == userId && !us.IsDeleted)
                .OrderByDescending(us => us.SnapshotDate)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
