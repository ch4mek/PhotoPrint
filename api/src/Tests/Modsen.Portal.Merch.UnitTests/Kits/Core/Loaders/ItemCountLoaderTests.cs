using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Core.Loaders;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Loaders;

public class ItemCountLoaderTests
{
    private readonly ItemsCountLoader _loader;
    private readonly Mock<IMerchDbContext> _dbContext = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private static readonly List<ItemTransactionEntity> ItemTransactionsSet =
    [
        new ItemTransactionEntity
        {
            Id = 1,
            Type = MerchTransactionType.Receipt,
            OrderWeight = 100,
            GroupCode = Guid.CreateVersion7(),
            Group = null,
            ItemId = 1,
            SizeId = "XL",
            WarehouseId = 1,
            SenderId = null,
            WarehouseRecipientId = null,
            Count = 11,
            CreatedById = 12
        },
        new ItemTransactionEntity
        {
            Id = 2,
            Type = MerchTransactionType.Receipt,
            OrderWeight = 200,
            GroupCode = Guid.CreateVersion7(),
            Group = null,
            ItemId = 2,
            SizeId = "L",
            WarehouseId = 1,
            SenderId = null,
            WarehouseRecipientId = null,
            Count = 22,
            CreatedById = 12
        },
        new ItemTransactionEntity
        {
            Id = 3,
            Type = MerchTransactionType.Receipt,
            OrderWeight = 300,
            GroupCode = Guid.CreateVersion7(),
            Group = null,
            ItemId = 3,
            SizeId = "M",
            WarehouseId = 1,
            SenderId = null,
            WarehouseRecipientId = null,
            Count = 33,
            CreatedById = 12
        }
    ];

    private static readonly List<ItemEntity> ItemsSet =
    [
        new ItemEntity
        {
            Id = 1,
            Name = "Item 1",
            IsClothing = true,
            Supplier = "supplier 1",
            Status = MerchItemStatus.Available,
            CreatedById = 1,
            CreatedAtUtc = DateTime.UtcNow,
            ItemTransactions = [ItemTransactionsSet![0]]
        },
        new ItemEntity
        {
            Id = 2,
            Name = "Item 2",
            IsClothing = true,
            Supplier = "supplier 2",
            Status = MerchItemStatus.Available,
            CreatedById = 2,
            CreatedAtUtc = DateTime.UtcNow,
            ItemTransactions = [ItemTransactionsSet![1]]
        },
        new ItemEntity
        {
            Id = 3,
            Name = "Item 3",
            IsClothing = true,
            Supplier = "supplier 3",
            Status = MerchItemStatus.Available,
            CreatedById = 3,
            CreatedAtUtc = DateTime.UtcNow,
        }
    ];

    private static readonly List<WarehouseEntity> WarehousesSet =
    [
        new WarehouseEntity
        {
            Id = 1,
            Location = "Minsk",
            ItemTransactions = ItemTransactionsSet!
        }
    ];

    private static readonly List<ItemSizeMinAmountEntity> ItemSizeMinAmountsSet =
    [
        new ItemSizeMinAmountEntity
        {
            ItemId = 1,
            Item = ItemsSet[0],
            SizeId = "XL",
            WarehouseId = 1,
            Warehouse = WarehousesSet[0],
            MinAmount = 12
        },
        new ItemSizeMinAmountEntity
        {
            ItemId = 2,
            Item = ItemsSet[1],
            SizeId = "L",
            WarehouseId = 1,
            Warehouse = WarehousesSet[0],
            MinAmount = 144
        },
        new ItemSizeMinAmountEntity
        {
            ItemId = 3,
            Item = ItemsSet[2],
            SizeId = "M",
            WarehouseId = 1,
            Warehouse = WarehousesSet[0],
            MinAmount = 81
        },
    ];

    public ItemCountLoaderTests()
    {
        _loader = new ItemsCountLoader(_dbContext.Object);
    }

    [Fact]
    public async Task LoadAsyncWhenItemsNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        var itemsDbSet = ItemsSet.BuildMockDbSet();

        _dbContext
            .Setup(x => x.Items)
            .Returns(itemsDbSet.Object);

        // Act
        var act = () => _loader.LoadAsync([1, 2, 3, 4], It.IsAny<int>(), _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task LoadAsyncWhenWarehousesNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        var itemsDbSet = ItemsSet.BuildMockDbSet();
        var warehousesDbSet = WarehousesSet.BuildMockDbSet();

        _dbContext
            .Setup(x => x.Items)
            .Returns(itemsDbSet.Object);

        _dbContext
            .Setup(x => x.Warehouses)
            .Returns(warehousesDbSet.Object);

        // Act
        var act = () => _loader.LoadAsync([1, 2, 3], 2, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task LoadAsyncWhenRequestIsValidShouldReturnItemCount()
    {
        // Arrange
        var itemsDbSet = ItemsSet.BuildMockDbSet();
        var warehousesDbSet = WarehousesSet.BuildMockDbSet();
        var transactionsDbSet = ItemTransactionsSet.BuildMockDbSet();
        var itemSizeMinAmountDbSet = ItemSizeMinAmountsSet.BuildMockDbSet();

        _dbContext
            .Setup(x => x.Items)
            .Returns(itemsDbSet.Object);

        _dbContext
            .Setup(x => x.Warehouses)
            .Returns(warehousesDbSet.Object);

        _dbContext
            .Setup(x => x.ItemTransactions)
            .Returns(transactionsDbSet.Object);

        _dbContext
            .Setup(x => x.ItemSizeMinAmounts)
            .Returns(itemSizeMinAmountDbSet.Object);

        var expected = new List<ItemSizeCountDto>
        {
            new ItemSizeCountDto
            {
                ItemId = 1,
                SizeCounts = [new SizeCountDto { SizeId = "XL", Count = 11 }]
            },
            new ItemSizeCountDto
            {
                ItemId = 2,
                SizeCounts = [new SizeCountDto { SizeId = "L", Count = 22 }]
            },
            new ItemSizeCountDto
            {
                ItemId = 3,
                SizeCounts = [new SizeCountDto { SizeId = "M", Count = 33 }]
            }
        };

        // Act
        var result = await _loader.LoadAsync([1, 2, 3], 1, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task LoadSingleAsyncWhenItemIsNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        var itemsDbSet = ItemsSet.BuildMockDbSet();

        _dbContext
            .Setup(x => x.Items)
            .Returns(itemsDbSet.Object);

        // Act
        var act = () => _loader.LoadAsync(4, It.IsAny<int>(), _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

}

