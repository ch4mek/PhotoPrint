using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Transactions.Core.Composers;
using Modsen.Portal.Merch.Requests.Transactions.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Transactions.Requests;

public class AddMerchTransactionRequestTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();

    private readonly IReceiptTransactionsComposer _receiptTransactionsComposer;
    private readonly ITransferTransactionsComposer _transferTransactionsComposer;
    private readonly IWriteOffTransactionsComposer _writeOffTransactionsComposer;

    private readonly AddItemTransactionRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly MerchSizeEntity _xsSize;
    private readonly MerchSizeEntity _xlSize;
    private readonly WarehouseEntity _warehouse;
    private readonly ItemEntity _item;

    public AddMerchTransactionRequestTests()
    {
        _receiptTransactionsComposer = new ReceiptTransactionsComposer(_authenticatedUserContextMock.Object, _merchDbContextMock.Object);

        _transferTransactionsComposer = new TransferTransactionsComposer(
            _merchDbContextMock.Object,
            _authenticatedUserContextMock.Object);

        _writeOffTransactionsComposer = new WriteOffTransactionsComposer(
            _merchDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _identityContractMock.Object);

        _handler = new(
            _receiptTransactionsComposer,
            _writeOffTransactionsComposer,
            _transferTransactionsComposer,
            _merchDbContextMock.Object);

        _xsSize = new()
        {
            Id = "XS",
            Name = "test",
        };

        _xlSize = new()
        {
            Id = "XL",
            Name = "test2",
        };

        _warehouse = new WarehouseEntity
        {
            Id = 1,
            Location = "Minsk",
        };

        _item = new ItemEntity
        {
            Id = 1,
            Name = "test",
            ItemSizeMinAmounts =
            [
                new() { SizeId = _xsSize.Id, MerchSize = _xsSize, WarehouseId = _warehouse.Id, Warehouse = _warehouse },
                new() { SizeId = _xlSize.Id, MerchSize = _xlSize, WarehouseId = _warehouse.Id, Warehouse = _warehouse }
            ],
        };
    }

    [Fact]
    public async Task HandleWhenCorrectReceiptShouldReturnCountOfTransactions()
    {
        // Arrange
        SetupSizes([_xsSize]);
        SetupWarehouses([_warehouse]);
        SetupItems([_item]);
        SetupTransactions([]);

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.Receipt,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
        };

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenCorrectWriteOffShouldReturnCountOfTransactions()
    {
        // Arrange
        var receiptTransaction = new ItemTransactionEntity
        {
            WarehouseId = _warehouse.Id,
            ItemId = _item.Id,
            SizeId = _xsSize.Id,
            Count = 1,
        };

        var merchUser = new MerchUserEntity()
        {
            UserId = 1,
            Email = "test",
            LastName = "test",
            FirstName = "test"
        };

        SetupSizes([_xsSize]);
        SetupWarehouses([_warehouse]);
        SetupItems([_item]);
        SetupTransactions([receiptTransaction]);
        SetupUsers([merchUser]);

        _identityContractMock.Setup(x => x.IsActive(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _item.ItemTransactions = [receiptTransaction];

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.WriteOff,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
            UserRecipientId = merchUser.UserId,
        };

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenCorrectTransferShouldReturnCountOfTransactions()
    {
        // Arrange
        var receiptTransaction = new ItemTransactionEntity
        {
            WarehouseId = _warehouse.Id,
            ItemId = _item.Id,
            SizeId = _xsSize.Id,
            Count = 1,
        };

        var warehouse = new WarehouseEntity
        {
            Id = 2,
            Location = "Brest",
        };

        var merchUser = new MerchUserEntity()
        {
            UserId = 1,
            Email = "test",
            LastName = "test",
            FirstName = "test"
        };

        SetupSizes([_xsSize]);
        SetupWarehouses([_warehouse, warehouse]);
        SetupItems([_item]);
        SetupTransactions([receiptTransaction]);
        SetupUsers([merchUser]);

        _item.ItemTransactions = [receiptTransaction];

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.Transfer,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
            WarehouseRecipientId = warehouse.Id,
        };

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenWriteOffAndNotEnoughAmountShouldThrowException()
    {
        // Arrange
        var receiptTransaction = new ItemTransactionEntity
        {
            WarehouseId = _warehouse.Id,
            ItemId = _item.Id,
            SizeId = "XL",
            Count = 1,
        };

        var merchUser = new MerchUserEntity()
        {
            UserId = 1,
            Email = "test",
            LastName = "test",
            FirstName = "test"
        };

        _identityContractMock.Setup(x => x.IsActive(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        SetupSizes([_xsSize]);
        SetupWarehouses([_warehouse]);
        SetupItems([_item]);
        SetupTransactions([receiptTransaction]);
        SetupUsers([merchUser]);

        _item.ItemTransactions = [receiptTransaction];

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.WriteOff,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
            UserRecipientId = merchUser.UserId,
        };

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage("You cannot write off the requested amount. The amount is bigger than available in storage.");
    }

    [Fact]
    public async Task HandleWhenWarehouseIsNotExistShouldThrowNotFoundException()
    {
        // Arrange
        SetupSizes([_xsSize]);
        SetupWarehouses([]);
        SetupItems([_item]);
        SetupTransactions([]);

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.Receipt,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
        };

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Warehouse is not exist");
    }

    [Fact]
    public async Task HandleWhenItemIsNotExistShouldThrowNotFoundException()
    {
        // Arrange
        SetupSizes([_xsSize]);
        SetupWarehouses([_warehouse]);
        SetupItems([]);
        SetupTransactions([]);

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.Receipt,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
        };

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Item is not exist");
    }

    [Fact]
    public async Task HandleWhenItemIsNotContainSizeShouldThrowNotFoundException()
    {
        // Arrange
        SetupSizes([_xsSize]);
        SetupWarehouses([_warehouse]);
        SetupItems([_item]);
        SetupTransactions([]);

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.Receipt,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 1,
                },
                new()
                {
                    SizeId = "Bad size id",
                    Count = 1,
                }
            ],
            WarehouseId = _warehouse.Id,
        };

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage("Item is not contain size");
    }

    [Fact]
    public async Task HandleWhenZeroItemsShouldThrowInvalidDataAppException()
    {
        // Arrange
        SetupSizes([_xsSize, _xlSize]);
        SetupWarehouses([_warehouse]);
        SetupItems([_item]);
        SetupTransactions([]);

        var command = new AddItemTransactionRequest.Command
        {
            TransactionType = MerchTransactionType.Receipt,
            ItemId = _item.Id,
            SizeCounts =
            [
                new()
                {
                    SizeId = _xsSize.Id,
                    Count = 0,
                },
                new()
                {
                    SizeId = _xlSize.Id,
                    Count = 0,
                }
            ],
            WarehouseId = _warehouse.Id,
        };

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage("You cannot add transaction with 0 merch items");
    }

    private void SetupItems(List<ItemEntity> items)
    {
        _merchDbContextMock
            .Setup(mock => mock.Items)
            .Returns(items.BuildMockDbSet().Object);
    }

    private void SetupWarehouses(List<WarehouseEntity> warehouses)
    {
        _merchDbContextMock
            .Setup(mock => mock.Warehouses)
            .Returns(warehouses.BuildMockDbSet().Object);
    }

    private void SetupSizes(List<MerchSizeEntity> sizes)
    {
        _merchDbContextMock
            .Setup(mock => mock.MerchSizes)
            .Returns(sizes.BuildMockDbSet().Object);
    }

    private void SetupUsers(List<MerchUserEntity> users)
    {
        _merchDbContextMock
            .Setup(mock => mock.MerchUsers)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupTransactions(List<ItemTransactionEntity> transactions)
    {
        _merchDbContextMock
            .Setup(mock => mock.ItemTransactions)
            .Returns(transactions.BuildMockDbSet().Object);
    }
}
