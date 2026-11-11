using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Transactions.Requests;
using Modsen.Portal.Merch.Requests.Transactions.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Transactions.Requests
{
    public class GetMerchTransactionsRequestTests
    {
        private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
        private readonly GetTransactionsRquest _handler;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        private readonly MerchUserEntity _user;
        private readonly WarehouseEntity _warehouse;
        private readonly ItemEntity _item;
        private readonly MerchSizeEntity _size;

        public GetMerchTransactionsRequestTests()
        {
            _handler = new GetTransactionsRquest(_merchDbContextMock.Object);

            _user = new MerchUserEntity
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _warehouse = new WarehouseEntity
            {
                Id = 1,
                Location = "Minsk"
            };

            _size = new MerchSizeEntity
            {
                Id = "M",
                Name = "Medium"
            };

            _item = new ItemEntity
            {
                Id = 1,
                Name = "Test Item",
                IsClothing = true,
            };
        }

        [Fact]
        [InlineData(PortalRoleNames.Cto)]
        public async Task HandleWhenNoTransactionsShouldReturnEmptyList()
        {
            // Arrange
            SetupTransactionGroups([]);
            SetupTransactions([]);
            var queryParameters = new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "50",
                [nameof(ODataRawQueryOptions.Skip)] = "0"
            };

            var q = ODataQueryOptionsMock.BuildMockWithKey<TransationOdpDto>(queryParameters, nameof(TransationOdpDto.GroupCode));
            var query = new GetTransactionsRquest.Query(q, null, null);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task HandleWhenWithTransactionsShouldReturnCorrectCount()
        {
            // Arrange
            var groupCode = Guid.CreateVersion7();
            var senderWarehouse = new WarehouseEntity { Id = 2, Location = "Sender Location" };
            var recipientWarehouse = new WarehouseEntity { Id = 3, Location = "Recipient Location" };

            var transactionGroup = new ItemTransactionGroupEntity
            {
                GroupCode = groupCode,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = _user.UserId,
                CreatedByUser = _user,
                Notes = "Test notes",
                ItemTransactions =
                [
                    new ItemTransactionEntity
                    {
                        Id = 1,
                        GroupCode = groupCode,
                        ItemId = _item.Id,
                        Item = _item,
                        SizeId = _size.Id,
                        Count = 5,
                        Type = MerchTransactionType.Receipt,
                        OrderWeight = 1,
                        WarehouseId = _warehouse.Id,
                        Warehouse = _warehouse,
                        SenderId = senderWarehouse.Id,
                        Sender = senderWarehouse,
                        UserRecipientId = null,
                        WarehouseRecipientId = recipientWarehouse.Id,
                        WarehouseRecipient = recipientWarehouse
                    }
                ]
            };

            SetupTransactionGroups([transactionGroup]);
            SetupTransactions(transactionGroup.ItemTransactions);

            var queryParameters = new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "50",
                [nameof(ODataRawQueryOptions.Skip)] = "0"
            };

            var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<TransationOdpDto>(
                queryParameters,
                nameof(TransationOdpDto.GroupCode));

            var query = new GetTransactionsRquest.Query(queryOptions, null, null);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(true, -30)]
        [InlineData(false, -5)]
        public async Task HandleWithFullDateRangeFilterShouldReturnFilteredResults(bool isOutOfDateRange, int days)
        {
            // Arrange
            var groupCode = Guid.CreateVersion7();
            var senderWarehouse = new WarehouseEntity { Id = 2, Location = "Sender Location" };
            var recipientWarehouse = new WarehouseEntity { Id = 3, Location = "Recipient Location" };

            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(5);

            var transactionGroup = new ItemTransactionGroupEntity
            {
                GroupCode = groupCode,
                CreatedAtUtc = DateTime.UtcNow.AddDays(days),
                CreatedById = _user.UserId,
                CreatedByUser = _user,
                Notes = "Test notes",
                ItemTransactions =
                [
                    new ItemTransactionEntity
            {
                Id = 1,
                GroupCode = groupCode,
                ItemId = _item.Id,
                Item = _item,
                SizeId = _size.Id,
                Count = 5,
                Type = MerchTransactionType.Receipt,
                OrderWeight = 1,
                WarehouseId = _warehouse.Id,
                Warehouse = _warehouse,
                SenderId = senderWarehouse.Id,
                Sender = senderWarehouse,
                UserRecipientId = null,
                WarehouseRecipientId = recipientWarehouse.Id,
                WarehouseRecipient = recipientWarehouse
            }
                ]
            };

            SetupTransactionGroups([transactionGroup]);
            SetupTransactions(transactionGroup.ItemTransactions);

            var queryParameters = new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "50",
                [nameof(ODataRawQueryOptions.Skip)] = "0",
                [nameof(ODataRawQueryOptions.Filter)] = $"Date ge {startDate:yyyy-MM-dd}T00:00:00Z and Date le {endDate:yyyy-MM-dd}T23:59:59Z"
            };

            var queryOptions = ODataQueryOptionsMock.BuildMockWithKey<TransationOdpDto>(queryParameters, nameof(TransationOdpDto.GroupCode));
            var query = new GetTransactionsRquest.Query(queryOptions, null, null);

            // Act
            var result = await _handler.Handle(query, _cancellationToken);

            // Assert
            result.Should().NotBeNull();
            if (isOutOfDateRange)
            {
                result.Items.Should().HaveCount(0);
            }
            else
            {
                result.Items.Should().HaveCount(1);
            }
        }

        private void SetupTransactionGroups(List<ItemTransactionGroupEntity> transactionGroups)
        {
            _merchDbContextMock
                .Setup(mock => mock.ItemTransactionGroups)
                .Returns(transactionGroups.BuildMockDbSet().Object);
        }

        private void SetupTransactions(List<ItemTransactionEntity> transactions)
        {
            _merchDbContextMock
                .Setup(mock => mock.ItemTransactions)
                .Returns(transactions.BuildMockDbSet().Object);
        }
    }
}
