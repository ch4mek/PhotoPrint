using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Migrator.DataAccess;

internal class DesignTimeMigratorDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<MigratorDbContext>
{
    public MigratorDbContext CreateDbContext(string[] args)
    {
        return new MigratorDbContext(new DbContextOptionsBuilder<MigratorDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
