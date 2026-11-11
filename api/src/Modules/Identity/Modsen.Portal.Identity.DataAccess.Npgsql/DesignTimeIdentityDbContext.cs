using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Identity.DataAccess.Npgsql;

internal class DesignTimeIdentityDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<IdentityModuleDbContext>
{
    public IdentityModuleDbContext CreateDbContext(string[] args)
    {
        return new IdentityModuleDbContext(new DbContextOptionsBuilder<IdentityModuleDbContext>().UsePortalMySql(GetConnectionString()).Options);
    }
}
