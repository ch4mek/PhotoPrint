using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.BusinessRules;

public class KitNameUniqueBusinessRuleTests
{
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly KitNameUniqueBusinessRule _businessRule;
    private readonly CancellationToken _token = CancellationToken.None;

    public KitNameUniqueBusinessRuleTests()
    {
        _businessRule = new KitNameUniqueBusinessRule(_dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNameIsNotUniqueAndIdNullReturnsFalse()
    {
        //Arrange
        var name = "kit name";

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() { Id = 1, Name = name}
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(name, null, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNameIsNotUniqueAndIdNotNullReturnsFalse()
    {
        //Arrange
        var name = "kit name";

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() { Id = 1, Name = name },
                new() { Id = 2, Name = "another name" },
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(name, 2, _token);

        //Assert
        result.Should().BeFalse();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNameIsUniqueAndIdNotNullReturnsTrue()
    {
        //Arrange
        var name = "kit name";

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() { Id = 1, Name = "another name" },
                new() { Id = 2, Name = name },
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(name, 2, _token);

        //Assert
        result.Should().BeTrue();

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNameIsUniqueAndIdNullReturnsTrue()
    {
        //Arrange
        var name = "kit name";

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() { Id = 1, Name = "another name" },
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(name, null, _token);

        //Assert
        result.Should().BeTrue();

    }
}
