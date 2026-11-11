using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Finance.DataAccess.Npgsql;

internal class DesignTimeFinanceDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        return new FinanceDbContext(new DbContextOptionsBuilder<FinanceDbContext>().UsePortalMySql(GetConnectionString()).Options);
    }
}
