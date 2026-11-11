using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql;

internal class DesignTimeTimeTrackDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<TimeTrackDbContext>
{
    public TimeTrackDbContext CreateDbContext(string[] args)
    {
        return new TimeTrackDbContext(new DbContextOptionsBuilder<TimeTrackDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
