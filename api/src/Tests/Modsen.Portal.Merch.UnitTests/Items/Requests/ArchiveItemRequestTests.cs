using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class ArchiveItemRequestTests
{
    private readonly Mock<IGetMerchItemByIdDbQuery> _getItemDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _userContextMock = new();
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly ArchiveItemRequest _handler;

    public ArchiveItemRequestTests()
    {
        _handler = new ArchiveItemRequest(
            _getItemDbQueryMock.Object,
            _userContextMock.Object,
            _merchDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenItemNotFoundThrowsInvalidDataAppException()
    {
        //Arrange
        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(It.IsAny<int>(), _token))
            .ReturnsAsync((ItemEntity?)null);

        var command = new ArchiveItemRequest.Command(1);

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.MerchItemNotFound);

    }

    [Fact]
    public async Task HandleWhenItemIsAlreadyArchivedReturnsStatusDtoWithoutDbChanges()
    {
        //Arrange
        var item = new ItemEntity { Id = 1, Name = "t-shirt", Status = MerchItemStatus.Archived };

        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(item.Id, _token))
            .ReturnsAsync(item);

        var command = new ArchiveItemRequest.Command(item.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Archived);
        result.Name.Should().Be(item.Name);
        result.Id.Should().Be(item.Id);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Never);
    }

    [Fact]
    public async Task HandleWhenItemIsInStockThrowsInvalidDataAppException()
    {
        //Arrange
        var item = new ItemEntity { Id = 1, Name = "t-shirt", Status = MerchItemStatus.Available };

        var transactions = new List<ItemTransactionEntity>
        {
            new() { Count = 1, ItemId = item.Id, WarehouseId = 1, SizeId = "XL" },
            new() { Count = 10, ItemId = item.Id, WarehouseId = 2, SizeId = "XL" },
            new() { Count = 1, ItemId = item.Id, WarehouseId = 3, SizeId = "XL" },
            new() { Count = -1, ItemId = item.Id, WarehouseId = 1 , SizeId = "XL"},
            new() { Count = -9, ItemId = item.Id, WarehouseId = 2 , SizeId = "XL"},
            new() { Count = -1, ItemId = item.Id, WarehouseId = 3 , SizeId = "XL"}
        };

        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(item.Id, _token))
            .ReturnsAsync(item);

        _merchDbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);

        var command = new ArchiveItemRequest.Command(item.Id);

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>("Items with existing stock cannot be archived. " +
            "To archive this item write-off it from Storage tab");
    }

    [Fact]
    public async Task HandleWhenItemIsOutOfStockAndNotInKitReturnStatusDtoWithDbChanges()
    {
        //Arrange
        var item = new ItemEntity { Id = 1, Name = "t-shirt", Status = MerchItemStatus.Available };
        const int userId = 1;

        var transactions = new List<ItemTransactionEntity>
        {
            new() { Count = 1, ItemId = item.Id, WarehouseId = 1, SizeId = "XL" },
            new() { Count = 10, ItemId = item.Id, WarehouseId = 2, SizeId = "XL" },
            new() { Count = 1, ItemId = item.Id, WarehouseId = 3 , SizeId = "XL"},
            new() { Count = -1, ItemId = item.Id, WarehouseId = 1 , SizeId = "XL"},
            new() { Count = -10, ItemId = item.Id, WarehouseId = 2 , SizeId = "XL"},
            new() { Count = -1, ItemId = item.Id, WarehouseId = 3, SizeId = "XL" }
        };

        _userContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(item.Id, _token))
            .ReturnsAsync(item);

        _merchDbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);

        _merchDbContextMock
            .Setup(context => context.KitItems)
            .Returns(new List<KitItemEntity>().BuildMockDbSet().Object);

        var command = new ArchiveItemRequest.Command(item.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Archived);
        result.Name.Should().Be(item.Name);
        result.Id.Should().Be(item.Id);

        item.ArchivedById.Should().Be(userId);
        item.UpdatedById.Should().Be(userId);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Once);
    }

    [Fact]
    public async Task HandleWhenItemIsOutOfStockAndInKitReturnStatusDtoWithDbChangesAndArchiveKit()
    {
        //Arrange
        var item = new ItemEntity { Id = 1, Name = "t-shirt", Status = MerchItemStatus.Available };
        var kit = new KitEntity { Id = 1, Name = "t-shirt kit", Status = MerchItemStatus.Available };
        const int userId = 1;

        var transactions = new List<ItemTransactionEntity>
        {
            new() { Count = 1, ItemId = item.Id, WarehouseId = 1 , SizeId = "XL"},
            new() { Count = 10, ItemId = item.Id, WarehouseId = 2 , SizeId = "XL"},
            new() { Count = 1, ItemId = item.Id, WarehouseId = 3 , SizeId = "XL"},
            new() { Count = -1, ItemId = item.Id, WarehouseId = 1 , SizeId = "XL"},
            new() { Count = -10, ItemId = item.Id, WarehouseId = 2, SizeId = "XL" },
            new() { Count = -1, ItemId = item.Id, WarehouseId = 3, SizeId = "XL" }
        };

        _userContextMock
            .Setup(context => context.UserId)
            .Returns(userId);

        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(item.Id, _token))
            .ReturnsAsync(item);

        _merchDbContextMock
            .Setup(context => context.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);

        _merchDbContextMock
            .Setup(context => context.KitItems)
            .Returns(new List<KitItemEntity>
            {
                new() { Count = 1, ItemId = 1, KitId = 1, Kit = kit}
            }.BuildMockDbSet().Object);

        _merchDbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity> { kit }.BuildMockDbSet().Object);

        var command = new ArchiveItemRequest.Command(item.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Archived);
        result.Name.Should().Be(item.Name);
        result.Id.Should().Be(item.Id);

        item.ArchivedById.Should().Be(userId);
        item.UpdatedById.Should().Be(userId);

        kit.Status.Should().Be(MerchItemStatus.Archived);
        kit.ArchivedById.Should().Be(userId);
        kit.UpdatedById.Should().Be(userId);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Once);
    }
}
