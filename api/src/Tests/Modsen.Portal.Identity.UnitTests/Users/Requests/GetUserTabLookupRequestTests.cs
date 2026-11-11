using System.Linq.Expressions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetUserTabLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLookupLoader> _userLookupLoaderMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetUserTabLookupRequest _handler;
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();

    public GetUserTabLookupRequestTests()
    {
        _handler = new GetUserTabLookupRequest(
            _authenticatedUserContextMock.Object,
            _userLookupLoaderMock.Object,
            _identityDbContextMock.Object,
            _timeTrackContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserHasManageHrClaimShouldNotFilterByDepartmentOrResourceGroup()
    {
        // Arrange
        const string keyWord = "test";
        bool? isActive = true;
        var roles = new List<string> { "Admin" };
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = "dep",
                PositionDescription = "Developer",
                ResourceGroupId = 2
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.Roles == roles
            && x.DepartmentId == null
            && x.ResourceGroupId == null;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(true);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("John");
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentWithEmptyRolesShouldFilterByDepartmentId()
    {
        // Arrange
        const string keyWord = "search";
        bool? isActive = false;
        var roles = new List<string>();
        const string departmentId = "dept-123";
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>();

        var dbRoles = new List<Role>
        {
            new()
            {
                Id = 1,
                Name = PortalRoleNames.Employee,
                IsCompanyManager = false,
                PublicName = "Employee"
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == departmentId
            && x.ResourceGroupId == null;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.HeadOfDepartment });

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentWithCompanyManagerRolesShouldNotFilterByDepartmentId()
    {
        // Arrange
        const string keyWord = "admin";
        bool? isActive = true;
        var roles = new List<string> { PortalRoleNames.Ceo };
        const string departmentId = "dept-456";
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                PositionDescription = "CEO"
            }
        };

        var dbRoles = new List<Role>
        {
            new()
            {
                Id = 1,
                Name = PortalRoleNames.Ceo,
                IsCompanyManager = true,
                PublicName = "CEO"
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == null
            && x.ResourceGroupId == null;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.HeadOfDepartment });

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Admin");
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentWithMixedRolesOneNotCompanyManagerShouldFilterByDepartmentId()
    {
        // Arrange
        const string keyWord = null;
        bool? isActive = null;
        var roles = new List<string> { PortalRoleNames.Employee, PortalRoleNames.Ceo };
        const string departmentId = "dept-789";
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>();

        var dbRoles = new List<Role>
        {
            new()
            {
                Id = 1,
                Name = PortalRoleNames.Employee,
                IsCompanyManager = false,
                PublicName = "Employee"
            },
            new()
            {
                Id = 2,
                Name = PortalRoleNames.Ceo,
                IsCompanyManager = true,
                PublicName = "CEO"
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == departmentId
            && x.ResourceGroupId == null;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.HeadOfDepartment });

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerWithResourceGroupIdShouldFilterByResourceGroupId()
    {
        // Arrange
        const string keyWord = "manager";
        bool? isActive = true;
        var roles = new List<string>();
        const int resourceGroupId = 42;
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>();

        var dbRoles = new List<Role>();

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == null
            && x.ResourceGroupId == resourceGroupId;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.GroupManager });

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(resourceGroupId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerWithoutResourceGroupIdShouldSetResourceGroupIdToNegativeOne()
    {
        // Arrange
        const string keyWord = "test";
        bool? isActive = false;
        var roles = new List<string> { PortalRoleNames.Employee };
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>();

        var dbRoles = new List<Role>();

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == null
            && x.ResourceGroupId == -1;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.GroupManager });

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(-1);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserHasMultipleRolesShouldRespectRolesInQuery()
    {
        // Arrange
        const string keyWord = "lookup";
        bool? isActive = true;
        var roles = new List<string> { PortalRoleNames.Employee, PortalRoleNames.HeadOfDepartment };
        const string departmentId = "dept-multi";
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Multi",
                LastName = "Role",
                PositionDescription = "Employee"
            }
        };

        var dbRoles = new List<Role>
        {
            new()
            {
                Id = 1,
                Name = PortalRoleNames.Employee,
                IsCompanyManager = false,
                PublicName = "Employee"
            },
            new()
            {
                Id = 2,
                Name = PortalRoleNames.HeadOfDepartment,
                IsCompanyManager = false,
                PublicName = "Head of Department"
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.Roles.Count == 2
            && x.DepartmentId == departmentId;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.HeadOfDepartment });

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Multi");
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentButNotGroupManagerShouldNotSetResourceGroupId()
    {
        // Arrange
        const string keyWord = "test";
        bool? isActive = true;
        var roles = new List<string>();
        const string departmentId = "dept-head-only";
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>();

        var dbRoles = new List<Role>();

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == departmentId
            && x.ResourceGroupId == null;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.HeadOfDepartment });

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerButNotHeadOfDepartmentShouldNotSetDepartmentId()
    {
        // Arrange
        const string keyWord = "test";
        bool? isActive = true;
        var roles = new List<string>();
        const int resourceGroupId = 99;
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>();

        var dbRoles = new List<Role>();

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == null
            && x.ResourceGroupId == resourceGroupId;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.GroupManager });

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(resourceGroupId);

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeShouldNotFilterByDepartmentOrResourceGroup()
    {
        // Arrange
        const string keyWord = "employee";
        bool? isActive = true;
        var roles = new List<string>();
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var expectedUsers = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Regular",
                LastName = "Employee",
                PositionDescription = "Developer"
            }
        };

        var dbRoles = new List<Role>();

        Expression<Func<UserLookupLoaderRequestDto, bool>> loaderRequestMatch = x =>
            x.KeyWord == keyWord
            && x.IsActive == isActive
            && x.DepartmentId == null
            && x.ResourceGroupId == null;

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(false);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(new List<string> { PortalRoleNames.Employee });

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock
            .Setup(x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Regular");
        _userLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is(loaderRequestMatch), _cancellationToken),
            Times.Once);
    }
}
