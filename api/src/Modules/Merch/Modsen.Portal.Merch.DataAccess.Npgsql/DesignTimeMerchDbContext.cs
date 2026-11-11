using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Merch.DataAccess.Npgsql;

internal class DesignTimeMerchDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<MerchDbContext>
{
    public MerchDbContext CreateDbContext(string[] args)
    {
        return new MerchDbContext(new DbContextOptionsBuilder<MerchDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
