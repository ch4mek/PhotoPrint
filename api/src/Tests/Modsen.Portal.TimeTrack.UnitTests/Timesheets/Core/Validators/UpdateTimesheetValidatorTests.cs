using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Validators;

public class UpdateTimesheetValidatorTests
{
    private readonly Mock<ICanUserUpdateTimesheetBusinessRule> _canUserUpdateTimesheetBusinessRuleMock = new();
    private readonly Mock<ICanTrackBillableHoursBusinessRule> _canTrackBillableHoursBusinessRuleMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetProjectActiveStatusDbQuery> _getProjectActiveStatusDbQueryMock = new();

    private readonly UpdateTimesheetValidator _validator;
    private readonly CancellationToken _token = CancellationToken.None;

    public UpdateTimesheetValidatorTests()
    {
        _validator = new UpdateTimesheetValidator(
            _canUserUpdateTimesheetBusinessRuleMock.Object,
            _canTrackBillableHoursBusinessRuleMock.Object,
            _authenticatedUserContextMock.Object,
            _getProjectActiveStatusDbQueryMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task UpdateTrackedTimeWhenNotManagerUserAndCannotTrackBillableTimeThrowsActionNotAllowedException(
        string userRole)
    {
        // Arrange
        var userId = 1;
        var projectId = 1;

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(2);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, projectId, 111, _token))
            .ReturnsAsync(false);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(projectId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(userId, projectId, 111, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotTrackBillableHoursWithoutPermission);
    }


    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task UpdateTrackedTimeWhenNotManagerUserAndCannotUpdateUsersTimesheetThrowsActionNotAllowedException(
        string userRole)
    {
        // Arrange
        var userId = 1;
        var projectId = 1;

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(2);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, projectId, 111, _token))
            .ReturnsAsync(true);

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(false);

        // Act
        var result = () => _validator.ValidateAsync(userId, projectId, 111, _token);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task UpdateTrackedTimeWhenNotManagerUserAndCanUpdateUsersTimesheetNotThrowsAnyException(
        string userRole)
    {
        // Arrange
        var userId = 1;
        var projectId = 1;

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(2);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);

        _canTrackBillableHoursBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, projectId, 111, _token))
            .ReturnsAsync(true);

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(projectId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(userId, projectId, 111, _token);

        //Assert
        await result.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task UpdateTrackedTimeWhenManagerCannotUpdateUsersTimesheetThrowsActionNotAllowed(string userRole)
    {
        // Arrange
        var userId = 1;
        var projectId = 1;

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(false);

        // Act
        var result = () => _validator.ValidateAsync(userId, projectId, 111, _token);

        //Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task UpdateTrackedTimeWhenManagerCanUpdateUsersTimesheetNotThrowsAnyException(string userRole)
    {
        // Arrange
        var userId = 1;
        var projectId = 1;

        _authenticatedUserContextMock.Setup(context => context.GetRoles()).Returns([userRole]);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(projectId, _token))
            .ReturnsAsync(true);

        // Act
        var result = () => _validator.ValidateAsync(userId, projectId, 111, _token);

        //Assert
        await result.Should().NotThrowAsync();
    }

}
