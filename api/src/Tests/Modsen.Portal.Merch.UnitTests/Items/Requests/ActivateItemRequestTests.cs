using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class ActivateItemRequestTests
{
    private readonly Mock<IGetMerchItemByIdDbQuery> _getItemDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _userContextMock = new();
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly ActivateItemRequest _handler;

    public ActivateItemRequestTests()
    {
        _handler = new ActivateItemRequest(
            _getItemDbQueryMock.Object,
            _merchDbContextMock.Object,
            _userContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenItemNotFoundThrowsInvalidDataAppException()
    {
        //Arrange
        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(It.IsAny<int>(), _token))
            .ReturnsAsync((ItemEntity?)null);

        var command = new ActivateItemRequest.Command(1);

        //Act
        var action = () => _handler.Handle(command, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.MerchItemNotFound);

    }

    [Fact]
    public async Task HandleWhenItemIsAlreadyActiveReturnsStatusDtoWithoutDbChanges()
    {
        //Arrange
        var item = new ItemEntity { Id = 1, Name = "t-shirt", Status = MerchItemStatus.Available };

        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(item.Id, _token))
            .ReturnsAsync(item);

        var command = new ActivateItemRequest.Command(item.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Available);
        result.Name.Should().Be(item.Name);
        result.Id.Should().Be(item.Id);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Never);
    }

    [Fact]
    public async Task HandleWhenItemWasArchivedReturnsAvailableStatusDto()
    {
        //Arrange
        var item = new ItemEntity { Id = 1, Name = "t-shirt", Status = MerchItemStatus.Archived };

        _getItemDbQueryMock
            .Setup(query => query.ExecuteAsync(item.Id, _token))
            .ReturnsAsync(item);

        _userContextMock
            .Setup(context => context.UserId)
            .Returns(1);

        var command = new ActivateItemRequest.Command(item.Id);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Status.Should().Be(MerchItemStatus.Available);
        result.Name.Should().Be(item.Name);
        result.Id.Should().Be(item.Id);
        item.UpdatedById.Should().Be(1);

        _merchDbContextMock.Verify(db => db.SaveChangesAsync(_token), Times.Once);
    }
}
