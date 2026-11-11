using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.Validators;

public class UpdateTaskNameValidatorTests
{
    private readonly Mock<ICanModifyTimesheetForDateBusinessRule> _canModifyTimesheetForDateBusinessRuleMock = new();
    private readonly Mock<ICanUserUpdateTimesheetBusinessRule> _canUserUpdateTimesheetBusinessRuleMock = new();
    private readonly Mock<INewTaskNameHasNoCollisionsBusinessRule> _newTaskNameHasNoCollisionsBusinessRuleMock = new();
    private readonly Mock<IGetProjectActiveStatusDbQuery> _getProjectActiveStatusDbQueryMock = new();

    private readonly UpdateTaskNameValidator _validator;

    private readonly CancellationToken _token = CancellationToken.None;

    public UpdateTaskNameValidatorTests()
    {
        _validator = new UpdateTaskNameValidator(
            _canModifyTimesheetForDateBusinessRuleMock.Object,
            _canUserUpdateTimesheetBusinessRuleMock.Object,
            _newTaskNameHasNoCollisionsBusinessRuleMock.Object,
            _getProjectActiveStatusDbQueryMock.Object);
    }

    [Fact]
    public async Task VaidateAsyncWhenUserCannotUpdateTimesheetThrowsActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var updateDto = new UpdateTaskNameDto { UserId = userId, OldTaskName = "name", TaskName = "taskName" };
        var timesheetsToUpdate = new List<TimesheetWithTrackedTimeCoreDto>();

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(false);

        //Act
        var action = () => _validator
            .ValidateAsync(updateDto, timesheetsToUpdate, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task VaidateAsyncWhenUserCannotModifyTimesheetForDateThrowsActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var projectId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Now);
        var updateDto = new UpdateTaskNameDto
        {
            UserId = userId,
            OldTaskName = "name",
            TaskName = "taskName",
            ProjectId = projectId,
        };

        var timesheetsToUpdate = new List<TimesheetWithTrackedTimeCoreDto>
        {
            new() { Id = 1, ReportForDate = reportForDate}
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
        var action = () => _validator
            .ValidateAsync(updateDto, timesheetsToUpdate, _token);

        //Assert
        await action.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.OnlyManagersCanModifyApprovedAndSubmittedTimesheets);
    }

    [Fact]
    public async Task VaidateAsyncWhenTimesheetHasCollisionThrowsEntityConflictException()
    {
        //Arrange
        var userId = 1;
        var reportForDate = DateOnly.FromDateTime(DateTime.Now);
        var updateDto = new UpdateTaskNameDto { UserId = userId, OldTaskName = "name", TaskName = "taskName" };

        var timesheetsToUpdate = new List<TimesheetWithTrackedTimeCoreDto>
        {
            new() { Id = 1, ReportForDate = reportForDate}
        };

        _canUserUpdateTimesheetBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, _token))
            .ReturnsAsync(true);

        _canModifyTimesheetForDateBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(userId, reportForDate, _token))
            .ReturnsAsync(true);

        _newTaskNameHasNoCollisionsBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(updateDto, timesheetsToUpdate, _token))
            .ReturnsAsync(false);

        _getProjectActiveStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(updateDto.ProjectId, _token))
            .ReturnsAsync(true);

        //Act
        var action = () => _validator
            .ValidateAsync(updateDto, timesheetsToUpdate, _token);

        //Assert
        await action.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.TaskWithSameNameAlreadyExists);
    }
}
