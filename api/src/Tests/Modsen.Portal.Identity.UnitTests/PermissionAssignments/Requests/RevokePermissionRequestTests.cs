using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.PermissionAssignments.Requests;

public class RevokePermissionRequestTests
{
    private readonly Mock<IDeletePermissionAssignmentsDbQuery> _deletePermissionAssignmentsDbQueryMock = new();
    private readonly Mock<IPermissionAssignmentsLoader> _permissionAssignmentsLoaderMock = new();
    private readonly Mock<IAuthenticationTokenContext> _authenticationTokenContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly RevokePermissionRequest _handler;

    public RevokePermissionRequestTests()
    {
        _handler = new RevokePermissionRequest(
            _deletePermissionAssignmentsDbQueryMock.Object,
            _permissionAssignmentsLoaderMock.Object,
            _authenticationTokenContextMock.Object,
            _authenticatedUserContext.Object,
            _userLoaderMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Net")]
    public async Task HandeWhenUserExistsShouldCallRemovePermissionAssignmentDbQueryOnce(
        string currentUserRole,
        string? currentUserDepartmentId)
    {
        // Arrange
        const int assignmentId = 1;
        const int userId = 1;
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        const PermissionType permissionType = PermissionType.TrackBillableWorkingHours;

        var assignment = new PermissionAssignmentCoreDto
        {
            Id = 2,
            PermissionType = permissionType,
            ResourceId = "3",
            ResourceName = "Test3",
            ResourceType = resourceType,
            UserId = userId
        };

        var command = new RevokePermissionRequest.Command(assignmentId);

        var user = new UserDto
        {
            FirstName = "Name",
            Email = "name@mail.com",
            DepartmentId = "Net"
        };

        _permissionAssignmentsLoaderMock.Setup(x => x.LoadAsync(assignmentId, _cancellationToken))
            .ReturnsAsync(assignment);

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContext.Setup(x => x.GetRoles())
            .Returns([currentUserRole]);

        _authenticatedUserContext.Setup(x => x.DepartmentId)
            .Returns(currentUserDepartmentId);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        _deletePermissionAssignmentsDbQueryMock
            .Verify(query => query.ExecuteAsync(It.IsAny<int[]>(), _cancellationToken),
            Times.Once);

        _authenticationTokenContextMock.Verify(context => context.RevokeAllUserTokens(userId, "project permission assignment has been changed", _cancellationToken), Times.Once);
    }

    [Fact]
    public void HandleWhenPermissionAssignmentDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int assignmentId = 1;

        var command = new RevokePermissionRequest.Command(assignmentId);

        _permissionAssignmentsLoaderMock.Setup(x => x.LoadAsync(assignmentId, _cancellationToken))
            .ReturnsAsync((PermissionAssignmentCoreDto?)null);

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public void HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const int assignmentId = 1;
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        const PermissionType permissionType = PermissionType.TrackBillableWorkingHours;

        var command = new RevokePermissionRequest.Command(assignmentId);

        var assignment = new PermissionAssignmentCoreDto
        {
            Id = 2,
            PermissionType = permissionType,
            ResourceId = "3",
            ResourceName = "Test3",
            ResourceType = resourceType,
            UserId = userId
        };

        _permissionAssignmentsLoaderMock.Setup(x => x.LoadAsync(assignmentId, _cancellationToken))
            .ReturnsAsync(assignment);

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync((UserDto?)null);

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public void HandleWhenHeadOfDepartmentUpdatesPermissionsForUserFromAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int userId = 1;
        const int assignmentId = 1;
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        const PermissionType permissionType = PermissionType.TrackBillableWorkingHours;

        var command = new RevokePermissionRequest.Command(assignmentId);

        var assignment = new PermissionAssignmentCoreDto
        {
            Id = 2,
            PermissionType = permissionType,
            ResourceId = "3",
            ResourceName = "Test3",
            ResourceType = resourceType,
            UserId = userId
        };

        _permissionAssignmentsLoaderMock.Setup(x => x.LoadAsync(assignmentId, _cancellationToken))
            .ReturnsAsync(assignment);

        var user = new UserDto
        {
            Id = userId,
            DepartmentId = "Net",
            FirstName = "Name",
            Email = "name@mail.com",
            RoleNames = [PortalRoleNames.Employee]
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContext.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContext.Setup(x => x.DepartmentId)
            .Returns("Ruby");

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
