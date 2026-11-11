using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Core.BusinessRules;

public class CanGroupMemberBeRemovedBusinessRuleTests
{
    private readonly Mock<ICanUserUpdateGroupBusinessRule> _canUserUpdateGroupBusinessRuleMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CanGroupMemberBeRemovedBusinessRule _canGroupMemberBeRemovedBusinessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanGroupMemberBeRemovedBusinessRuleTests()
    {
        _canGroupMemberBeRemovedBusinessRule = new(
            _canUserUpdateGroupBusinessRuleMock.Object,
            _identityDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsNotExistShouldReturnFail()
    {
        // Arrange
        var userId = 1;
        var groupId = 1;
        var request = new CanGroupMemberBeRemovedBusinessRule.Request(userId, groupId);
        SetupUsers([]);
        SetupGroups([]);

        // Assert
        var result = await _canGroupMemberBeRemovedBusinessRule.IsCorrespondsToBusiness(request, _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenGroupIsNotExistShouldReturnFail()
    {
        // Arrange
        var userId = 1;
        var groupId = 1;
        var request = new CanGroupMemberBeRemovedBusinessRule.Request(userId, groupId);

        var user = new User
        {
            Id = 1,
            IsActive = true,
            FirstName = "firstname",
            LastName = "lastname",
        };

        SetupUsers([user]);
        SetupGroups([]);

        // Assert
        var result = await _canGroupMemberBeRemovedBusinessRule.IsCorrespondsToBusiness(request, _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.GroupNotFound);
    }

    [Fact]
    public async Task HandleWhenUserIsNotGroupMemberShouldReturnFail()
    {
        // Arrange
        var userId = 1;
        var groupId = 1;
        var departmentId = ".Net";
        var request = new CanGroupMemberBeRemovedBusinessRule.Request(userId, groupId);

        var user = new User
        {
            Id = userId,
            IsActive = true,
            FirstName = "firstname",
            LastName = "lastname",
            ResourceGroupId = 2
        };

        var group = new ResourceGroup
        {
            Id = groupId,
            DepartmentId = departmentId,
            Name = "RG_TEST"
        };

        SetupUsers([user]);
        SetupGroups([group]);

        // Assert
        var result = await _canGroupMemberBeRemovedBusinessRule.IsCorrespondsToBusiness(request, _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.UserIsNotGroupMember);
    }

    [Fact]
    public async Task HandleWhenUserIsNotEmployeeShouldReturnFail()
    {
        // Arrange
        var userId = 1;
        var groupId = 1;
        var departmentId = ".Net";
        var request = new CanGroupMemberBeRemovedBusinessRule.Request(userId, groupId);

        var user = new User
        {
            Id = userId,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole() {Role = new Role{Name = PortalRoleNames.GroupManager, PublicName = PortalRoleNames.GroupManager } }
            },
            FirstName = "firstname",
            LastName = "lastname",
            ResourceGroupId = groupId,
        };

        var group = new ResourceGroup
        {
            Id = groupId,
            DepartmentId = departmentId,
            Name = "RG_TEST"
        };

        SetupUsers([user]);
        SetupGroups([group]);

        // Assert
        var result = await _canGroupMemberBeRemovedBusinessRule.IsCorrespondsToBusiness(request, _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.OnlyEmployeeGroupMemberCanBeUpdated);
    }

    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupGroups(List<ResourceGroup> groups)
    {
        _identityDbContextMock
            .Setup(mock => mock.ResourceGroups)
            .Returns(groups.BuildMockDbSet().Object);
    }
}
