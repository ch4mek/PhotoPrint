using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanRoleBeOccupiedBusinessRuleTests
{
    private readonly Mock<IIdentityDbContext> _IdentityDbContextMock = new();
    private readonly Mock<IDepartmentsCache> _departmentsCacheMock = new();

    private readonly CanRoleBeOccupiedBusinessRule _businessRule;

    private readonly CancellationToken _cancellationToken;

    public CanRoleBeOccupiedBusinessRuleTests()
    {
        _businessRule = new CanRoleBeOccupiedBusinessRule(
            _IdentityDbContextMock.Object,
            _departmentsCacheMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, ".Net")]
    public async Task IsCorrespondsToBusinessWhenRoleIsOccupiedShouldReturnFailedBusinessRuleResult(string role, string? departmentId)
    {
        // Arrange
        var request = new CanRoleBeOccupiedBusinessRule.Request
        (
            Position: role,
            DepartmentId: departmentId,
            ResourceGroupId: null
        );

        var roleEntity = new Role
        {
            Name = role,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        var users = new List<User>
        {
            new()
            {
                FirstName = "test",
                LastName = "test",
                UserRoles = [ new UserRole { Role = roleEntity } ],
                IsActive = true,
                DepartmentId = departmentId,
            }
        };

        var departments = new List<DepartmentCacheRecord>
        {
            new() { Id = departmentId, FullName = departmentId, ShortName = departmentId }
        };

        _IdentityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        _IdentityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        _departmentsCacheMock
            .Setup(x => x.GetAllDepartments(_cancellationToken))
            .ReturnsAsync(departments);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.Should().Match<BusinessRuleResult>(x =>
            !x.IsSuccess && x.Reason != null);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGroupManagerRoleIsOccupiedShouldReturnFailedBusinessRuleResult()
    {
        // Arrange
        var request = new CanRoleBeOccupiedBusinessRule.Request
        (
            Position: PortalRoleNames.GroupManager,
            DepartmentId: null,
            ResourceGroupId: 1
        );

        var roleEntity = new Role
        {
            Name = request.Position,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        var users = new List<User>
        {
            new()
            {
                FirstName = "test",
                LastName = "test",
                UserRoles = [ new UserRole { Role = roleEntity } ],
                IsActive = true,
                ResourceGroupId = request.ResourceGroupId,
                ResourceGroup = new() { Id = (int)request.ResourceGroupId!, Name = "RG_TEST", DepartmentId = "Net"}
            }
        };

        _IdentityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        _IdentityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.Should().Match<BusinessRuleResult>(x =>
            !x.IsSuccess && x.Reason != null);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    public async Task IsCorrespondsToBusinessWhenRoleIsNotOccupiedShouldReturnSuccessfulBusinessRuleResult(string role)
    {
        // Arrange
        var user = new CanRoleBeOccupiedBusinessRule.Request
        (
            Position: role,
            DepartmentId: null,
            ResourceGroupId: null
        );

        var roleEntity = new Role
        {
            Name = role,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _IdentityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        _IdentityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        var expectedResult = BusinessRuleResult.Success();

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(user, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
