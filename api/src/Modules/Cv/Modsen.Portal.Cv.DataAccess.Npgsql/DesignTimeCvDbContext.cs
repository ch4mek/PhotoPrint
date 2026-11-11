using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Cv.DataAccess.Npgsql;

internal class DesignTimeCvDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<CvDbContext>
{
    public CvDbContext CreateDbContext(string[] args)
    {
        return new CvDbContext(new DbContextOptionsBuilder<CvDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
