using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Validators;

public class DeleteTimesheetValidatorTests
{
    private readonly Mock<ICanModifyTimesheetForDateBusinessRule> _canModifyTimesheetForDateBusinessRuleMock = new();
    private readonly Mock<ICanUserUpdateTimesheetBusinessRule> _canUserUpdateTimesheetBusinessRuleMock = new();
    private readonly Mock<IGetProjectActiveStatusDbQuery> _getProjectActiveStatusDbQueryMock = new();

    private readonly DeleteTimesheetValidator _validator;

    private readonly CancellationToken _token = CancellationToken.None;

    public DeleteTimesheetValidatorTests()
    {
        _validator = new DeleteTimesheetValidator(
            _canModifyTimesheetForDateBusinessRuleMock.Object,
            _canUserUpdateTimesheetBusinessRuleMock.Object,
            _getProjectActiveStatusDbQueryMock.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserCannotUpdateTimesheetShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var timesheet = new Timesheet { UserId = userId, TaskName = "Test 1" };

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(false);

        //Act
        var action = () => _validator.ValidateAsync(timesheet, _token);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task ValidateAsyncWhenUserCannotModifyTimesheetShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var projectId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Today);
        var timesheet = new Timesheet
        {
            UserId = userId,
            ReportForDate = reportForDate,
            ProjectId = projectId,
            TaskName = "Test 1"
        };

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _canModifyTimesheetForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, reportForDate, _token))
            .ReturnsAsync(false);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(projectId, _token))
            .ReturnsAsync(true);

        //Act
        var action = () => _validator.ValidateAsync(timesheet, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage("Employee and SysAdmin cannot modify timesheets with Approved or Submitted statuses");
    }
}
