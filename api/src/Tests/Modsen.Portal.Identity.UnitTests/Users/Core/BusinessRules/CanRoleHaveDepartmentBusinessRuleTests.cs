using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanRoleHaveDepartmentBusinessRuleTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly CanRoleHaveDepartmentBusinessRule _rule;
    private readonly CancellationToken _cancellationToken;

    public CanRoleHaveDepartmentBusinessRuleTests()
    {
        _rule = new(_identityDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRoleIsNotDefinedShouldReturnFail()
    {
        // Arrange
        var model = new CanRoleHaveDepartmentBusinessRule.Request(It.IsAny<string>(), "TestDepartment");

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role>().BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Unsupported RoleTypes");
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task IsCorrespondsToBusinessWhenRoleMustNotHaveDepartmentAndDepartmentIdIsProvidedShouldReturnFail(string role)
    {
        // Arrange
        var model = new CanRoleHaveDepartmentBusinessRule.Request(role, "TestDepartment");

        var roleEntity = new Role
        {
            Name = role,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("CTO and CEO cannot have department");
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task IsCorrespondsToBusinessWhenRoleMustNotHaveDepartmentAndDepartmentIdIsNotProvidedShouldReturnSuccess(string role)
    {
        // Arrange
        var model = new CanRoleHaveDepartmentBusinessRule.Request(role, null);

        var roleEntity = new Role
        {
            Name = role,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenRoleMustHaveDepartmentAndDepartmentIdIsProvidedShouldReturnFail(string role)
    {
        // Arrange
        var model = new CanRoleHaveDepartmentBusinessRule.Request(role, null);

        var roleEntity = new Role
        {
            Name = role,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        // Act
        var result = await _rule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("SystemAdministrator, HeadOfDepartment, GroupManager, Employee and Intern must have department");
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task IsCorrespondsToBusinessWhenRoleMustHaveDepartmentAndDepartmentIdIsProvidedShouldReturnSuccess(string role)
    {
        var roleEntity = new Role
        {
            Name = role,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        // Arrange
        var model = new CanRoleHaveDepartmentBusinessRule.Request(role, "TestDepartment");

        // Act
        var result = await _rule.IsCorrespondsToBusiness(model, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
