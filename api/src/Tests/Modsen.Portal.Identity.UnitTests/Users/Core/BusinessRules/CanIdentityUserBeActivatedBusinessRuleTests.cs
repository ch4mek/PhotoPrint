using System.Linq.Expressions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;
using Moq;
using UserDto = Modsen.Portal.Identity.Requests.Dto.UserDto;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanUserDtoBeActivatedBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IUserLookupLoader> _userLookupLoaderMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();

    private readonly CanIdentityUserBeActivatedBusinessRule _canIdentityUserBeActivatedBusinessRule;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<IIdentityPortalSettingsAccessor> _settingsAccessor = new();

    public CanUserDtoBeActivatedBusinessRuleTests()
    {
        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityPortalSettingsDto
            {
                MaxCeoCount = 1,
                MaxCtoCount = 1,
                MaxHeadOfSpecificDepartmentCount = 1
            });

        _canIdentityUserBeActivatedBusinessRule = new CanIdentityUserBeActivatedBusinessRule(
            _authenticatedUserContext.Object,
            _settingsAccessor.Object,
            _identityDbContextMock.Object,
            _userLookupLoaderMock.Object
            );
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Ceo, PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.Cto, PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Cto, PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenCeoOrCtoActivatesEmployeeOrSysAdminShouldReturnSuccessfulBusinessRuleResult(
        string currentUserRole,
        string userToActivateRole)
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = new List<string> { userToActivateRole }
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([currentUserRole]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().BeEquivalentTo(BusinessRuleResult.Success());
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessWhenCeoOrCtoActivatesHeadOfDepartmentAndThereIsNoActiveHeadOfThatDepartmentShouldReturnSuccessfulBusinessRuleResult(
        string currentUserRole)
    {
        // Arrange
        var userToActivateRole = PortalRoleNames.HeadOfDepartment;
        const string departmentId = "Net";

        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = new List<string> { userToActivateRole },
            DepartmentId = departmentId
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([currentUserRole]);

        Expression<Func<UserLookupLoaderRequestDto, bool>> dbRequestMatch = r =>
            r.KeyWord == null
            && r.DepartmentId == departmentId
            && r.IsActive == true
            && r.Roles.Contains(PortalRoleNames.HeadOfDepartment);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync([]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().BeEquivalentTo(BusinessRuleResult.Success());

        _userLookupLoaderMock.Verify(x =>
                x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessWhenCeoOrCtoActivatesHeadOfDepartmentAndThereIsActiveHeadOfThatDepartmentShouldReturnFailedBusinessRuleResult(
        string currentUserRole)
    {
        // Arrange
        var userToActivateRole = PortalRoleNames.HeadOfDepartment;
        const string departmentId = "Net";

        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [userToActivateRole],
            DepartmentId = departmentId
        };

        var userLookupDto = new UserLookupDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            PositionDescription = userToActivateRole,
            DepartmentId = departmentId
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([currentUserRole]);

        Expression<Func<UserLookupLoaderRequestDto, bool>> dbRequestMatch = r =>
            r.KeyWord == null
            && r.DepartmentId == departmentId
            && r.IsActive == true
            && r.Roles.Contains(PortalRoleNames.HeadOfDepartment);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync([userLookupDto]);

        var department = new Department
        {
            Id = "Net",
            FullName = ".NET",
            ShortName = "Net"
        };

        _identityDbContextMock
            .Setup(x => x.Departments)
            .Returns(new List<Department> { department }.BuildMockDbSet().Object);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));

        _userLookupLoaderMock.Verify(x =>
                x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCeoActivatesCtoAndThereIsNoActiveCtoShouldReturnSuccessfulBusinessRuleResult()
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [PortalRoleNames.Cto]
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().BeEquivalentTo(BusinessRuleResult.Success());
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCeoActivatesCtoAndThereIsActiveCtoShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [PortalRoleNames.Cto]
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);

        var userEntity = new User
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            UserRoles = [ new UserRole { Role = new Role { Name = PortalRoleNames.Cto, PublicName = "Test" } } ],
            IsActive = true,
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>{ userEntity }.BuildMockDbSet().Object);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessWhenCtoActivatesCeoOrCtoShouldReturnFailedBusinessRuleResult(string role)
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = new List<string> { role }
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadOfDepartmentActivatesEmployeeFromDepartmentShouldReturnSuccessfulBusinessRuleResult()
    {
        // Arrange
        var departmentId = "Ruby";

        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = new List<string> { PortalRoleNames.Employee },
            DepartmentId = departmentId
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns(departmentId);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().BeEquivalentTo(BusinessRuleResult.Success());
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadOfDepartmentActivatesEmployeeFromAnotherDepartmentShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        var departmentId = "Ruby";

        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = new List<string> { PortalRoleNames.Employee },
            DepartmentId = departmentId
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Mobile");

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenHeadOfDepartmentActivatesNotEmployeeRoleUserShouldReturnFailedBusinessRuleResult(
        string userToActivate)
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = new List<string> { userToActivate }
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Mobile");

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGroupManagerActivatesEmployeeFromGroupShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        var groupId = 1;

        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [PortalRoleNames.Employee],
            ResourceGroupId = groupId
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContext.Setup(x => x.ResourceGroupId).Returns(groupId);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess && !string.IsNullOrEmpty(r.Reason));
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGroupManagerActivatesEmployeeFromAnotherGroupShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        var groupId = 1;

        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [PortalRoleNames.Employee],
            ResourceGroupId = groupId,
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContext.Setup(x => x.ResourceGroupId).Returns(2);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenGroupManagerActivatesNotEmployeeRoleUserShouldReturnFailedBusinessRuleResult(
        string userToActivate)
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [userToActivate]
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenSysAdminActivatesNotEmployeeRoleUserShouldReturnFailedBusinessRuleResult(
        string userToActivate)
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [userToActivate]
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSysAdminActivatesEmployeeRoleUserShouldReturnSuccessBusinessRuleResult()
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [PortalRoleNames.Employee]
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().BeEquivalentTo(BusinessRuleResult.Success());
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenEmployeeActivatesUserShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        var user = new UserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "example@modsen-software.com",
            RoleNames = [PortalRoleNames.Employee]
        };

        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);

        // Act
        var actualResult = await _canIdentityUserBeActivatedBusinessRule.IsCorrespondsToBusiness(user, _cancellationToken);

        // Assert
        actualResult.Should().Match<BusinessRuleResult>(r =>
            !r.IsSuccess
            && !string.IsNullOrEmpty(r.Reason));
    }
}
