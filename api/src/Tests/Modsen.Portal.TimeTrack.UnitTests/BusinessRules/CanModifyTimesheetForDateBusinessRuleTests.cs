using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UnitTests.BusinessRules;

public class CanModifyTimesheetForDateBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetTimesheetSubmitStatusDbQuery> _getSubmitStatusDbQueryMock = new();

    private readonly CanModifyTimesheetForDateBusinessRule _businessRule;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanModifyTimesheetForDateBusinessRuleTests()
    {
        _businessRule = new CanModifyTimesheetForDateBusinessRule(
            _authenticatedUserContextMock.Object,
            _getSubmitStatusDbQueryMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessWhenUserIsManagerShouldReturnSuccess(string role)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserContextMock.Setup(x => x.IsManager).Returns(true);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId: 1,
            reportForDate: new DateOnly(2024, 12, 12), _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator, TimesheetSubmitStatusType.Rejected)]
    [InlineData(PortalRoleNames.Employee, TimesheetSubmitStatusType.Rejected)]
    [InlineData(PortalRoleNames.SystemAdministrator, null)]
    [InlineData(PortalRoleNames.Employee, null)]
    public async Task IsCorrespondsToBusinessWhenUserIsNotManagerAndStatusIsRejectedOrNullShouldReturnSuccess(
        string role, TimesheetSubmitStatusType? status)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        _getSubmitStatusDbQueryMock
            .Setup(x =>
                x.ExecuteAsync(1, new DateOnly(2024, 12, 12), _cancellationToken))
            .ReturnsAsync(status);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId: 1,
            reportForDate: new DateOnly(2024, 12, 12), _cancellationToken);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator, TimesheetSubmitStatusType.UnderReview)]
    [InlineData(PortalRoleNames.Employee, TimesheetSubmitStatusType.UnderReview)]
    [InlineData(PortalRoleNames.SystemAdministrator, TimesheetSubmitStatusType.Approved)]
    [InlineData(PortalRoleNames.Employee, TimesheetSubmitStatusType.Approved)]
    public async Task IsCorrespondsToBusinessWhenUserIsNotManagerAndStatusIsSubmittedOrApprovedShouldReturnFail(
        string role, TimesheetSubmitStatusType? status)
    {
        //Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        _getSubmitStatusDbQueryMock
            .Setup(x =>
                x.ExecuteAsync(1, new DateOnly(2024, 12, 12) , _cancellationToken))
            .ReturnsAsync(status);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId: 1,
            reportForDate: new DateOnly(2024, 12, 12), _cancellationToken);

        //Assert
        result.Should().BeFalse();
    }
}
