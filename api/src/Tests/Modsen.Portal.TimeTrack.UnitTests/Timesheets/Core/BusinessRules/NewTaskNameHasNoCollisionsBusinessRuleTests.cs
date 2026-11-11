using MockQueryable.Moq;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.BusinessRules;

public class NewTaskNameHasNoCollisionsBusinessRuleTests
{
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly NewTaskNameHasNoCollisionsBusinessRule _businessRule;
    private readonly CancellationToken _token;

    public NewTaskNameHasNoCollisionsBusinessRuleTests()
    {
        _businessRule = new NewTaskNameHasNoCollisionsBusinessRule(_dbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrepondsToBusinessWhenTaskWithSameNameExistsReturnsFalse()
    {
        //Arrange
        const string taskName = "New Task Name";

        var startDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-7);
        var endDate = startDate.AddDays(5);
        var reportForDate = startDate.AddDays(2);

        var updateDto = new UpdateTaskNameDto
        {
            UserId = 1,
            TaskName = taskName,
            OldTaskName = "Task 1",
            StartDate = startDate,
            EndDate = endDate,
            ProjectId = 1
        };

        var timesheetWithTrackedTimeDto = new TimesheetWithTrackedTimeCoreDto
        {
            Id = 1, ReportForDate = reportForDate, TrackedInMinutes = 120
        };

        _dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(new List<Timesheet>
            {
                new()
                {
                    Id = 1,
                    UserId = updateDto.UserId,
                    ProjectId = updateDto.ProjectId,
                    TaskName = taskName,
                    ReportForDate = reportForDate
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(updateDto, [timesheetWithTrackedTimeDto], _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrepondsToBusinessWhenTaskWithSameNameNotExistsReturnsTrue()
    {
        //Arrange
        const string taskName = "New Task Name";

        var startDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-7);
        var endDate = startDate.AddDays(5);
        var reportForDate = startDate.AddDays(2);

        var updateDto = new UpdateTaskNameDto
        {
            UserId = 1,
            TaskName = taskName,
            OldTaskName = "Task 1",
            StartDate = startDate,
            EndDate = endDate,
            ProjectId = 1
        };

        var timesheetWithTrackedTimeDto = new TimesheetWithTrackedTimeCoreDto
        {
            Id = 1, ReportForDate = reportForDate, TrackedInMinutes = 120
        };

        _dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(new List<Timesheet>
            {
                new()
                {
                    Id = 1,
                    UserId = updateDto.UserId,
                    ProjectId = updateDto.ProjectId,
                    TaskName = "some name",
                    ReportForDate = reportForDate
                }
            }.BuildMockDbSet().Object);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(updateDto, [timesheetWithTrackedTimeDto], _token);

        //Assert
        result.Should().BeTrue();
    }
}
