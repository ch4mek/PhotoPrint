using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql;

internal class DesignTimeTeamBoardDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<TeamBoardDbContext>
{
    public TeamBoardDbContext CreateDbContext(string[] args)
    {
        return new TeamBoardDbContext(new DbContextOptionsBuilder<TeamBoardDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
