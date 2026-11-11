using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests;

public class ActivateKitRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _userContextMock = new();
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly ActivateKitRequest _handler;

    public ActivateKitRequestTests()
    {
        _handler = new ActivateKitRequest(
            _userContextMock.Object,
            _merchDbContextMock.Object,
            _clockServiceMock.Object);

        _clockServiceMock
            .Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task HandleWhenKitNotFoundThrowsInvalidDataAppException()
    {
        //Arrange
        _merchDbContextMock
            .Setup(x => x.Kits)
            .Returns(new List<KitEntity>().BuildMockDbSet().Object);

        var command = new ActivateKitRequest.Command(1);

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.MerchItemNotFound);

    }

    [Fact]
    public async Task HandleWhenKitIsAlreadyActiveReturnsStatusDtoWithoutDbChanges()
    {
        //Arrange
        var kit = new KitEntity
        {
            Id = 1,
            Name = "New Year kit",
            Status = MerchItemStatus.Available
        };

        var mockDbSet = new List<KitEntity> { kit }.BuildMockDbSet();
        _merchDbContextMock
            .Setup(x => x.Kits)
            .Returns(mockDbSet.Object);

        var command = new ActivateKitRequest.Command(kit.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Available);
        result.Name.Should().Be(kit.Name);
        result.Id.Should().Be(kit.Id);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Never);
    }

    [Fact]
    public async Task HandleWhenKitWasArchivedReturnsAvailableStatusDto()
    {
        //Arrange
        var kit = new KitEntity
        {
            Id = 1,
            Name = "New Year kit",
            Status = MerchItemStatus.Archived,
        };

        var mockDbSet = new List<KitEntity> { kit }.BuildMockDbSet();
        _merchDbContextMock
            .Setup(x => x.Kits)
            .Returns(mockDbSet.Object);

        _merchDbContextMock
            .Setup(x => x.KitItems)
            .Returns(new List<KitItemEntity>().BuildMockDbSet().Object);

        _userContextMock
            .Setup(context => context.UserId)
            .Returns(1);

        var command = new ActivateKitRequest.Command(kit.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Available);
        result.Name.Should().Be(kit.Name);
        result.Id.Should().Be(kit.Id);
        kit.UpdatedById.Should().Be(1);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Once);
    }

    [Fact]
    public async Task HandleWhenKitItemWasArchivedThrowsInvalidDataAppException()
    {
        //Arrange
        var kit = new KitEntity
        {
            Id = 1,
            Name = "New Year kit",
            Status = MerchItemStatus.Archived,
        };

        var kitItem = new KitItemEntity
        {
            KitId = kit.Id,
            ItemId = 1,
            Item = new ItemEntity
            {
                Id = 1,
                Name = "test",
                Status = MerchItemStatus.Archived,
            }
        };

        var mockDbSet = new List<KitEntity> { kit }.BuildMockDbSet();
        _merchDbContextMock
            .Setup(x => x.Kits)
            .Returns(mockDbSet.Object);

        _merchDbContextMock
            .Setup(x => x.KitItems)
            .Returns(new List<KitItemEntity>{ kitItem }.BuildMockDbSet().Object);

        _userContextMock
            .Setup(context => context.UserId)
            .Returns(1);

        var command = new ActivateKitRequest.Command(kit.Id);

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.MerchItemNotFound);
    }
}
