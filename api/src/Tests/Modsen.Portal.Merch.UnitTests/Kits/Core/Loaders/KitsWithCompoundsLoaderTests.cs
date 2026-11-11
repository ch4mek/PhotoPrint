using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Loaders;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Loaders;

public class KitsWithCompoundsLoaderTests
{
    private readonly Mock<IMerchDbContext> _dbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly KitsWithCompoundsLoader _loader;

    private static readonly WarehouseEntity MinskStorage = new() { Id = 1, Location = "Minsk" };
    private static readonly WarehouseEntity VitebskStorage = new() { Id = 2, Location = "Vitebsk" };
    private static readonly WarehouseEntity WarsawStorage = new() { Id = 3, Location = "Warsaw" };

    private static readonly List<KitEntity> Kits =
    [
        new()
        {
            Id = 1,
            Name = "Hoodie and cup",
            Description = "Kit Description",
            PhotoFileId = Guid.CreateVersion7().ToString(),
            KitMinAmounts =
            [
                new() { MinAmount = 1, WarehouseId = 1, Warehouse = MinskStorage },
                new() { MinAmount = 1, WarehouseId = 2, Warehouse = VitebskStorage },
                new() { MinAmount = 1, WarehouseId = 3, Warehouse = WarsawStorage }
            ]
        },

        new()
        {
            Id = 2,
            Name = "Postcards",
            Description = "Kit of Postcards Description",
            PhotoFileId = Guid.CreateVersion7().ToString(),
            KitMinAmounts =
            [
                new() { MinAmount = 1, WarehouseId = 1, Warehouse = MinskStorage },
                new() { MinAmount = 1, WarehouseId = 2, Warehouse = VitebskStorage },
                new() { MinAmount = 1, WarehouseId = 3, Warehouse = WarsawStorage }
            ]
        }
    ];

    private static readonly List<KitItemEntity> KitItems =
    [
        new()
        {
            Id = 1,
            KitId = Kits.First().Id,
            Count = 1,
            ItemId = 1,
            Item = new ItemEntity { Name = "Hoodie", IsClothing = true }
        },

        new()
        {
            Id = 2,
            KitId = Kits.First().Id,
            Count = 2,
            ItemId = 2,
            Item = new ItemEntity { Name = "Cup", IsClothing = false }
        },

        new()
        {
            Id = 3,
            KitId = Kits[1].Id,
            Count = 1,
            ItemId = 3,
            Item = new ItemEntity { Name = "Postcard 1", IsClothing = false }
        },

        new()
        {
            Id = 4,
            KitId = Kits[1].Id,
            Count = 1,
            ItemId = 4,
            Item = new ItemEntity { Name = "Postcard 2", IsClothing = false }
        }
    ];

    private static readonly List<ItemTransactionEntity> Transactions =
    [
        new()
        {
            ItemId = 1,
            WarehouseId = 1,
            Warehouse = MinskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 1,
            WarehouseId = 2,
            Warehouse = VitebskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 1,
            WarehouseId = 3,
            Warehouse = WarsawStorage,
            Count = 1,
            SizeId = "XL"
        },

        new()
        {
            ItemId = 2,
            WarehouseId = 1,
            Warehouse = MinskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 2,
            WarehouseId = 2,
            Warehouse = VitebskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 2,
            WarehouseId = 3,
            Warehouse = WarsawStorage,
            Count = 1,
            SizeId = "XL"
        },

        new()
        {
            ItemId = 3,
            WarehouseId = 1,
            Warehouse = MinskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 3,
            WarehouseId = 2,
            Warehouse = VitebskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 3,
            WarehouseId = 3,
            Warehouse = WarsawStorage,
            Count = 1,
            SizeId = "XL"
        },

        new()
        {
            ItemId = 4,
            WarehouseId = 1,
            Warehouse = MinskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 4,
            WarehouseId = 2,
            Warehouse = VitebskStorage,
            Count = 1,
            SizeId = "XL"
        },
        new()
        {
            ItemId = 4,
            WarehouseId = 3,
            Warehouse = WarsawStorage,
            Count = 1,
            SizeId = "XL"
        }
    ];

