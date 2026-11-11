using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestMerchSeed : ISeedEntitiesProvider<MerchDbContext>
{
    private readonly MerchDbContext _dbContext;


    public IntegrationTestMerchSeed(MerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var storages = new List<WarehouseEntity>()
        {
            new() { Id = 1, Location = "Minsk" },
            new() { Id = 2, Location = "Vitebsk" },
            new() { Id = 3, Location = "Warsaw" },
        };

        var sizesEntities = new List<MerchSizeEntity>
        {
            new() { Id = "XS", OrderWeight = 100, IsForClothing = true, Name = "XS" },
            new() { Id = "S", OrderWeight = 200, IsForClothing = true, Name = "S" },
            new() { Id = "M", OrderWeight = 300, IsForClothing = true , Name = "M"},
            new() { Id = "L", OrderWeight = 400, IsForClothing = true , Name = "L"},
            new() { Id = "XL", OrderWeight = 500, IsForClothing = true, Name = "XL" },
            new() { Id = "2XL", OrderWeight = 600, IsForClothing = true, Name = "2XL" },
            new() { Id = "3XL", OrderWeight = 700, IsForClothing = true, Name = "3XL" },
            new() { Id = "4XL", OrderWeight = 800, IsForClothing = true, Name = "4XL" },
            new() { Id = "One", OrderWeight = 0, IsForClothing = false, Name = "One" },
        };

        var items = new List<ItemEntity>()
        {
            new()
            {
                Id = 1,
                Name = "Modsen T-shirt",
                IsClothing = true,
                Supplier = "Test supplier",
                Status = MerchItemStatus.Available,
                Description = "White gift T-shirt",
                CreatedById = 1,
                CreatedAtUtc = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Name = "Modsen Coffee cup",
                IsClothing = false,
                Supplier = "Test supplier",
                Status = MerchItemStatus.Available,
                Description = "White gift coffee cup",
                CreatedById = 1,
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        var storagesCount = _dbContext.Warehouses.Count();
        var sizesCount = _dbContext.MerchSizes.Count();

        if (storagesCount == 0)
        {
            _dbContext.Warehouses.AddRange(storages);
        }

        if (sizesCount == 0)
        {
            _dbContext.MerchSizes.AddRange(sizesEntities);
        }


        var shirt = _dbContext.Items.FirstOrDefault(item => item.Id == items[0].Id);
        if (shirt is null)
        {
            var shirtMinAmounts = new List<ItemSizeMinAmountEntity>()
            {
                new() { Item = shirt, SizeId = "XS", MinAmount = 10, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "S", MinAmount = 15, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "M", MinAmount = 20, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "L", MinAmount = 22, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "XL", MinAmount = 15, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "2XL", MinAmount = 10, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "3XL", MinAmount = 5, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "4XL", MinAmount = 5, WarehouseId = 1 },
                new() { Item = shirt, SizeId = "XS", MinAmount = 10, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "S", MinAmount = 13, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "M", MinAmount = 19, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "L", MinAmount = 21, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "XL", MinAmount = 13, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "2XL", MinAmount = 9, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "3XL", MinAmount = 4, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "4XL", MinAmount = 3, WarehouseId = 2 },
                new() { Item = shirt, SizeId = "XS", MinAmount = 0, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "S", MinAmount = 0, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "M", MinAmount = 0, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "L", MinAmount = 1, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "XL", MinAmount = 0, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "2XL", MinAmount = 0, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "3XL", MinAmount = 0, WarehouseId = 3 },
                new() { Item = shirt, SizeId = "4XL", MinAmount = 0, WarehouseId = 3 }
            };

            items[0].ItemSizeMinAmounts = shirtMinAmounts;

            var sizes = new[]
            {
                "XS", "S", "M", "L", "XL", "2XL", "3XL", "4XL"
            };

            var firstItemTransactions = new List<ItemTransactionEntity>();

            foreach (var size in sizes)
            {
                for (var warehouseId = 1; warehouseId <= 3; warehouseId++)
                {
                    firstItemTransactions.Add(new ItemTransactionEntity
                    {
                        Item = items[0],
                        SizeId = size,
                        SenderId = warehouseId,
                        Type = MerchTransactionType.Receipt,
                        Count = 10,
                        CreatedAtUtc = DateTime.UtcNow,
                        CreatedById = 1
                    });


                    var outcomeCount = warehouseId == 3 ? 10 : 5;
                    firstItemTransactions.Add(new ItemTransactionEntity
                    {
                        Item = items[0],
                        SizeId = size,
                        SenderId = warehouseId,
                        Type = MerchTransactionType.WriteOff,
                        Count = -outcomeCount,
                        CreatedAtUtc = DateTime.UtcNow.AddMinutes(1),
                        CreatedById = 1
                    });
                }
            }

            items[0].ItemTransactions = firstItemTransactions;

            _dbContext.Items.Add(items[0]);
        }

        var cup = _dbContext.Items.FirstOrDefault(item => item.Id == items[1].Id);
        if (cup is null)
        {
            var cupMinAmounts = new List<ItemSizeMinAmountEntity>()
            {
                new() { Item = cup, SizeId = "One", MinAmount = 30, WarehouseId = 1 },
                new() { Item = cup, SizeId = "One", MinAmount = 5, WarehouseId = 2 },
                new() { Item = cup, SizeId = "One", MinAmount = 0, WarehouseId = 3 },
            };

            items[1].ItemSizeMinAmounts = cupMinAmounts;

            var secondItemTransactions = new List<ItemTransactionEntity>();

            for (var warehouseId = 1; warehouseId <= 3; warehouseId++)
            {
                secondItemTransactions.Add(new ItemTransactionEntity
                {
                    Item = items[1],
                    SizeId = "One",
                    SenderId = warehouseId,
                    Type = MerchTransactionType.Receipt,
                    Count = 15,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedById = 1
                });

                var outcomeCount = warehouseId == 2 ? 15 : 7;
                secondItemTransactions.Add(new ItemTransactionEntity
                {
                    Item = items[1],
                    SizeId = "One",
                    SenderId = warehouseId,
                    Type = MerchTransactionType.WriteOff,
                    Count = -outcomeCount,
                    CreatedAtUtc = DateTime.UtcNow.AddMinutes(1),
                    CreatedById = 1
                });
            }

            items[1].ItemTransactions = secondItemTransactions;
            _dbContext.Items.Add(items[1]);
        }

        _dbContext.SaveChanges();
    }
}
