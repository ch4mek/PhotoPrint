using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class UpdateUserLocationRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _dbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBus = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdateUserLocationRequest _handler;

    public UpdateUserLocationRequestTests()
    {
        _handler = new UpdateUserLocationRequest(
            _dbContextMock.Object,
            _communicationBus.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeIsTryingToUpdateAnotherUserLocationThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1111;

        var query = new UpdateUserLocationRequest.Command(userId, LocationType.Minsk);

        var user = new User
        {
            Id = userId,
            Location = LocationType.Tbilisi,
            FirstName = "test",
            LastName = "test",
        };
        var users = new List<User> { user };

        _dbContextMock
            .SetupGet(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["Employee"]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(234);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to update another user location");
    }

    [Fact]
    public async Task HandleWhenUserIsSysAdminIsTryingToUpdateAnotherUserLocationThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1111;

        var query = new UpdateUserLocationRequest.Command(userId, LocationType.Minsk);

        var user = new User
        {
            Id = userId,
            Location = LocationType.Tbilisi,
            FirstName = "test",
            LastName = "test",
        };
        var users = new List<User> { user };

        _dbContextMock
            .SetupGet(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["SystemAdministrator"]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(234);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to update another user location");
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserDoesNotExistThrowEntityNotFoundException()
    {
        //Arrange
        var userId = 1111;
        var users = new List<User>();

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["CEO"]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(234);

        var query = new UpdateUserLocationRequest.Command(userId, LocationType.Minsk);

        _dbContextMock.Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"User with id = {userId} does not exist");
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistReturnNewLocationAndUpdateLocation()
    {
        //Arrange
        var userId = 1111;

        var user = new User { Id = userId, Location = LocationType.Tbilisi,
            FirstName = "test",
            LastName = "test",
        };
        var users = new List<User> { user };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["CEO"]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        var query = new UpdateUserLocationRequest.Command(userId, LocationType.Minsk);

        _dbContextMock.Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var location = await _handler.Handle(query, _token);

        //Assert
        location.Should().Be(LocationType.Minsk.GetDescription());
        user.Location.Should().Be(LocationType.Minsk);
    }

    [Fact]
    public async Task HandleWhenTeamBoardUserExistPublishNotification()
    {
        //Arrange
        var userId = 1111;

        var user = new User { Id = userId, Location = LocationType.Tbilisi,
            FirstName = "test",
            LastName = "test",
        };
        var users = new List<User> { user };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(["CEO"]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        var query = new UpdateUserLocationRequest.Command(userId, LocationType.Minsk);

        _dbContextMock.Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        await _handler.Handle(query, _token);

        //Assert
        _communicationBus.Verify(
            outbox => outbox.PublishAsync(It.IsAny<UserLocationUpdatedEvent>(), _token),
            Times.Once);
    }
}
