using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql;

internal class DesignTimeEquipmentDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<EquipmentDbContext>
{
    public EquipmentDbContext CreateDbContext(string[] args)
    {
        return new EquipmentDbContext(new DbContextOptionsBuilder<EquipmentDbContext>().UsePortalMySql(GetConnectionString()).Options);
    }
}