    public KitsWithCompoundsLoaderTests()
    {
        Kits[0].KitItems = KitItems.Where(kit => kit.KitId == 1).ToList();
        Kits[1].KitItems = KitItems.Where(kit => kit.KitId == 2).ToList();

        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity>
            {
                MinskStorage,
                VitebskStorage,
                WarsawStorage
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(Kits.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitItems)
            .Returns(KitItems.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(Transactions.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 1, Warehouse = MinskStorage },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 1, Warehouse = VitebskStorage },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1, Warehouse = WarsawStorage },

                new() { KitId = 2, WarehouseId = 1, MinAmount = 1, Warehouse = MinskStorage },
                new() { KitId = 2, WarehouseId = 2, MinAmount = 1, Warehouse = VitebskStorage },
                new() { KitId = 2, WarehouseId = 3, MinAmount = 1, Warehouse = WarsawStorage },
            }.BuildMockDbSet().Object);

        _loader = new KitsWithCompoundsLoader(_dbContextMock.Object);
    }

    [Fact]
    public async Task LoadAsyncReturnsValidKitsWithCompounds()
    {
        //Arrange
        var expectedResult = new List<KitWithCompoundsCoreDto>
        {
            new()
            {
                Id = Kits[0].Id,
                Name = Kits[0].Name,
                Description = Kits[0].Description,
                PhotoFileId = Kits[0].PhotoFileId,
                ContainsClothing = true,
                AllStorages = [MinskStorage.Location, VitebskStorage.Location, WarsawStorage.Location],
                Compounds =
                [
                    new()
                    {
                        Name = KitItems[0].Item.Name,
                        AmountInKit = KitItems[0].Count,
                        Id = KitItems[0].ItemId,
                        IsClothing = KitItems[0].Item.IsClothing
                    },

                    new()
                    {
                        Name = KitItems[1].Item.Name,
                        AmountInKit = KitItems[1].Count,
                        Id = KitItems[1].ItemId,
                        IsClothing = KitItems[1].Item.IsClothing
                    }
                ]
            },
            new()
            {
                Id = Kits[1].Id,
                Name = Kits[1].Name,
                Description = Kits[1].Description,
                PhotoFileId = Kits[1].PhotoFileId,
                ContainsClothing = false,
                AllStorages = [MinskStorage.Location, VitebskStorage.Location, WarsawStorage.Location],
                Compounds =
                [
                    new()
                    {
                        Name = KitItems[2].Item.Name,
                        AmountInKit = KitItems[2].Count,
                        Id = KitItems[2].ItemId,
                        IsClothing = KitItems[2].Item.IsClothing
                    },

                    new()
                    {
                        Name = KitItems[3].Item.Name,
                        AmountInKit = KitItems[3].Count,
                        Id = KitItems[3].ItemId,
                        IsClothing = KitItems[3].Item.IsClothing
                    }
                ]
            }
        };

        //Act
        var result = await _loader.LoadAsync([1, 2],null , _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task LoadAsyncWhenOnlyOneIdProvidedReturnsValidKitWithCompounds()
    {
        //Arrange
        var expectedResult = new List<KitWithCompoundsCoreDto>
        {
            new()
            {
                Id = Kits[1].Id,
                Name = Kits[1].Name,
                Description = Kits[1].Description,
                PhotoFileId = Kits[1].PhotoFileId,
                ContainsClothing = false,
                AllStorages = [MinskStorage.Location, VitebskStorage.Location, WarsawStorage.Location],
                Compounds =
                [
                    new()
                    {
                        Name = KitItems[2].Item.Name,
                        AmountInKit = KitItems[2].Count,
                        Id = KitItems[2].ItemId,
                        IsClothing = KitItems[2].Item.IsClothing
                    },

                    new()
                    {
                        Name = KitItems[3].Item.Name,
                        AmountInKit = KitItems[3].Count,
                        Id = KitItems[3].ItemId,
                        IsClothing = KitItems[3].Item.IsClothing
                    }
                ]
            }
        };

        //Act
        var result = await _loader.LoadAsync([2], null, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task LoadAsyncWhenStorageIdProvidedReturnsValidKitWithCompoundsAndContainsOnlyOneStorage()
    {
        //Arrange
        var expectedResult = new List<KitWithCompoundsCoreDto>
        {
            new()
            {
                Id = Kits[1].Id,
                Name = Kits[1].Name,
                Description = Kits[1].Description,
                PhotoFileId = Kits[1].PhotoFileId,
                ContainsClothing = false,
                AllStorages = [MinskStorage.Location],
                Compounds =
                [
                    new()
                    {
                        Name = KitItems[2].Item.Name,
                        AmountInKit = KitItems[2].Count,
                        Id = KitItems[2].ItemId,
                        IsClothing = KitItems[2].Item.IsClothing
                    },

                    new()
                    {
                        Name = KitItems[3].Item.Name,
                        AmountInKit = KitItems[3].Count,
                        Id = KitItems[3].ItemId,
                        IsClothing = KitItems[3].Item.IsClothing
                    }
                ]
            }
        };

        //Act
        var result = await _loader.LoadAsync([2], [1], _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task LoadAsyncWhenKitsIdsNotProvidedReturnsEmptyList()
    {
        //Arrange
        var expectedResult = new List<KitWithCompoundsCoreDto>();

        //Act
        var result = await _loader.LoadAsync(new int[1], null, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
