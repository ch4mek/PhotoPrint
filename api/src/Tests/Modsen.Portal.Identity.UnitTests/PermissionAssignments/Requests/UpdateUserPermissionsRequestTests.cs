using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Composers;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.DbQueries;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.PermissionAssignments.Requests;

public class UpdateUserPermissionsRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ICanUserAccessResourcesBusinessRule> _doPermissionResourceExistsBusinessRule = new();
    private readonly Mock<IDeletePermissionAssignmentsDbQuery> _deletePermissionAssignmentsDbQuery = new();
    private readonly Mock<IPermissionAssignmentsComposer> _permissionAssignmentsComposerMock = new();
    private readonly Mock<IPermissionAssignmentsLoader> _permissionAssignmentsLoaderMock = new();
    private readonly Mock<IAuthenticationTokenContext> _authenticationTokenContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly Mock<IIdentityDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateUserPermissionsRequest _handler;

    public UpdateUserPermissionsRequestTests()
    {
        _handler = new UpdateUserPermissionsRequest(
            _authenticatedUserContext.Object,
            _deletePermissionAssignmentsDbQuery.Object,
            _doPermissionResourceExistsBusinessRule.Object,
            _permissionAssignmentsComposerMock.Object,
            _permissionAssignmentsLoaderMock.Object,
            _authenticationTokenContextMock.Object,
            _userLoaderMock.Object,
            _dbContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Net")]
    public async Task HandleWhenOnlyAddingNewAssignmentsForResourcesShouldCallCreatePermissionAssignmentsDbQueryOnceAndRemoveNever(
        string currentUserRole,
        string? currentUserDepartmentId)
    {
        // Arrange
        const int userId = 1;
        var resourcesToUpdate = new Dictionary<string, string>
        {
            { "1", "Test1" },
            { "2", "Test2" },
            { "3", "Test3" },
            { "4", "Test4" }
        };
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        const PermissionType permissionType = PermissionType.TrackBillableWorkingHours;

        var existingAssignments = new List<PermissionAssignmentCoreDto>
        {
            new()
            {
                Id = 1,
                PermissionType = permissionType,
                ResourceId = "1",
                ResourceName = "Test1",
                ResourceType = resourceType,
                UserId = userId
            },
            new()
            {
                Id = 2,
                PermissionType = permissionType,
                ResourceId = "2",
                ResourceName = "Test2",
                ResourceType = resourceType,
                UserId = userId
            }
        };

        var newAssignments = new List<PermissionAssignmentCoreDto>
        {
            new()
            {
                Id = 3,
                PermissionType = permissionType,
                ResourceId = "3",
                ResourceName = "Test3",
                ResourceType = resourceType,
                UserId = userId
            },
            new()
            {
                Id = 4,
                PermissionType = permissionType,
                ResourceId = "4",
                ResourceName = "Test4",
                ResourceType = resourceType,
                UserId = userId
            }
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            PermissionType.TrackBillableWorkingHours,
            Resources: resourcesToUpdate,
            ResourceType: PermissionResourceType.Project);


        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net"
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContext.Setup(x => x.GetRoles())
            .Returns([currentUserRole]);

        _authenticatedUserContext.Setup(x => x.DepartmentId)
            .Returns(currentUserDepartmentId);

        var doPermissionExistsModel = new CanUserAccessResourcesBusinessRule.Request(userId, resourceType, resourcesToUpdate);

        _doPermissionResourceExistsBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(doPermissionExistsModel, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.PermissionAssignments)
            .Returns(
                new List<PermissionAssignment>
                {
                    new()
                    {
                        Id = 1,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "1",
                        ResourceName = "Test1",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    },
                    new()
                    {
                        Id = 2,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "2",
                        ResourceName = "Test2",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    }
                }.BuildMockDbSet().Object);

        _permissionAssignmentsLoaderMock
            .Setup(x => x.LoadAsync(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync(existingAssignments);

        _permissionAssignmentsComposerMock
            .Setup(x => x.ComposeAsync(
                It.IsAny<PermissionAssignmentsComposerRequest>(), _cancellationToken))
            .ReturnsAsync(newAssignments);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        _permissionAssignmentsComposerMock.Verify(x =>
                x.ComposeAsync(It.IsAny<PermissionAssignmentsComposerRequest>(), _cancellationToken),
            Times.Once);

        _deletePermissionAssignmentsDbQuery.Verify(x =>
            x.ExecuteAsync(It.IsAny<int[]>(),It.IsAny<CancellationToken>()),
            Times.Never);

        _authenticationTokenContextMock.Verify(x => x.RevokeAllUserTokens(userId, "test", _cancellationToken), Times.Never);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Net")]
    public async Task HandleWhenOnlyRemovingOldAssignmentsForResourcesShouldCallRemovePermissionAssignmentsDbQueryOnceAndCreateNever(
        string currentUserRole,
        string? departmentId)
    {
        // Arrange
        const int userId = 1;
        var resourcesToRemain = new Dictionary<string, string>
        {
            { "1", "Test1" },
            { "2", "Test2" }
        };
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        const PermissionType permissionType = PermissionType.TrackBillableWorkingHours;

        var existingAssignments = new List<PermissionAssignmentCoreDto>
        {
            new()
            {
                Id = 1,
                PermissionType = permissionType,
                ResourceId = "1",
                ResourceName = "Test1",
                ResourceType = resourceType,
                UserId = userId
            },
            new()
            {
                Id = 2,
                PermissionType = permissionType,
                ResourceId = "2",
                ResourceName = "Test2",
                ResourceType = resourceType,
                UserId = userId
            },
            new()
            {
                Id = 3,
                PermissionType = permissionType,
                ResourceId = "3",
                ResourceName = "Test3",
                ResourceType = resourceType,
                UserId = userId
            }
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            permissionType,
            resourcesToRemain,
            resourceType);

        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net"
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContext.Setup(x => x.GetRoles())
            .Returns([currentUserRole]);

        _authenticatedUserContext.Setup(x => x.DepartmentId)
            .Returns(departmentId);

        var doPermissionExistsModel = new CanUserAccessResourcesBusinessRule.Request(userId, resourceType, resourcesToRemain);

        _doPermissionResourceExistsBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(doPermissionExistsModel, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.PermissionAssignments)
            .Returns(
                new List<PermissionAssignment>
                {
                    new()
                    {
                        Id = 1,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "1",
                        ResourceName = "Test1",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    },
                    new()
                    {
                        Id = 2,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "2",
                        ResourceName = "Test2",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    },
                    new()
                    {
                        Id = 3,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "3",
                        ResourceName = "Test3",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    }
                }.BuildMockDbSet().Object);

        _permissionAssignmentsLoaderMock
            .Setup(x => x.LoadAsync(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync(existingAssignments);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        _deletePermissionAssignmentsDbQuery.Verify(x =>
                x.ExecuteAsync(It.IsAny<int[]>(),It.IsAny<CancellationToken>()),
            Times.Once);

        _permissionAssignmentsComposerMock.Verify(x =>
                x.ComposeAsync(It.IsAny<PermissionAssignmentsComposerRequest>(), _cancellationToken),
            Times.Never);

        _authenticationTokenContextMock.Verify(x => x.RevokeAllUserTokens(userId, "permission assignment has been changed", _cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Net")]
    public async Task HandleWhenAddingNewAssignmentsAndRemovingOldForResourcesShouldCallCreatePermissionAssignmentsDbQueryOnceAndRemoveOnce(
        string currentUserRole,
        string? currentUserDepartmentId)
    {
        // Arrange
        const int userId = 1;
        var resourcesToUpdate = new Dictionary<string, string>
        {
            { "2", "Test2" },
            { "3", "Test3" },
            { "4", "Test4" }
        };
        const PermissionResourceType resourceType = PermissionResourceType.Project;
        const PermissionType permissionType = PermissionType.TrackBillableWorkingHours;

        var existingAssignments = new List<PermissionAssignmentCoreDto>
        {
            new()
            {
                Id = 1,
                PermissionType = permissionType,
                ResourceId = "1",
                ResourceName = "Test1",
                ResourceType = resourceType,
                UserId = userId
            },
            new()
            {
                Id = 2,
                PermissionType = permissionType,
                ResourceId = "2",
                ResourceName = "Test2",
                ResourceType = resourceType,
                UserId = userId
            }
        };

        var newAssignments = new List<PermissionAssignmentCoreDto>
        {
            new()
            {
                Id = 3,
                PermissionType = permissionType,
                ResourceId = "3",
                ResourceName = "Test3",
                ResourceType = resourceType,
                UserId = userId
            },
            new()
            {
                Id = 4,
                PermissionType = permissionType,
                ResourceId = "4",
                ResourceName = "Test4",
                ResourceType = resourceType,
                UserId = userId
            }
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            permissionType,
            resourcesToUpdate,
            resourceType);

        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            RoleNames = [PortalRoleNames.Employee],
            DepartmentId = "Net"
        };

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        _authenticatedUserContext.Setup(x => x.GetRoles())
            .Returns([currentUserRole]);

        _authenticatedUserContext.Setup(x => x.DepartmentId)
            .Returns(currentUserDepartmentId);

        var isResourcesExistModel = new CanUserAccessResourcesBusinessRule.Request(userId, resourceType, resourcesToUpdate);

        _doPermissionResourceExistsBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(isResourcesExistModel, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.PermissionAssignments)
            .Returns(
                new List<PermissionAssignment>
                {
                    new()
                    {
                        Id = 1,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "1",
                        ResourceName = "Test1",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    },
                    new()
                    {
                        Id = 2,
                        Permission = new PermissionDefinition { Name = permissionType.ToString() },
                        ResourceId = "2",
                        ResourceName = "Test2",
                        ResourceType = resourceType.ToString(),
                        UserId = userId
                    }
                }.BuildMockDbSet().Object);

        _permissionAssignmentsLoaderMock
            .Setup(x => x.LoadAsync(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync(existingAssignments);

        _permissionAssignmentsComposerMock
            .Setup(x => x.ComposeAsync(It.IsAny<PermissionAssignmentsComposerRequest>(), _cancellationToken))
            .ReturnsAsync(newAssignments);

        // Act
        await _handler.Handle(command, _cancellationToken);

        // Assert
        _deletePermissionAssignmentsDbQuery.Verify(x =>
                x.ExecuteAsync(It.IsAny<int[]>(),It.IsAny<CancellationToken>()),
            Times.Once);

        _permissionAssignmentsComposerMock.Verify(x =>
                x.ComposeAsync(It.IsAny<PermissionAssignmentsComposerRequest>(), _cancellationToken),
            Times.Once);

        _authenticationTokenContextMock.Verify(x => x.RevokeAllUserTokens(userId, "permission assignment has been changed", _cancellationToken), Times.Once);
    }

    [Fact]
    public void HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            PermissionType.TrackBillableWorkingHours,
            Resources: new Dictionary<string, string> { { "2", "Test2" } },
            ResourceType: PermissionResourceType.Project);

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync((UserDto?)null);

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public void HandleWhenUserIsNotEmployeeShouldThrowActionNotAllowedException(string userRole)
    {
        // Arrange
        const int userId = 1;

        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            DepartmentId = "Net",
            RoleNames = [userRole]
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            PermissionType.TrackBillableWorkingHours,
            Resources: new Dictionary<string, string> { { "2", "Test2" } },
            ResourceType: PermissionResourceType.Project);

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public void HandleWhenHeadOfDepartmentUpdatesPermissionsForUserFromAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int userId = 1;

        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            DepartmentId = "Net",
            RoleNames = [PortalRoleNames.Employee]
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            PermissionType.TrackBillableWorkingHours,
            Resources: new Dictionary<string, string> { { "2", "Test2" } },
            ResourceType: PermissionResourceType.Project);

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

    [Fact]
    public void HandleWhenUserIsNotInDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int userId = 1;

        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            DepartmentId = null,
            RoleNames = [PortalRoleNames.Employee]
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            PermissionType.TrackBillableWorkingHours,
            Resources: new Dictionary<string, string> { { "2", "Test2" } },
            ResourceType: PermissionResourceType.Project);

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public void HandleWhenUserCanNotAccessResourcesShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int userId = 1;

        var user = new UserDto
        {
            Id = userId,
            FirstName = "Name",
            Email = "name@mail.com",
            DepartmentId = "Net",
            RoleNames = [PortalRoleNames.Employee]
        };

        var command = new UpdateUserPermissionsRequest.Command(
            userId,
            PermissionType.TrackBillableWorkingHours,
            Resources: new Dictionary<string, string> { { "2", "Test2" } },
            ResourceType: PermissionResourceType.Project);

        _userLoaderMock.Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(user);

        var resourcesToUpdate = new Dictionary<string, string>();

        var isResourcesExistModel = new CanUserAccessResourcesBusinessRule.Request(userId, PermissionResourceType.Project, resourcesToUpdate);

        _doPermissionResourceExistsBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(isResourcesExistModel, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var actionUnderTest = () => _handler.Handle(command, _cancellationToken);

        // Assert
        actionUnderTest.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
