using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Requests;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests;

public class GetKitByIdRequestTests
{
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly GetKitByIdRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetKitByIdRequestTests()
    {
        _handler = new GetKitByIdRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenKitIsNullThrowsEntityNotFoundException()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 2,
                    Name = "Kit 1",
                    Description = "Kit 1 description",
                    KitItems =
                    [
                        new() { ItemId = 1, Item = new ItemEntity { IsClothing = false, Name = "Cup" }, Count = 2 }
                    ],
                    KitMinAmounts = [new() { WarehouseId = 1, MinAmount = 2 }],
                    Status = MerchItemStatus.Available
                }
            }.BuildMockDbSet().Object);

        var request = new GetKitByIdRequest.Query(1);

        //Act
        var action = () => _handler.Handle(request, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.KitNotFound);

    }

    [Fact]
    public async Task HandleWhenKitExistsReturnsKitDto()
    {
        //Arrange
        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1",
                    Description = "Kit 1 description",
                    KitItems =
                    [
                        new() { ItemId = 1, Item = new ItemEntity { IsClothing = false, Name = "Cup" }, Count = 2 }
                    ],
                    KitMinAmounts = [new() { WarehouseId = 1, MinAmount = 2 }],
                    Status = MerchItemStatus.Available
                }
            }.BuildMockDbSet().Object);

        var expectedCompounds = new List<KitCompoundDto> { new(1, false, "Cup", 2) };
        var expectedMinAmounts = new List<KitMinAmountDto> { new(1, 2)};

        var request = new GetKitByIdRequest.Query(1);

        //Act
        var kitDto = await _handler.Handle(request, _token);

        //Assert
        kitDto.Id.Should().Be(1);
        kitDto.Name.Should().Be("Kit 1");
        kitDto.Description.Should().Be("Kit 1 description");
        kitDto.Compounds.Should().BeEquivalentTo(expectedCompounds);
        kitDto.MinAmounts.Should().BeEquivalentTo(expectedMinAmounts);
        kitDto.Status.Should().Be(MerchItemStatus.Available);
    }
}
