using MockQueryable.Moq;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.BusinessRules;

public class IsItemNameUniqueBusinessRuleTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();

    private readonly IsItemNameUniqueBusinessRule _businessRule;

    public IsItemNameUniqueBusinessRuleTests()
    {
        _businessRule = new IsItemNameUniqueBusinessRule(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNewItemNameIsUniqueReturnsTrue()
    {
        //Arrange
        _merchDbContextMock.Setup(x => x.Items)
            .Returns(new List<ItemEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Name"
                }
            }.BuildMockDbSet().Object);

        int? id = null;
        var uniqueName = Guid.CreateVersion7().ToAlphabetic();

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(id: id, name: uniqueName, cancellationToken: CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNewItemNameIsNotUniqueReturnsFalse()
    {
        //Arrange
        var name = Guid.CreateVersion7().ToAlphabetic();

        _merchDbContextMock.Setup(x => x.Items)
            .Returns(new List<ItemEntity>
            {
                new()
                {
                    Id = 1,
                    Name = name
                }
            }.BuildMockDbSet().Object);

        int? id = null;

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(id: id, name: name, cancellationToken: CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenExistingItemNameNotChangeAndIsUniqueReturnsTrue()
    {
        //Arrange
        var name = Guid.CreateVersion7().ToAlphabetic();
        var id = 2;

        _merchDbContextMock.Setup(x => x.Items)
            .Returns(new List<ItemEntity>
            {
                new()
                {
                    Id = 1,
                    Name = Guid.CreateVersion7().ToAlphabetic()
                },
                new()
                {
                    Id = id,
                    Name = name
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(id: id, name: name, cancellationToken: CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenExistingItemNameIsUniqueReturnsTrue()
    {
        //Arrange
        var name = Guid.CreateVersion7().ToAlphabetic();
        var id = 2;

        _merchDbContextMock.Setup(x => x.Items)
            .Returns(new List<ItemEntity>
            {
                new()
                {
                    Id = 1,
                    Name = Guid.CreateVersion7().ToAlphabetic()
                },
                new()
                {
                    Id = id,
                    Name = Guid.CreateVersion7().ToAlphabetic()
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(id: id, name: name, cancellationToken: CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenExistingItemNameIsNotUniqueReturnsFalse()
    {
        //Arrange
        var name = Guid.CreateVersion7().ToAlphabetic();
        var id = 2;

        _merchDbContextMock.Setup(x => x.Items)
            .Returns(new List<ItemEntity>
            {
                new()
                {
                    Id = 1,
                    Name = name
                },
                new()
                {
                    Id = id,
                    Name = Guid.CreateVersion7().ToAlphabetic()
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(id: id, name: name, cancellationToken: CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }
}
