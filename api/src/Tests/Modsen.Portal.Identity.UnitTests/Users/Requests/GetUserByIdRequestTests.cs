using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetUserByIdRequestTests
{
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetUserByIdRequest _handler;

    public GetUserByIdRequestTests()
    {
        _handler = new GetUserByIdRequest(_userLoaderMock.Object, _timeTrackContractMock.Object, _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task GetUserWhenUserExistsReturnSuccessResult()
    {
        //Arrange
        var userId = 1;

        var user = new UserDto()
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        var userDto = new UserDto
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        //Act
        var result = await _handler.Handle(
            new GetUserByIdRequest.Query(userId, DepartmentId: null, Roles: [], ResourceGroupId: null, Claims: []),
            _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(userDto);
    }

    [Fact]
    public async Task GetUserWhenUserFromAnotherDepartmentRequestsShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var user = new UserDto()
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        string[] roles = [];

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new GetUserByIdRequest.Query(userId, DepartmentId: "Net", ResourceGroupId: null, roles, []),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task GetUserWhenHeadCoordinatorRequestsUserFromSameDepartmentNotInProjectShouldReturnUser()
    {
        //Arrange
        var userId = 1;
        var departmentId = "Js";

        var user = new UserDto()
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = departmentId,
            ResourceGroupId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        string[] roles = [PortalRoleNames.HeadOfDepartment];
        string[] claims = [CoordinatorClaimTypeNames.ManageProjectCoordinator.Name];

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        //Act
        var result = await _handler.Handle(
            new GetUserByIdRequest.Query(userId, DepartmentId: departmentId, ResourceGroupId: null, roles, Claims: claims),
            _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetUserWhenHeadCoordinatorRequestsUserFromDifferentDepartmentNotInProjectShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var user = new UserDto()
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = null,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        string[] roles = [PortalRoleNames.HeadOfDepartment];
        string[] claims = [CoordinatorClaimTypeNames.ManageProjectCoordinator.Name];
        var currentUserId = 99;

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(currentUserId);

        _timeTrackContractMock.Setup(x => x.GetUserIdsByProjectManagerAsync(currentUserId, _cancellationToken))
            .ReturnsAsync([]);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new GetUserByIdRequest.Query(userId, DepartmentId: "Net", ResourceGroupId: null, roles, Claims: claims),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task GetUserWhenGroupManagetFromAnotherGroupRequestsShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var user = new UserDto()
        {
            Id = userId,
            FirstName = "FirstName",
            LastName = "LastName",
            FullName = "LastName FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        };

        string[] roles = [PortalRoleNames.GroupManager];

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new GetUserByIdRequest.Query(userId, DepartmentId: "Net", ResourceGroupId: 2, roles, []),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
