using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class CanModifyTimesheetForDateBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetTimesheetSubmitStatusDbQuery> _getSubmitStatusDbQueryMock = new();

    private readonly CanModifyTimesheetForDateBusinessRule _businessRule;

    private readonly CancellationToken _token = CancellationToken.None;

    public CanModifyTimesheetForDateBusinessRuleTests()
    {
        _businessRule = new CanModifyTimesheetForDateBusinessRule(
            _authenticatedUserContextMock.Object,
            _getSubmitStatusDbQueryMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsManagerReturnsTrue()
    {
        // Arrange
        var userId = 2;
        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(true);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, reportForDate, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsNotManagerAndStatusIsUnderReviewReturnsFalse()
    {
        // Arrange
        var userId = 2;
        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);

        _getSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, reportForDate, _token))
            .ReturnsAsync(TimesheetSubmitStatusType.UnderReview);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, reportForDate, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsNotManagerAndStatusIsApprovedReturnsFalse()
    {
        // Arrange
        var userId = 2;
        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);

        _getSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, reportForDate, _token))
            .ReturnsAsync(TimesheetSubmitStatusType.Approved);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, reportForDate, _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsNotManagerAndStatusIsRejectedReturnsTrue()
    {
        // Arrange
        var userId = 2;
        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);

        _getSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, reportForDate, _token))
            .ReturnsAsync(TimesheetSubmitStatusType.Rejected);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, reportForDate, _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenUserIsNotManagerAndStatusIsNullReturnsTrue()
    {
        // Arrange
        var userId = 2;
        var reportForDate = DateOnly.FromDateTime(DateTime.UtcNow);

        _authenticatedUserContextMock.Setup(context => context.IsManager).Returns(false);
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);

        _getSubmitStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(userId, reportForDate, _token))
            .ReturnsAsync((TimesheetSubmitStatusType?)null);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(userId, reportForDate, _token);

        //Assert
        result.Should().BeTrue();
    }
}
