using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.BusinessRules;

public class AllKitMinAmountsListedBusinessRuleTests
{
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly AllKitMinAmountsListedBusinessRule _businessRule;
    private readonly CancellationToken _token = CancellationToken.None;

    public AllKitMinAmountsListedBusinessRuleTests()
    {
        _businessRule = new AllKitMinAmountsListedBusinessRule(_dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNotAllStoragesAreListedReturnsFalse()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity>
            {
                new() { Id = 1, Location = "test" },
                new() { Id = 2, Location = "test"  },
                new() { Id = 3, Location = "test"  },
            }.BuildMockDbSet().Object);

        var minAmounts = new List<KitMinAmountDto> { new(1, 2), new(2, 2) };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(minAmounts, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenStoragesInMinAmountsDuplicatingReturnsFalse()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity>
            {
                new() { Id = 1, Location = "test"  },
                new() { Id = 2, Location = "test"  },
                new() { Id = 3, Location = "test"  },
            }.BuildMockDbSet().Object);

        var minAmounts = new List<KitMinAmountDto>
        {
            new(StorageId: 1, MinimumAmount: 2),
            new(StorageId: 2, MinimumAmount: 2),
            new(StorageId: 2, MinimumAmount: 2),
            new(StorageId: 3, MinimumAmount: 2)
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(minAmounts, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenStoragesInMinAmountsMoreThanInDatabaseReturnsFalse()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity>
            {
                new() { Id = 1, Location = "test"  },
                new() { Id = 2, Location = "test"  },
                new() { Id = 3, Location = "test"  },
            }.BuildMockDbSet().Object);

        var minAmounts = new List<KitMinAmountDto>
        {
            new(StorageId: 1, MinimumAmount: 2),
            new(StorageId: 2, MinimumAmount: 2),
            new(StorageId: 3, MinimumAmount: 2),
            new(StorageId: 4, MinimumAmount: 2)
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(minAmounts, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllStoragesAreListedReturnsTrue()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity>
            {
                new() { Id = 1, Location = "test"  },
                new() { Id = 2, Location = "test"  },
                new() { Id = 3, Location = "test"  },
            }.BuildMockDbSet().Object);

        var minAmounts = new List<KitMinAmountDto>
        {
            new(StorageId: 1, MinimumAmount: 2),
            new(StorageId: 2, MinimumAmount: 2),
            new(StorageId: 3, MinimumAmount: 2)
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(minAmounts, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllStoragesAreListedButnotOrderedReturnsTrue()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Warehouses)
            .Returns(new List<WarehouseEntity>
            {
                new() { Id = 1, Location = "test"  },
                new() { Id = 2, Location = "test"  },
                new() { Id = 3, Location = "test"  },
            }.BuildMockDbSet().Object);

        var minAmounts = new List<KitMinAmountDto>
        {
            new(StorageId: 3, MinimumAmount: 2),
            new(StorageId: 1, MinimumAmount: 2),
            new(StorageId: 2, MinimumAmount: 2)
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(minAmounts, _token);

        //Assert
        result.Should().BeTrue();

    }
}
