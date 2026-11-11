using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Seeds;

internal class KitsSeed : ISeedEntitiesProvider<MerchDbContext>
{
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly MerchDbContext _dbContext;

    public KitsSeed(IHostEnvironmentContext hostEnvironmentContext, MerchDbContext dbContext)
    {
        _hostEnvironmentContext = hostEnvironmentContext;
        _dbContext = dbContext;

    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        foreach (var kit in GetKits())
        {
            if (!_dbContext.Kits.Any(x => x.Name != kit.Name))
            {
                _dbContext.Kits.Add(kit);
            }
        }

        _dbContext.SaveChanges();
    }

    private IEnumerable<KitEntity> GetKits()
    {
        yield return new KitEntity
        {
            Name = "1 year anniversary at Modsen",
            Description = "Kit with presents for 1 year anniversary at Modsen",
            CreatedById = 1,
            CreatedAtUtc = DateTime.UtcNow,
            Status = MerchItemStatus.Available,
            KitItems =
            [
                new()
                {
                    Count = 1,
                    Item = _dbContext.Items.FirstOrDefault(x => x.Name == "1 year anniversary at Modsen cup")
                },
                new()
                {
                    Count = 2,
                    Item = _dbContext.Items.FirstOrDefault(x => x.Name == "1 year anniversary at Modsen postcard")
                }
            ],
            KitMinAmounts =
            [
                new() { WarehouseId = 1, MinAmount = 10 },
                new() { WarehouseId = 2, MinAmount = 3 },
                new() { WarehouseId = 3, MinAmount = 1 }
            ]
        };
        yield return new KitEntity
        {
            Name = "New Year 2026",
            Description = "Kit with presents for new year 2026",
            CreatedById = 1,
            CreatedAtUtc = DateTime.UtcNow,
            Status = MerchItemStatus.Available,
            KitItems =
            [
                new()
                {
                    Count = 1, Item = _dbContext.Items.FirstOrDefault(x => x.Name == "New Year 2026 black hoodie")
                },
                new() { Count = 2, Item = _dbContext.Items.FirstOrDefault(x => x.Name == "New Year 2026 postcard") }
            ],
            KitMinAmounts =
            [
                new() { WarehouseId = 1, MinAmount = 2 },
                new() { WarehouseId = 2, MinAmount = 10 },
                new() { WarehouseId = 3, MinAmount = 5 }
            ]
        };
    }
}
