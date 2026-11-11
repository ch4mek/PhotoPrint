using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.Composers;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.Composers;

public class ItemSizeMinAmountComposerTests
{
    private readonly ItemSizeMinAmountComposer _composer = new();

    [Fact]
    public void ComposeWhenMinAmountsExistShouldReturnDatabaseEntities()
    {
        //Arrange
        const int itemId = 1;

        var amounts = new List<SizeMinAmountCoreDto>
        {
            new("XS", 1, 1),
            new("S", 1, 1),
            new("M", 1, 1),
            new("L", 1, 1),
            new("XL", 1, 1),
            new("2XL", 1, 1),
            new("3XL", 1, 1),
            new("4XL", 1, 1),

            new("XS", 2, 2),
            new("S", 2, 2),
            new("M", 2, 2),
            new("L", 2, 2),
            new("XL", 2, 2),
            new("2XL", 2, 2),
            new("3XL", 2, 2),
            new("4XL", 2, 2),

            new("XS", 3, 3),
            new("S", 3, 3),
            new("M", 3, 3),
            new("L", 3, 3),
            new("XL", 3, 3),
            new("2XL", 3, 3),
            new("3XL", 3, 3),
            new("4XL", 3, 3),
        };

        var expectedEntities = new List<ItemSizeMinAmountEntity>
        {
            new () { SizeId = "XS", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "S", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "M", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "L", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "XL", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "2XL", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "3XL", ItemId = itemId, WarehouseId = 1, MinAmount = 1},
            new () { SizeId = "4XL", ItemId = itemId, WarehouseId = 1, MinAmount = 1},

            new () { SizeId = "XS", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "S", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "M", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "L", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "XL", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "2XL", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "3XL", ItemId = itemId, WarehouseId = 2, MinAmount = 2},
            new () { SizeId = "4XL", ItemId = itemId, WarehouseId = 2, MinAmount = 2},

            new () { SizeId = "XS", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "S", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "M", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "L", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "XL", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "2XL", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "3XL", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
            new () { SizeId = "4XL", ItemId = itemId, WarehouseId = 3, MinAmount = 3},
        };

        //Act
        var entities = _composer.Compose(itemId: itemId, amounts);

        //Assert
        entities.Should().BeEquivalentTo(expectedEntities);

    }
}
