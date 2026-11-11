using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class UpdateUserAvatarColorRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBus = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdateUserAvatarColorRequest _handler;

    public UpdateUserAvatarColorRequestTests()
    {
        _handler = new UpdateUserAvatarColorRequest(
            _communicationBus.Object,
            _authenticatedUserContextMock.Object,
            _userLoaderMock.Object,
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsNotFoundThrowEntityNotFoundException()
    {
        //Arrange
        var query = new UpdateUserAvatarColorRequest.Command(1, "#ffffff");

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("User with id 1 does not exist");
    }

    [Fact]
    public async Task HandleWhenUserIsHeadAndUpdatedUserFromAnotherDepartmentThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1111;

        var user = new User
        {
            Id = userId,
            DepartmentId = "Net",
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Js");

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>{ user }.BuildMockDbSet().Object);

        var query = new UpdateUserAvatarColorRequest.Command(userId, "#ffffff");

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>(PortalMessages.YouCannotChangeUserWithDifferentDepartment);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerAndUpdatedUserFromAnotherGroupThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1111;

        var user = new User
        {
            Id = userId,
            ResourceGroupId = 1,
            FirstName = "test",
            LastName = "test",
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(2);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        var query = new UpdateUserAvatarColorRequest.Command(userId, "#ffffff");

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>(PortalMessages.YouCannotChangeUserWithDifferentGroup);
    }

    [Fact]
    public async Task HandleWhenUserExistsPublishNotificationAndReturnDto()
    {
        //Arrange
        var userId = 1111;
        var newColor = "#ffffff";

        var roles = new List<UserRole>
        {
            new()
            {
                Role = new Role
                {
                    Name = PortalRoleNames.Employee,
                    OrderWeight = 100,
                    PublicName = "Test"
                }
            }
        };

        var user = new User
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@email.com",
            DepartmentId = "Net",
            AvatarColor = "#000000",
            Color = "#000000",
            UserRoles = roles,
            ResourceGroupId = 1,
        };

        var users = new List<User> { user };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        var userDto = new UserDto
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@email.com",
            AvatarColor = newColor
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userId, _token))
            .ReturnsAsync(userDto);

        var query = new UpdateUserAvatarColorRequest.Command(userId, newColor);

        _identityDbContextMock.Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var location = await _handler.Handle(query, _token);

        //Assert
        _communicationBus.Verify(
            outbox => outbox.PublishAsync(It.Is<UserUpdatedEvent>(x => x.AvatarColor == newColor), _token),
            Times.Once);

        location.Should().BeEquivalentTo(userDto);
    }
}
