using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Core.BusinessRules;

public class CanGroupMemberBeAddedBusinessRuleTests
{
    private readonly Mock<ICanUserUpdateGroupBusinessRule> _canUserUpdateGroupBusinessRuleMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CanGroupMemberBeAddedBusinessRule _canGroupMemberBeAddedBusinessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanGroupMemberBeAddedBusinessRuleTests()
    {
        _canGroupMemberBeAddedBusinessRule = new(
            _canUserUpdateGroupBusinessRuleMock.Object,
            _identityDbContextMock.Object,
            _authenticatedUserContextMock.Object);   
    }

    [Fact]
    public async Task HandleWhenUserIsNotExistShouldReturnFail()
    {
        // Arrange
        SetupUsers([]);

        // Act
        var result = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(new(0, 0), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenGroupIsNotExistShouldReturnFail()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            IsActive = true,
            FirstName = "firstname",
            LastName = "lastname",
            UserRoles = new List<UserRole>
            {
                new UserRole() {Role = new Role{Name = PortalRoleNames.Employee, PublicName = PortalRoleNames.Employee } }
            }
        };

        SetupUsers([user]);
        SetupResourceGroups([]);

        // Act
        var result = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(new(1, 0), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be(PortalMessages.GroupNotFound);
    }

    [Fact]
    public async Task HandleWhenUserIsNotEmployeeShouldReturnFail()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole() {Role = new Role{Name = PortalRoleNames.Cto, PublicName = PortalRoleNames.Cto } }
            },
            FirstName = "firstname",
            LastName = "lastname",
        };

        var resourceGroup = new ResourceGroup
        {
            Id = 1,
            IsActive = true,
            DepartmentId = ".Net",
            Name = "RG_TEST"
        };

        SetupUsers([user]);
        SetupResourceGroups([resourceGroup]);

        // Act
        var result = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(new(1, 1), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be(PortalMessages.OnlyEmployeeGroupMemberCanBeUpdated);
    }

    [Fact]
    public async Task HandleWhenUserHasResourceGroupShouldReturnFail()
    {
        // Arrange
        var userEmail = "email";

        var user = new User
        {
            Id = 1,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole() {Role = new Role{Name = PortalRoleNames.Employee, PublicName = PortalRoleNames.Employee } }
            },
            FirstName = "firstname",
            LastName = "lastname",
            ResourceGroupId = 1,
            Email = userEmail
        };

        var resourceGroup = new ResourceGroup
        {
            Id = 1,
            IsActive = true,
            DepartmentId = ".Net",
            Name = "RG_TEST"
        };

        SetupUsers([user]);
        SetupResourceGroups([resourceGroup]);

        // Act
        var result = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(new(1, 1), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be($"User {userEmail} is already in the group");
    }

    [Fact]
    public async Task HandleWhenUserFromOtherDepartmentShouldReturnFail()
    {
        // Arrange
        var userDepartment = ".Net";
        var userEmail = "email";

        var user = new User
        {
            Id = 1,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole() {Role = new Role{Name = PortalRoleNames.Employee, PublicName = PortalRoleNames.Employee } }
            },
            FirstName = "firstname",
            LastName = "lastname",
            ResourceGroupId = null,
            Email = userEmail,
            DepartmentId = userDepartment
        };

        var resourceGroup = new ResourceGroup
        {
            Id = 1,
            IsActive = true,
            DepartmentId = ".Net",
            Name = "RG_TEST"
        };

        SetupUsers([user]);
        SetupResourceGroups([resourceGroup]);
        SetupCanUserUpdateGroup(true);
        SetupUserRoles([PortalRoleNames.HeadOfDepartment]);
        SetupUserDepartment("other");

        // Act
        var result = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(new(1, 1), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be(PortalMessages.HeadCannotAssignUserFromAnotherDepartmentToGroup);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnSuccses()
    {
        // Arrange
        var userDepartment = ".Net";
        var userEmail = "email";

        var user = new User
        {
            Id = 1,
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole() {Role = new Role{Name = PortalRoleNames.Employee, PublicName = PortalRoleNames.Employee } }
            },
            FirstName = "firstname",
            LastName = "lastname",
            ResourceGroupId = null,
            Email = userEmail,
            DepartmentId = userDepartment
        };

        var resourceGroup = new ResourceGroup
        {
            Id = 1,
            IsActive = true,
            DepartmentId = ".Net",
            Name = "RG_TEST"
        };

        SetupUsers([user,]);
        SetupResourceGroups([resourceGroup]);
        SetupCanUserUpdateGroup(true);
        SetupUserRoles([PortalRoleNames.HeadOfDepartment]);
        SetupUserDepartment(userDepartment);

        // Act
        var result = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(new(1, 1), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(true);
    }


    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupResourceGroups(List<ResourceGroup> resourceGroups)
    {
        _identityDbContextMock
            .Setup(mock => mock.ResourceGroups)
            .Returns(resourceGroups.BuildMockDbSet().Object);
    }

    private void SetupCanUserUpdateGroup(bool result)
    {
        _canUserUpdateGroupBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<CanUserUpdateGroupBusinessRule.Request>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());
    }

    private void SetupUserRoles(string[] roles)
    {
        _authenticatedUserContextMock
            .Setup(mock => mock.GetRoles())
            .Returns(roles);
    }

    private void SetupUserDepartment(string departmentId)
    {
        _authenticatedUserContextMock
            .Setup(mock => mock.DepartmentId)
            .Returns(departmentId);
    }
}
