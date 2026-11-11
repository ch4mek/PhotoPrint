using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.BusinessRules;

public class AreCompoundsValidBusinessRuleTests
{
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly AreCompoundsValidBusinessRule _businessRule;
    private readonly CancellationToken _token = CancellationToken.None;

    public AreCompoundsValidBusinessRuleTests()
    {
        _businessRule = new AreCompoundsValidBusinessRule(_dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenOneOfCompoundsIsArchivedReturnsFalse()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new(){ Id = 1, Status = MerchItemStatus.Available, Name = "test" },
                new(){ Id = 2, Status = MerchItemStatus.Archived , Name = "test" },
                new(){ Id = 3, Status = MerchItemStatus.Available, Name = "test" },
            }.BuildMockDbSet().Object);

        var compounds = new List<AddKitCompoundDto>
        {
            new(ItemId: 1, AmountInKit: 1), new(ItemId: 2, AmountInKit: 1), new(ItemId: 3, AmountInKit: 1),
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(compounds, _token);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.NotAllCompoundsExistOrAvailable);

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNotAllCompoundsExistReturnsFalse()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new(){ Id = 1, Status = MerchItemStatus.Available, Name = "test" },
                new(){ Id = 2, Status = MerchItemStatus.Available, Name = "test" },
                new(){ Id = 4, Status = MerchItemStatus.Available, Name = "test" },
            }.BuildMockDbSet().Object);

        var compounds = new List<AddKitCompoundDto>
        {
            new(ItemId: 1, AmountInKit: 1), new(ItemId: 2, AmountInKit: 1), new(ItemId: 3, AmountInKit: 1),
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(compounds, _token);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.NotAllCompoundsExistOrAvailable);

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCompoundsContainsMoreThanOneClothingReturnsFalse()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new(){ Id = 1, Status = MerchItemStatus.Available, IsClothing = true , Name = "test"},
                new(){ Id = 2, Status = MerchItemStatus.Available, IsClothing = true , Name = "test"},
                new(){ Id = 3, Status = MerchItemStatus.Available, IsClothing = false, Name = "test"},
            }.BuildMockDbSet().Object);

        var compounds = new List<AddKitCompoundDto>
        {
            new(ItemId: 1, AmountInKit: 1), new(ItemId: 2, AmountInKit: 2), new(ItemId: 3, AmountInKit: 1),
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(compounds, _token);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.OnlyOneClothingInKit);

    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCompoundsExistAndContainOneClothingReturnsTrue()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new(){ Id = 1, Status = MerchItemStatus.Available, IsClothing = true , Name = "test" },
                new(){ Id = 2, Status = MerchItemStatus.Available, IsClothing = false, Name = "test" },
                new(){ Id = 3, Status = MerchItemStatus.Available, IsClothing = false, Name = "test" },
            }.BuildMockDbSet().Object);

        var compounds = new List<AddKitCompoundDto>
        {
            new(ItemId: 1, AmountInKit: 1), new(ItemId: 2, AmountInKit: 2), new(ItemId: 3, AmountInKit: 1),
        };

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(compounds, _token);

        //Assert
        result.IsSuccess.Should().BeTrue();

    }
}
