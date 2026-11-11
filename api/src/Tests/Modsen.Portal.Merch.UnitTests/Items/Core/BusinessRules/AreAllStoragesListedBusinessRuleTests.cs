using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.BusinessRules;

public class AreAllStoragesListedBusinessRuleTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly AreAllStoragesListedBusinessRule _businessRule;

    public AreAllStoragesListedBusinessRuleTests()
    {
        _merchDbContextMock.Setup(x => x.Warehouses)
            .Returns(
                new List<WarehouseEntity>
                    {
                        new() { Id = 1, Location = "Minsk" },
                        new() { Id = 2, Location = "Vitebsk" },
                        new() { Id = 3, Location = "Warsaw" }
                    }.BuildMockDbSet()
                    .Object);

        _businessRule = new AreAllStoragesListedBusinessRule(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllStoragesAreListedReturnsTrue()
    {
        //Arrange
        var amounts = new List<SizeMinAmountCoreDto>
        {
            new("S", 1, 1),
            new("S", 2, 1),
            new("S", 3, 1),
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNotAllStoragesAreListedReturnsFalse()
    {
        //Arrange
        var amounts = new List<SizeMinAmountCoreDto>
        {
            new("S", 1, 1),
            new("S", 2, 1),
            new("S", 2, 1),
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, _token);

        //Assert
        result.Should().BeFalse();
    }
}
