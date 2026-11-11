using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.BusinessRules;

public class AreAllSizesListedBusinessRuleTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly AreAllSizesListedBusinessRule _businessRule;

    public AreAllSizesListedBusinessRuleTests()
    {
        _merchDbContextMock.Setup(x => x.MerchSizes)
            .Returns(new List<MerchSizeEntity>
            {
                new() { Id = "XS", Name = "Extra small", OrderWeight = 100 },
                new() { Id = "S", Name = "Small", OrderWeight = 200 },
                new() { Id = "M", Name = "Medium", OrderWeight = 300 },
                new() { Id = "L", Name = "Large", OrderWeight = 400 },
                new() { Id = "XL", Name = "Extra large", OrderWeight = 500 },
                new() { Id = "2XL", Name = "Double extra large", OrderWeight = 600 },
                new() { Id = "3XL", Name = "Triple extra large", OrderWeight = 700 },
                new() { Id = "4XL", Name = "Quadruple extra large", OrderWeight = 800 },
                new() { Id = "One", Name = "One size", OrderWeight = 0 }
            }.BuildMockDbSet().Object);

        _businessRule = new AreAllSizesListedBusinessRule(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllSizesListedAndIsClothingReturnsTrue()
    {
        //Arrange
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
        };

        const bool isClothing = true;

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, isClothing, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllSizesListedAndIsNotClothingReturnsTrue()
    {
        //Arrange
        var amounts = new List<SizeMinAmountCoreDto>
        {
            new("One", 1, 1),
        };

        const bool isClothing = false;

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, isClothing, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenWrongSizesListedAndIsNotClothingReturnsFalse()
    {
        //Arrange
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
        };

        const bool isClothing = false;

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, isClothing, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenWrongSizesListedAndIsClothingReturnsFalse()
    {
        //Arrange
        var amounts = new List<SizeMinAmountCoreDto>
        {
            new("One", 1, 1),
        };

        const bool isClothing = true;

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, isClothing, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetNotAllSizesForClothing))]
    private async Task IsCorrespondsToBusiness_WhenNotAllSizesListedAndIsClothing_ReturnsFalse(
        List<SizeMinAmountCoreDto> amounts)
    {
        //Arrange
        const bool isClothing = false;

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(amounts, isClothing, _token);

        //Assert
        result.Should().BeFalse();
    }

    public static IEnumerable<object[]> GetNotAllSizesForClothing()
    {
        yield return
        [
            new List<SizeMinAmountCoreDto>
            {
                new("XS", 1, 1),
                new("S", 1, 1),
                new("M", 1, 1),
                new("L", 1, 1),
                new("XL", 1, 1),
                new("2XL", 1, 1),
                new("3XL", 1, 1),
            }
        ];
        yield return
        [
            new List<SizeMinAmountCoreDto>
            {
                new("XS", 1, 1),
            }
        ];
        yield return
        [
            new List<SizeMinAmountCoreDto>
            {
                new("XS", 1, 1),
                new("S", 1, 1),
                new("L", 1, 1),
                new("XL", 1, 1),
                new("2XL", 1, 1),
                new("3XL", 1, 1),
                new("4XL", 1, 1),
            }
        ];
        yield return
        [
            new List<SizeMinAmountCoreDto>
            {
                new ("XS", 1, 1),
                new ("S", 1, 1),
                new ("M", 1, 1),
                new ("L", 1, 1),
                new ("XL", 1, 1),
                new ("2XL", 1, 1),
                new ("3XL", 1, 1),
                new ("4XL", 1, 1),
                new ("One", 1, 1)
            }
        ];
    }
}
