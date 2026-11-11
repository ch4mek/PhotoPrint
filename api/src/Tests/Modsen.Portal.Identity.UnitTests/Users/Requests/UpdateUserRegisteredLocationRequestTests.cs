using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class UpdateUserRegisteredLocationRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBus = new();
    private readonly Mock<ITeamBoardContract> _teamBordContractMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UpdateUserRegisteredLocationRequest _handler;

    public UpdateUserRegisteredLocationRequestTests()
    {
        _handler = new UpdateUserRegisteredLocationRequest(
            _identityDbContextMock.Object,
            _communicationBus.Object,
            _authenticatedUserContextMock.Object,
            _teamBordContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsNotFoundThrowEntityNotFoundException()
    {
        //Arrange
        var initiatotId = 2;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var query = new UpdateUserRegisteredLocationRequest.Command(1, 1, null, LocationType.Unknown);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>("User with id = 1 does not exist");
    }

    [Fact]
    public async Task HandleWhenUserIsHeadAndUpdatedUserFromAnotherDepartmentThrowActionNotAllowedException()
    {
        //Arrange
        var initiatotId = 1;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

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

        var query = new UpdateUserRegisteredLocationRequest.Command(userId, 1, null, LocationType.Unknown);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to update user from another department");
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerAndUpdatedUserFromAnotherGroupThrowActionNotAllowedException()
    {
        //Arrange
        var initiatotId = 1;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

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

        var query = new UpdateUserRegisteredLocationRequest.Command(userId, 1, null, LocationType.Unknown);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to update user from another resource group");
    }

    [Fact]
    public async Task HandleWhenCountryIsNotFoundThrowInvalidDataAppException()
    {
        //Arrange
        var initiatotId = 2;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var userId = 1111;

        var user = new User
        {
            Id = userId,
            ResourceGroupId = 1,
            FirstName = "test",
            LastName = "test",
        };

        var query = new UpdateUserRegisteredLocationRequest.Command(userId, 2, null, LocationType.Unknown);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(234);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        var countries = new List<CountryOutput> { new() { Id = 1, Name = "Belarus" } };

        _teamBordContractMock
            .Setup(x => x.GetCountries(_token))
            .ReturnsAsync(countries);

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>("Country with id: 2 is not found");
    }

    [Fact]
    public async Task HandleWhenUserAndCountryExistPublishNotificationAndReturnDto()
    {
        //Arrange
        var initiatotId = 2;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var userId = 1111;
        var newRegisteredLocationId = 1;
        var newCurrentLocation = "Minsk";

        var user = new User
        {
            Id = userId,
            RegisteredLocationId = 2,
            FirstName = "test",
            LastName = "test",
        };

        var users = new List<User> { user };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        var countries = new List<CountryOutput> { new() { Id = newRegisteredLocationId, Name = "Belarus" } };
        var newLocation = new RegisteredLocationDto
        {
            RegisteredLocationId = newRegisteredLocationId,
            RegisteredLocationName = countries.First().Name,
            CurrentLocation = newCurrentLocation,
            Location = LocationType.Minsk
        };

        _teamBordContractMock
            .Setup(x => x.GetCountries(_token))
            .ReturnsAsync(countries);

        var query = new UpdateUserRegisteredLocationRequest
            .Command(userId, newRegisteredLocationId, newCurrentLocation, LocationType.Minsk);

        _identityDbContextMock.Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var location = await _handler.Handle(query, _token);

        //Assert
        _communicationBus.Verify(
            outbox => outbox.PublishAsync(It.Is<UserRegisteredLocationUpdatedEvent>(x =>
                    x.RegisteredLocationId == newRegisteredLocationId &&
                    x.CurrentLocation == newCurrentLocation),
                _token),
            Times.Once);

        location.Should().BeEquivalentTo(newLocation);
    }

    [Fact]
    public async Task HandleWhenUserUpdatesThemselfThrowActionNotAllowedException()
    {
        //Arrange
        var initiatotId = 1;
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);
       
        var query = new UpdateUserRegisteredLocationRequest.Command(initiatotId, 2, null, LocationType.Unknown);           

        //Act
        var action = () => _handler.Handle(query, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to edit your own registered location");
    }
}
