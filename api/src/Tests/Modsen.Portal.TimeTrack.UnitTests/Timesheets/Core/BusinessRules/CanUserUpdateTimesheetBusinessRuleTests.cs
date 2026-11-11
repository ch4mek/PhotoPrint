using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class CanUserUpdateTimesheetBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetTimeTrackUserDbQuery> _getUserDbQueryMock = new();
    private readonly Mock<IGetUserActiveStatusDbQuery> _getUserActiveStatusDbQueryMock = new();

    private readonly CanUserUpdateTimesheetBusinessRule _businessRule;

    private readonly CancellationToken _token = CancellationToken.None;

    public CanUserUpdateTimesheetBusinessRuleTests()
    {
        _businessRule = new CanUserUpdateTimesheetBusinessRule(
            _getUserDbQueryMock.Object,
            _authenticatedUserContextMock.Object,
            _getUserActiveStatusDbQueryMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenNotManagerUserTriesToUpdateTimesheetOfAnotherUserReturnsFalse(string userRole)
    {
        // Arrange
        var userId = 2;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task IsCorrespondsToBusinessWhenNotManagerUserTriesToUpdateHisTimesheetReturnsTrue(string userRole)
    {
        // Arrange
        var userId = 1;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(userId);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task IsCorrespondsToBusinessAuthedAsManagerWhenUserDoesNotExistThrowsInvalidDataAppException(string userRole)
    {
        // Arrange
        var userId = 2;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync((TimeTrackUser?)null);

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadOfDepartmentTryUpdateTimesheetForUserFromAnotherDepartmentReturnsFalse()
    {
        // Arrange
        var userId = 2;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser { DepartmentId = "Java", Position = PortalRoleNames.Employee, FirstName = "test", LastName = "test", Email = "test" });

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGroupManagerTryUpdateTimesheetForUserFromAnotherGroupReturnsFalse()
    {
        // Arrange
        var userId = 2;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                ResourceGroupId = 2,
                Position = PortalRoleNames.Employee,
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsInactiveShouldThrowActionNotAllowedException()
    {
        // Arrange
        var userId = 2;
        var resourceGroupId = 1;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(resourceGroupId);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                ResourceGroupId = resourceGroupId,
                Position = PortalRoleNames.Employee,
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(false);

        // Act
        var result = () => _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotManageTimesheetOfInactiveUser);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenManagerAndRequestValidReturnsTrue()
    {
        // Arrange
        var userId = 2;
        var resourceGroupId = 1;
        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(context => context.ResourceGroupId).Returns(resourceGroupId);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _getUserDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(new TimeTrackUser
            {
                DepartmentId = "Net",
                ResourceGroupId = resourceGroupId,
                Position = PortalRoleNames.Employee,
                FirstName = "test",
                LastName = "test",
                Email = "test"
            });

        _getUserActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, _token))
            .ReturnsAsync(true);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        result.Should().BeTrue();
    }
}
