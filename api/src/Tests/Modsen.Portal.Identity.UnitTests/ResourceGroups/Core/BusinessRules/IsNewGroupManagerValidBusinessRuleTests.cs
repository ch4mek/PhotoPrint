using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Core.BusinessRules;

public class IsNewGroupManagerValidBusinessRuleTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly IsNewGroupManagerValidBusinessRule _isNewGroupManagerValidBusinessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public IsNewGroupManagerValidBusinessRuleTests()
    {
        _isNewGroupManagerValidBusinessRule = new(_identityDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsNotExistShouldReturnFail()
    {
        // Arrange
        var departmentId = ".Net";
        var userId = 1;
        SetupUsers([]);

        // Assert
        var result = await _isNewGroupManagerValidBusinessRule.IsCorrespondsToBusiness(new(userId, departmentId), _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserHasResourceGroupShouldReturnFail()
    {
        // Arrange
        var departmentId = ".Net";
        var userId = 1;
        var user = new User
        {
            Id = userId,
            FirstName = "test",
            LastName = "test",
            IsActive = true,
            UserRoles =
            [
                new()
                {
                    Role = new()
                    {
                        Name = PortalRoleNames.GroupManager,
                        PublicName = "Test"
                    }
                }
            ],
            ResourceGroup = new()
            {
                IsActive = true,
                Name = "RG_TEST",
                DepartmentId = departmentId
            },
            ResourceGroupId = 1
        };

        SetupUsers([user]);

        // Assert
        var result = await _isNewGroupManagerValidBusinessRule.IsCorrespondsToBusiness(new(userId, departmentId), _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.GroupManagerAlreadyHasActiveGroup);
    }

    [Fact]
    public async Task HandleWhenUserFromAnotherDepartmentShouldReturnFail()
    {
        // Arrange
        var departmentId = ".Net";
        var userId = 1;
        var user = new User
        {
            Id = userId,
            IsActive = true,
            FirstName = "test",
            LastName = "test",
            UserRoles =
            [
                new()
                {
                    Role = new()
                    {
                        Name = PortalRoleNames.GroupManager,
                        PublicName = "Test"
                    }
                }
            ],
            ResourceGroup = new()
            {
                IsActive = false,
                Name = "RG_TEST",
                DepartmentId = departmentId,
            },
            ResourceGroupId = 1,
            DepartmentId = "Another"
        };

        SetupUsers([user]);

        // Assert
        var result = await _isNewGroupManagerValidBusinessRule.IsCorrespondsToBusiness(new(userId, departmentId), _cancellationToken);

        // Act
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.GroupManagerIsFromAnotherDepartment);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnSuccess()
    {
        // Arrange
        var departmentId = ".Net";
        var userId = 1;
        var user = new User
        {
            Id = userId,
            IsActive = true,
            FirstName = "test",
            LastName = "test",
            UserRoles =
            [
                new()
                {
                    Role = new()
                    {
                        Name = PortalRoleNames.GroupManager,
                        PublicName = "Test"
                    }
                }
            ],
            ResourceGroup = new()
            {
                IsActive = false,
                Name = "RG_TEST",
                DepartmentId = departmentId,
            },
            ResourceGroupId = 1,
            DepartmentId = departmentId
        };

        SetupUsers([user]);

        // Assert
        var result = await _isNewGroupManagerValidBusinessRule.IsCorrespondsToBusiness(new(userId, departmentId), _cancellationToken);

        // Act
        result.IsSuccess.Should().BeTrue();
    }

    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }
}
