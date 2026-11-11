using Modsen.Portal.Core.Database;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Seeds;

internal class WarehousesSeed : ISeedEntitiesProvider<MerchDbContext>
{
    private readonly MerchDbContext _dbContext;

    public WarehousesSeed(MerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var warehouses = new List<WarehouseEntity>
        {
            new() { Location = "Minsk" },
            new() { Location = "Vitebsk" },
            new() { Location = "Warsaw" }
        };

        foreach (var warehouseEntity in warehouses)
        {
            var entity = _dbContext.Warehouses
                .FirstOrDefault(warehouse => warehouse.Location == warehouseEntity.Location);

            if (entity is null)
            {
                _dbContext.Warehouses.Add(warehouseEntity);
            }
        }

        _dbContext.SaveChanges();
    }
}
