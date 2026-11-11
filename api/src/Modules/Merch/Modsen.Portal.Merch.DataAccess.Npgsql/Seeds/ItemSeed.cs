using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Seeds;

internal class ItemSeed : ISeedEntitiesProvider<MerchDbContext>
{
    private readonly MerchDbContext _dbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public ItemSeed(MerchDbContext dbContext, IHostEnvironmentContext hostEnvironmentContext)
    {
        _dbContext = dbContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        var createdAt = DateTime.UtcNow;
        
        foreach (var itemToCreate in GetItems(createdAt))
        {
            if (!_dbContext.Items.Any(item => item.Name == itemToCreate.Name))
            {
                _dbContext.Items.Add(itemToCreate);
            }
        }

        _dbContext.SaveChanges();
    }

    private static IEnumerable<ItemEntity> GetItems(DateTime createdAt)
    {
        yield return new ItemEntity
        {
            Name = "Modsen T-shirt",
            IsClothing = true,
            Supplier = "Test supplier",
            Status = MerchItemStatus.Available,
            Description = "White gift T-shirt",
            CreatedById = 1,
            CreatedAtUtc = createdAt,
            ItemSizeMinAmounts = GetAllSizes()
        };
        yield return new ItemEntity
        {
            Name = "Modsen Coffee cup",
            IsClothing = false,
            Supplier = "Test supplier",
            Status = MerchItemStatus.Available,
            Description = "White gift coffee cup",
            CreatedById = 1,
            CreatedAtUtc = createdAt,
            ItemSizeMinAmounts =
            [
                ItemSize("One", 30, 1),
                ItemSize("One", 5, 2),
                ItemSize("One", 0, 3)
            ]
        };
        yield return new ItemEntity
        {
            Name = "New Year 2026 black hoodie",
            IsClothing = true,
            Supplier = "Supplier of New Year 2026",
            Status = MerchItemStatus.Available,
            Description = "Black hoodie with new year print",
            CreatedById = 1,
            CreatedAtUtc = createdAt,
            ItemSizeMinAmounts = GetAllSizes()
        };
        yield return new ItemEntity
        {
            Name = "New Year 2026 postcard",
            IsClothing = false,
            Supplier = "Test supplier",
            Status = MerchItemStatus.Available,
            Description = "Gift postcard",
            CreatedById = 1,
            CreatedAtUtc = createdAt,
            ItemSizeMinAmounts =
            [
                ItemSize("One", 30, 1),
                ItemSize("One", 5, 2),
                ItemSize("One", 0, 3)
            ]
        };
        yield return new ItemEntity
        {
            Name = "1 year anniversary at Modsen cup",
            IsClothing = false,
            Supplier = "Supplier 1",
            Status = MerchItemStatus.Available,
            Description = "Cup with Modsen logo",
            CreatedById = 1,
            CreatedAtUtc = createdAt,
            ItemSizeMinAmounts =
            [
                ItemSize("One", 15, 1),
                ItemSize("One", 10, 2),
                ItemSize("One", 5, 3),
            ]
        };
        yield return new ItemEntity
        {
            Name = "1 year anniversary at Modsen postcard",
            IsClothing = false,
            Supplier = "Supplier Minsk 2",
            Status = MerchItemStatus.Available,
            Description = "Postcard with greetings",
            CreatedById = 1,
            CreatedAtUtc = createdAt,
            ItemSizeMinAmounts =
            [
                ItemSize("One", 20, 1),
                ItemSize("One", 10, 2),
                ItemSize("One", 10, 3),
            ]
        };
    }
    
    private static List<ItemSizeMinAmountEntity> GetAllSizes()
    {
        return
        [
            ItemSize("XS", 10, 1),
            ItemSize("S", 15, 1),
            ItemSize("M", 20, 1),
            ItemSize("L", 22, 1),
            ItemSize("XL", 15, 1),
            ItemSize("2XL", 10, 1),
            ItemSize("3XL", 5, 1),
            ItemSize("4XL", 5, 1),
            ItemSize("XS", 10, 2),
            ItemSize("S", 13, 2),
            ItemSize("M", 19, 2),
            ItemSize("L", 21, 2),
            ItemSize("XL", 13, 2),
            ItemSize("2XL", 9, 2),
            ItemSize("3XL", 4, 2),
            ItemSize("4XL", 3, 2),
            ItemSize("XS", 0, 3),
            ItemSize("S", 0, 3),
            ItemSize("M", 0, 3),
            ItemSize("L", 1, 3),
            ItemSize("XL", 0, 3),
            ItemSize("2XL", 0, 3),
            ItemSize("3XL", 0, 3),
            ItemSize("4XL", 0, 3),
        ];
    }

    private static ItemSizeMinAmountEntity ItemSize(string id, int minAmount, int warehouseId)
    {
        return new()
        {
            SizeId = id,
            MinAmount = minAmount,
            WarehouseId = warehouseId
        };
    }
}
