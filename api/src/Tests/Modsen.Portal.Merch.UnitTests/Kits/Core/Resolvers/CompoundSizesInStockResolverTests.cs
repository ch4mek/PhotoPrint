using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Resolvers;

public class CompoundSizesInStockResolverTests
{
    private readonly Mock<IMerchDbContext> _dbContextMock = new();
    private readonly CompoundSizesInStockResolver _resolver;

    private readonly CancellationToken _token = CancellationToken.None;

    public CompoundSizesInStockResolverTests()
    {
        _resolver = new CompoundSizesInStockResolver(_dbContextMock.Object);
    }

    [Fact]
    public async Task ResolveAsyncWhenItemWithOneSizeReturnsCurrentStockOfItemSizeInStorages()
    {
        //Arrange
        var size = new MerchSizeEntity { OrderWeight = 0, Id = "One", Name = "test" };
        var sizes = new List<MerchSizeEntity> { size };
        var minskStorage = new WarehouseEntity { Location = "Minsk", Id = 1 };
        var vitebskStorage = new WarehouseEntity { Location = "Vitebsk", Id = 2 };

        var transactions = new List<ItemTransactionEntity>
        {
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = size,
                WarehouseId = 1,
                Warehouse = minskStorage,
                Count = 100
            },
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = size,
                WarehouseId = 1,
                Warehouse = minskStorage,
                Count = -10
            }
        };

        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new() { Id = 1, IsClothing = false, Name = "test"}
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity> { minskStorage, vitebskStorage }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.MerchSizes)
            .Returns(sizes.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);

        var expectedResult = new List<CompoundSizeAmountInStockCoreDto>
        {
            new()
            {
                Amount = 90,
                SizeId = "One",
                SizeOrderWeight = size.OrderWeight,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "One",
                SizeOrderWeight = size.OrderWeight,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            }
        };

        //Act
        var result = await _resolver.ResolveAsync([1], null, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ResolveAsyncWhenItemWithOneSizeAndSpecifiedStorageIdReturnsCurrentStockOfItemSizeInStorage()
    {
        //Arrange
        var size = new MerchSizeEntity { OrderWeight = 0, Id = "One", Name = "test" };
        var sizes = new List<MerchSizeEntity> { size };
        var minskStorage = new WarehouseEntity { Location = "Minsk" };
        var vitebskStorage = new WarehouseEntity { Location = "Vitebsk" };

        var transactions = new List<ItemTransactionEntity>
        {
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = size,
                WarehouseId = 1,
                Warehouse = minskStorage,
                Count = 100
            },
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = size,
                WarehouseId = 1,
                Warehouse = minskStorage,
                Count = -10
            },
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = size,
                WarehouseId = 2,
                Warehouse = vitebskStorage,
                Count = 50
            }
        };

        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new() { Id = 1, IsClothing = false, Name = "test"}
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity> { minskStorage, vitebskStorage }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.MerchSizes)
            .Returns(sizes.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);

        var expectedResult = new List<CompoundSizeAmountInStockCoreDto>
        {
            new()
            {
                Amount = 90,
                SizeId = "One",
                SizeOrderWeight = size.OrderWeight,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            }
        };

        //Act
        var result = await _resolver.ResolveAsync([1], [1], _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ResolveAsyncWhenItemWithOneSizeAndSpecifiedStorageIdsReturnsCurrentStockOfItemSizeInStorages()
    {
        //Arrange
        var sizes = new List<MerchSizeEntity> { new() { Id = "One", OrderWeight = 0, Name = "test" } };
        var minskStorage = new WarehouseEntity { Location = "Minsk" };
        var vitebskStorage = new WarehouseEntity { Location = "Vitebsk" };

        var storages = new List<WarehouseEntity> { minskStorage, vitebskStorage };

        var transactions = new List<ItemTransactionEntity>
        {
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = sizes.First(),
                WarehouseId = 1,
                Warehouse = minskStorage,
                Count = 100
            },
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = sizes.First(),
                WarehouseId = 1,
                Warehouse = minskStorage,
                Count = -10
            },
            new()
            {
                ItemId = 1,
                SizeId = "One",
                Size = sizes.First(),
                WarehouseId = 2,
                Warehouse = vitebskStorage,
                Count = 50
            }
        };

        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new() { Id = 1, IsClothing = false, Name = "test"}
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.MerchSizes)
            .Returns(sizes.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(storages.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);

        var expectedResult = new List<CompoundSizeAmountInStockCoreDto>
        {
            new()
            {
                Amount = 90,
                SizeId = "One",
                SizeOrderWeight = sizes.First().OrderWeight,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 50,
                SizeId = "One",
                SizeOrderWeight = sizes.First().OrderWeight,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            }
        };

        //Act
        var result = await _resolver.ResolveAsync([1], [1, 2], _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task ResolveAsyncWhenItemsHaveNoTransactionsReturnsZerosForAllSizesAndAllStorages()
    {
        //Arrange
        var sizes = new List<MerchSizeEntity>
        {
            new() {Id = "XS", Name = "test", OrderWeight = 100, IsForClothing = true},
            new() {Id = "S",  Name = "test", OrderWeight = 200, IsForClothing = true},
            new() {Id = "M",  Name = "test", OrderWeight = 300, IsForClothing = true},
            new() {Id = "L",  Name = "test", OrderWeight = 400, IsForClothing = true},
            new() {Id = "XL", Name = "test", OrderWeight = 500, IsForClothing = true},
            new() {Id = "2XL",Name = "test", OrderWeight = 600, IsForClothing = true},
            new() {Id = "3XL",Name = "test", OrderWeight = 700, IsForClothing = true},
            new() {Id = "4XL",Name = "test", OrderWeight = 800, IsForClothing = true},
            new() {Id = "One",Name = "test", OrderWeight = 0, IsForClothing = false},
        };
        var minskStorage = new WarehouseEntity { Id = 1, Location = "Minsk" };
        var vitebskStorage = new WarehouseEntity { Id = 2, Location = "Vitebsk" };

        _dbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(new List<ItemTransactionEntity>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity> { minskStorage, vitebskStorage }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.MerchSizes)
            .Returns(new List<MerchSizeEntity>(sizes).BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity> { new() { Id = 1, IsClothing = true, Name = "test" }, new() { Id = 2, IsClothing = false, Name = "test" } }
                .BuildMockDbSet().Object);

        var expectedResult = new List<CompoundSizeAmountInStockCoreDto>
        {
            new()
            {
                Amount = 0,
                SizeId = "One",
                SizeOrderWeight = 0,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 2
            },
            new()
            {
                Amount = 0,
                SizeId = "One",
                SizeOrderWeight = 0,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 2
            },

            new()
            {
                Amount = 0,
                SizeId = "XS",
                SizeOrderWeight = 100,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "S",
                SizeOrderWeight = 200,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "M",
                SizeOrderWeight = 300,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "L",
                SizeOrderWeight = 400,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "XL",
                SizeOrderWeight = 500,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "2XL",
                SizeOrderWeight = 600,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "3XL",
                SizeOrderWeight = 700,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "4XL",
                SizeOrderWeight = 800,
                StorageId = 1,
                StorageName = minskStorage.Location,
                ItemId = 1
            },

            new()
            {
                Amount = 0,
                SizeId = "XS",
                SizeOrderWeight = 100,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "S",
                SizeOrderWeight = 200,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "M",
                SizeOrderWeight = 300,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "L",
                SizeOrderWeight = 400,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "XL",
                SizeOrderWeight = 500,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "2XL",
                SizeOrderWeight = 600,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "3XL",
                SizeOrderWeight = 700,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            },
            new()
            {
                Amount = 0,
                SizeId = "4XL",
                SizeOrderWeight = 800,
                StorageId = 2,
                StorageName = vitebskStorage.Location,
                ItemId = 1
            }
        };

        //Act
        var result = await _resolver.ResolveAsync([1, 2], [1,2], _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
