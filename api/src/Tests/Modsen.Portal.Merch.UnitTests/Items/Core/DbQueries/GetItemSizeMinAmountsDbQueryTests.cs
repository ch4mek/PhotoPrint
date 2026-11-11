using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.DbQueries;

public class GetItemSizeMinAmountsDbQueryTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetItemSizeMinAmountsDbQuery _dbQuery;

    public GetItemSizeMinAmountsDbQueryTests()
    {

        _dbQuery = new GetItemSizeMinAmountsDbQuery(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task ExecuteAsyncWhenMinAmountsExistReturnsMinAmounts()
    {
        //Arrange
        const int itemId = 1;

        var minAmounts = new List<ItemSizeMinAmountEntity>
        {
            new()
            {
                ItemId = itemId,
                SizeId = "One",
                WarehouseId = 1,
                MerchSize = new MerchSizeEntity {OrderWeight = 0, Name = "test", Id = "XL" }
            },
            new()
            {
                ItemId = itemId,
                SizeId = "One",
                WarehouseId = 2,
                MerchSize = new MerchSizeEntity {OrderWeight = 0, Name = "test", Id = "XL" }
            },
            new()
            {
                ItemId = itemId,
                SizeId = "One",
                WarehouseId = 3,
                MerchSize = new MerchSizeEntity {OrderWeight = 0, Name = "test", Id = "XL" }
            }
        };

        _merchDbContextMock.Setup(x => x.ItemSizeMinAmounts)
            .Returns(minAmounts.BuildMockDbSet().Object);

        //Act
        var result = await _dbQuery.ExecuteAsync(itemId, _token);

        //Assert
        result.Should().BeEquivalentTo(minAmounts);
    }

    [Fact]
    public async Task ExecuteAsyncWhenMinAmountsNotExistReturnsEmptyList()
    {
        //Arrange
        const int itemId = 1;

        var minAmounts = new List<ItemSizeMinAmountEntity>
        {
            new()
            {
                ItemId = itemId,
                SizeId = "One",
                WarehouseId = 1,
                MerchSize = new MerchSizeEntity {OrderWeight = 0, Name = "test", Id = "XL" }
            },
            new()
            {
                ItemId = itemId,
                SizeId = "One",
                WarehouseId = 2,
                MerchSize = new MerchSizeEntity {OrderWeight = 0, Name = "test", Id = "XL" }
            },
            new()
            {
                ItemId = itemId,
                SizeId = "One",
                WarehouseId = 3,
                MerchSize = new MerchSizeEntity {OrderWeight = 0, Name = "test", Id = "XL" }
            }
        };

        _merchDbContextMock.Setup(x => x.ItemSizeMinAmounts)
            .Returns(minAmounts.BuildMockDbSet().Object);

        //Act
        var result = await _dbQuery.ExecuteAsync(itemId + 1, _token);

        //Assert
        result.Should().BeEmpty();
    }
}
