using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanCurrentUserCreateUserWithRoleBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CanCurrentUserCreateUserWithRoleBusinessRule _businessRule;

    public CanCurrentUserCreateUserWithRoleBusinessRuleTests()
    {
        _businessRule = new CanCurrentUserCreateUserWithRoleBusinessRule(_authenticatedUserContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Java")]
    [InlineData(PortalRoleNames.SystemAdministrator, "Administartion")]
    public async Task IsCorrespondsToBusinessWhenHeadOfDepartmentTryToCreateNotEmployeeShouldReturnFailedResult(
        string role, string? departmentId)
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        var currentUserCreateUser = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(
            Position: role,
            DepartmentId: departmentId,
            ResourceGroupId: null);

        // Act
        var actualResult = await _businessRule.IsCorrespondsToBusiness(currentUserCreateUser, CancellationToken.None);

        // Assert
        actualResult.IsSuccess
            .Should().BeFalse();
        actualResult.Reason
            .Should().Be(
                $"You can create only Employee(Intern) with {_authenticatedUserContextMock.Object.DepartmentId} department");
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null)]
    [InlineData(PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Java")]
    [InlineData(PortalRoleNames.SystemAdministrator, "Administartion")]
    public async Task IsCorrespondsToBusinessWhenGroupManagerTryToCreateNotEmployeeShouldReturnFailedResult(
        string role, string? departmentId)
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        var currentUserCreateUser = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(
            Position: role,
            DepartmentId: departmentId,
            ResourceGroupId: null);

        // Act
        var actualResult = await _businessRule.IsCorrespondsToBusiness(currentUserCreateUser, CancellationToken.None);

        // Assert
        actualResult.IsSuccess
            .Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadOfDepartmentTryToCreateEmployeeFromAnotherDepartmentShouldReturnFailedResult()
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Js");

        var currentUserCreateUser = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(
            Position: PortalRoleNames.Employee,
            DepartmentId: "Java",
            ResourceGroupId: null);

        // Act
        var actualResult = await _businessRule.IsCorrespondsToBusiness(currentUserCreateUser, CancellationToken.None);

        // Assert
        actualResult.IsSuccess
            .Should().BeFalse();
        actualResult.Reason
            .Should().Be(
                $"You can create only Employee(Intern) with {_authenticatedUserContextMock.Object.DepartmentId} department");
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGroupManagerTryToCreateEmployeeFromAnotherGroupShouldReturnFailedResult()
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(1);

        var currentUserCreateUser = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(
            Position: PortalRoleNames.Employee,
            DepartmentId: "Java",
            ResourceGroupId: 2);

        // Act
        var actualResult = await _businessRule.IsCorrespondsToBusiness(currentUserCreateUser, CancellationToken.None);

        // Assert
        actualResult.IsSuccess
            .Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenTryToCreateCeoShouldReturnFailedResult()
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        var currentUserCreateUser = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(
            Position: PortalRoleNames.Ceo,
            DepartmentId: null,
            ResourceGroupId: null);

        // Act
        var actualResult = await _businessRule.IsCorrespondsToBusiness(currentUserCreateUser, CancellationToken.None);

        // Assert
        actualResult.IsSuccess
            .Should().BeFalse();
        actualResult.Reason
            .Should().Be("You cannot create user with CEO role");
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo, null, PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.Ceo, null, PortalRoleNames.HeadOfDepartment, "Java")]
    [InlineData(PortalRoleNames.Ceo, null, PortalRoleNames.SystemAdministrator, "Administration")]
    [InlineData(PortalRoleNames.Ceo, null, PortalRoleNames.Employee, "Java")]
    [InlineData(PortalRoleNames.Cto, null, PortalRoleNames.HeadOfDepartment, "Java")]
    [InlineData(PortalRoleNames.Cto, null, PortalRoleNames.SystemAdministrator, "Administration")]
    [InlineData(PortalRoleNames.Cto, null, PortalRoleNames.Employee, "Java")]
    [InlineData(PortalRoleNames.HeadOfDepartment, "Java", PortalRoleNames.Employee, "Java")]
    public async Task WhenValidRoleAndDepartmentIdShouldReturnSuccessResult(
        string role, string? departmentId, string position, string? positionDepartmentId)
    {
        // Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([role]);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(departmentId);

        var currentUserCreateUser = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(
            Position: position,
            DepartmentId: positionDepartmentId,
            ResourceGroupId: null);

        // Act
        var actualResult = await _businessRule.IsCorrespondsToBusiness(currentUserCreateUser, CancellationToken.None);

        // Assert
        actualResult.IsSuccess
            .Should().BeTrue();
        actualResult.Reason
            .Should().BeNull();
    }
}
