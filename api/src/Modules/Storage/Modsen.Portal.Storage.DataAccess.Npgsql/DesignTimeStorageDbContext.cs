using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Storage.DataAccess.Npgsql;

internal class DesignTimeStorageDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<StorageDbContext>
{
    public StorageDbContext CreateDbContext(string[] args)
    {
        return new StorageDbContext(new DbContextOptionsBuilder<StorageDbContext>().UsePortalMySql(GetConnectionString()).Options);
    }
}
