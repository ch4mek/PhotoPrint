using MockQueryable.Moq;
using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.Contract;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class DeactivateUserRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly Mock<IEquipmentContract> _equipmentContractMock = new();
    private readonly Mock<IBookingContract> _bookingContractMock = new();
    private readonly Mock<ICommunicationBus> _outboxContract = new();
    private readonly DeactivateUserRequest _handler;
    private readonly Mock<IAuthenticationTokenContext> _authenticationTokenContext = new();
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<IInquiriesContract> _inquiriesContractMock = new();

    public DeactivateUserRequestTests()
    {
        _handler = new DeactivateUserRequest(
            _authenticatedUserContextMock.Object,
            _timeTrackContractMock.Object,
            _equipmentContractMock.Object,
            _bookingContractMock.Object,
            _outboxContract.Object,
            _identityDbContextMock.Object,
            _userLoaderMock.Object,
            _authenticationTokenContext.Object,
            _inquiriesContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;

        _userLoaderMock
            .Setup(x => x.LoadAsync(1, _cancellationToken))
            .ThrowsAsync( new EntityNotFoundException($"User with id {userId} does not exist"));

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userId),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task HandleWhenUserIsTryingToDeactivateHimselfShouldThrowActionNotAllowedException(string requestUserRole)
    {
        // Arrange
        var user = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example1@example.com",
            RoleNames = [requestUserRole]
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(user.Id, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([requestUserRole]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(1);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(user.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(user.Id, _cancellationToken),
            Times.Once);
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task HandleWhenUserIsTryingToDeactivateCeoShouldThrowActionNotAllowedException(string requestUserRole)
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 1,
            FirstName = "CeoFirstName",
            Email = "ceo@example.com",
            RoleNames = new List<string> { PortalRoleNames.Ceo }
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([requestUserRole]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(1);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentIsTryingToDeactivateCtoShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 1,
            FirstName = "CtoFirstName",
            Email = "cto@example.com",
            RoleNames = new List<string> { PortalRoleNames.Cto }
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns("Net");

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentIsTryingToDeactivateUserFromAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js"
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns("Net");

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenSysAdminIsTryingToDeactivateNotAnEmployeeShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            RoleNames = [PortalRoleNames.HeadOfDepartment]
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.SystemAdministrator]);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateUserWithAssignedProjectsShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js"
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([It.IsAny<ProjectOutput>()]);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateUserWithEquipmentDebtsShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js"
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([]);

        _equipmentContractMock.Setup(x => x.CanEquipmentUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateUserWithActiveWorkplaceBookingsShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js"
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([]);

        _equipmentContractMock.Setup(x => x.CanEquipmentUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _bookingContractMock.Setup(x => x.CanBookingUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateUserWhichIsApproverOrSubstituteOfRequestsShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js"
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([]);

        _equipmentContractMock.Setup(x => x.CanEquipmentUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _bookingContractMock.Setup(x => x.CanBookingUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _inquiriesContractMock.Setup(x => x.IsUserApproverOrSubstitute(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.UserIsApproverOrSubstitute);

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateProjectManagerShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js"
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _equipmentContractMock.Setup(x => x.CanEquipmentUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _bookingContractMock.Setup(x => x.CanBookingUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([]);
        _timeTrackContractMock.Setup(x => x.IsUserAssignedToProjectAsPmAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var actionUnderTest = () =>
            _handler.Handle(new DeactivateUserRequest.Command(userToDeactivate.Id), _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateIsUserInResourceGroupShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = 1,
            RoleNames = [PortalRoleNames.Employee]
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([]);

        _equipmentContractMock.Setup(x => x.CanEquipmentUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _bookingContractMock.Setup(x => x.CanBookingUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsTryingToDeactivateIsGroupManagerWithActiveGroupShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userToDeactivate = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "example1@example.com",
            DepartmentId = "Js",
            ResourceGroupId = 1,
            RoleNames = [PortalRoleNames.GroupManager]
        };

        var groups = new List<ResourceGroup>
        {
            new() {
                Id = (int)userToDeactivate.ResourceGroupId,
                IsActive = true,
                Name = "RG_TEST",
                DepartmentId = "Net"
            }
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(userToDeactivate);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(2);

        _timeTrackContractMock.Setup(x => x.GetProjectsByUserId(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync([]);

        _equipmentContractMock.Setup(x => x.CanEquipmentUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _bookingContractMock.Setup(x => x.CanBookingUserBeDeactivated(userToDeactivate.Id, _cancellationToken))
            .ReturnsAsync(true);

        _identityDbContextMock.Setup(x => x.ResourceGroups)
            .Returns(groups.BuildMockDbSet().Object);

        // Act
        var actionUnderTest = () => _handler.Handle(
            new DeactivateUserRequest.Command(userToDeactivate.Id),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();

        _userLoaderMock.Verify(x =>
                x.LoadAsync(userToDeactivate.Id, _cancellationToken),
            Times.Once);
    }
}
