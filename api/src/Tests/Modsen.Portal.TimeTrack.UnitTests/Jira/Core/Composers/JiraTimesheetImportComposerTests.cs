using MockQueryable.Moq;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Jira.Core.Composers;

public class JiraTimesheetImportComposerTests
{
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();
    private readonly Mock<ITimesheetComposer> _timesheetComposerMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly JiraTimesheetImportComposer _composer;

    public JiraTimesheetImportComposerTests()
    {
        _composer = new JiraTimesheetImportComposer(
            _dbContextMock.Object,
            _timesheetComposerMock.Object);
    }

    [Fact]
    public async Task ComposeShouldReturnEmptyResultWhenNoWorklogs()
    {
        var result = await _composer.Compose([], 1, 1, new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31), _token);

        result.Created.Should().Be(0);
        result.Updated.Should().Be(0);
        result.Skipped.Should().Be(0);
        result.Total.Should().Be(0);

        _timesheetComposerMock.Verify(
            x => x.Compose(It.IsAny<UpsertFactDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ComposeShouldCreateTimesheetWhenTimesheetDoesNotExist()
    {
        const int userId = 1;
        const int projectId = 2;

        _dbContextMock
            .Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = userId,
                    ProjectId = projectId,
                    UserOnProjectFrom = new DateOnly(2024, 1, 1)
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>().BuildMockDbSet().Object);

        var result = await _composer.Compose(
            [CreateWorklog()],
            userId,
            projectId,
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 31),
            _token);

        result.Created.Should().Be(1);
        result.Updated.Should().Be(0);
        result.Skipped.Should().Be(0);
        result.Total.Should().Be(1);

        _timesheetComposerMock.Verify(
            x => x.Compose(
                It.IsAny<UpsertFactDto>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ComposeShouldUpdateTimesheetWhenTimesheetAlreadyExists()
    {
        var date = new DateOnly(2025, 1, 1);

        _dbContextMock
            .Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = 1,
                    ProjectId = 1,
                    UserOnProjectFrom = date.AddDays(-1)
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>
            {
                new()
                {
                    UserId = 1,
                    ProjectId = 1,
                    ReportForDate = date,
                    TaskName = "[TEST-1] Task"
                }
            }.BuildMockDbSet().Object);

        var result = await _composer.Compose(
            [CreateWorklog()],
            1,
            1,
            date,
            date,
            _token);

        result.Created.Should().Be(0);
        result.Updated.Should().Be(1);
        result.Skipped.Should().Be(0);
        result.Total.Should().Be(1);

        _timesheetComposerMock.Verify(
            x => x.Compose(
                It.IsAny<UpsertFactDto>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ComposeShouldSkipTimesheetWhenUserIsNotAssignedToProject()
    {
        _dbContextMock
            .Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>().BuildMockDbSet().Object);

        var result = await _composer.Compose(
            [CreateWorklog()],
            1,
            1,
            new DateOnly(2025, 1, 1),
            new DateOnly(2025, 1, 31),
            _token);

        result.Created.Should().Be(0);
        result.Updated.Should().Be(0);
        result.Skipped.Should().Be(1);
        result.Total.Should().Be(1);

        _timesheetComposerMock.Verify(
            x => x.Compose(
                It.IsAny<UpsertFactDto>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ComposeShouldIgnoreWorklogsWithZeroTimeSpent()
    {
        _dbContextMock
            .Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = 1,
                    ProjectId = 1,
                    UserOnProjectFrom = new DateOnly(2024, 1, 1)
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>().BuildMockDbSet().Object);

        var result = await _composer.Compose(
        [
            CreateWorklog(timeSpentSeconds: 0)
        ],
        1,
        1,
        new DateOnly(2025, 1, 1),
        new DateOnly(2025, 1, 31),
        _token);

        result.Created.Should().Be(0);
        result.Updated.Should().Be(0);
        result.Skipped.Should().Be(0);
        result.Total.Should().Be(0);

        _timesheetComposerMock.Verify(
            x => x.Compose(
                It.IsAny<UpsertFactDto>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ComposeShouldPassCorrectDataToTimesheetComposer()
    {
        _dbContextMock
            .Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = 10,
                    ProjectId = 20,
                    UserOnProjectFrom = new DateOnly(2024, 1, 1)
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>().BuildMockDbSet().Object);

        var date = new DateOnly(2025, 1, 5);

        await _composer.Compose(
            [CreateWorklog(date: date)],
            10,
            20,
            date,
            date,
            _token);

        _timesheetComposerMock.Verify(
            x => x.Compose(
                It.Is<UpsertFactDto>(dto =>
                    dto.UserId == 10 &&
                    dto.ProjectId == 20 &&
                    dto.ReportForDate == date &&
                    dto.TaskName == "[TEST-1] Task" &&
                    dto.FactTrackedInMinutes == 60 &&
                    dto.AllowAutomatic),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static JiraImportedWorklog CreateWorklog(
        string issueKey = "TEST-1",
        string issueSummary = "Task",
        DateOnly? date = null,
        int timeSpentSeconds = 3600)
    {
        return new JiraImportedWorklog(
            issueKey,
            issueSummary,
            date ?? new DateOnly(2025, 1, 1),
            timeSpentSeconds);
    }
}
