using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanUserBeUpdatedBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly CanUserBeUpdatedBusinessRule _businessRule;

    public CanUserBeUpdatedBusinessRuleTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _businessRule = new CanUserBeUpdatedBusinessRule(_authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task ValidateCeoPermissionsShouldFailWhenCeoOrCtoIsBeingUpdated()
    {
        // Arrange
        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                RoleNames = [PortalRoleNames.Cto]
            },
            null, null, null);

        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.Ceo]);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.UnsupportedUserUpdate);
    }

    [Fact]
    public async Task ValidateCtoPermissionsShouldFailWhenCeoOrCtoIsBeingUpdated()
    {
        // Arrange
        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                RoleNames = [PortalRoleNames.Ceo]
            },
            null, null, null);

        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.Cto]);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.UnsupportedUserUpdate);
    }

    [Fact]
    public async Task ValidateHeadOfDepartmentPermissionsShouldFailWhenEditingEmail()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 2,
                RoleNames = [PortalRoleNames.Employee]
            }, "Net", null, "newEmail");

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.UnsupportedUserUpdate);
    }

    [Fact]
    public async Task ValidateHeadOfDepartmentPermissionsShouldFailWhenEditingHimself()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 1,
                DepartmentId = "Net",
                RoleNames = [PortalRoleNames.Employee]
            }, "Net", null, null);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("Head of department cannot edit himself");
    }

    [Fact]
    public async Task ValidateHeadOfDepartmentPermissionsShouldFailWhenEditingUserFromAnotherDepartment()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 2,
                DepartmentId = "Net",
                RoleNames = [PortalRoleNames.Employee]
            }, "Net", null, "test");

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
    }

    [Fact]
    public async Task ValidateGroupManagerPermissionsShouldFailWhenEditingCeo()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 2,
                RoleNames = [PortalRoleNames.Ceo]
            }, null, null, "test");

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup);
    }

    [Fact]
    public async Task ValidateGroupManagerPermissionsShouldFailWhenEditingHimself()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.GroupManager]);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 1,
                RoleNames = [PortalRoleNames.GroupManager]
            }, "Net", null, null);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be("GM cannot edit himself");
    }

    [Fact]
    public async Task ValidateGroupManagerPermissionsShouldFailWhenEditingUserFromAnotherGroup()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(ctx => ctx.ResourceGroupId).Returns(1);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 2,
                DepartmentId = "Net",
                RoleNames = [PortalRoleNames.Employee],
                ResourceGroupId = 2
            }, "Net", 2, "test");

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup);
    }

    [Fact]
    public async Task ValidateHeadOfDepartmentPermissionsShouldFailWhenEditingCeo()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(ctx => ctx.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(ctx => ctx.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        var request = new CanUserBeUpdatedBusinessRule.Request(
            new UserDto
            {
                FirstName = "test",
                Email = "test",
                Id = 2,
                RoleNames = [PortalRoleNames.Ceo]
            }, null, null, "test");

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
    }
}
