using MockQueryable.Moq;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Project = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Project;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;
using TimeTrackUser = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.TimeTrackUser;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.DbQueries;

public class GetTimesheetByIdDbQueryTests
{
    [Fact]
    public async Task GetByIdAsyncWhenEntityIsNullReturnNull()
    {
        // Arrange
        var dbContextMock = new Mock<ITimeTrackDbContext>();

        dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(Array.Empty<Timesheet>().BuildMockDbSet().Object);

        var query = new GetTimesheetDbQuery(dbContextMock.Object);

        // Act
        var timesheet = await query.ExecuteAsync(timesheetId: 1, CancellationToken.None);

        // Assert
        timesheet.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsyncWhenEntityExistsReturnValidDomainObject()
    {
        //Arrange
        var dbContextMock = new Mock<ITimeTrackDbContext>();

        var entity = new Timesheet
        {
            Id = 1,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ReportDayOfYear = DateTime.UtcNow.DayOfYear,
            ReportMonthOfYear = DateTime.UtcNow.Month,
            ReportWeekOfYear = 10,
            ReportYear = DateTime.UtcNow.Year,
            ProjectId = 1,
            Project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Color = "#000000",
                IsPublic = false,
                IsBillable = false,
                IsActive = false,
                ClientId = 0,
                CreatedById = 0,
                ProjectUsers = null
            },
            TaskName = "Test Task",
            FactTrackedInMinutes = 60,
            UserId = 1,
            User = new TimeTrackUser
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test",
                Color = "#000000"
            }
        };

        var expectedDomainObject = new Timesheet()
        {
            Id = entity.Id,

            ReportForDate = entity.ReportForDate,
            ReportDayOfYear = entity.ReportDayOfYear,
            ReportMonthOfYear = entity.ReportMonthOfYear,
            ReportWeekOfYear = entity.ReportWeekOfYear,
            ReportYear = entity.ReportYear,

            ProjectId = entity.ProjectId,
            Project = new Project
            {
                Id = 1,
                Name = entity.Project.Name,
                Color = entity.Project.Color!,
                ProjectUsers = null,
                CreatedById = 0
            },

            TaskName = entity.TaskName,
            FactTrackedInMinutes = entity.FactTrackedInMinutes,
            BillableTrackedInMinutes = entity.BillableTrackedInMinutes,

            UserId = entity.UserId,
            User = new TimeTrackUser
            {
                UserId = 1,
                FirstName = entity.User.FirstName,
                Email = "test",
                LastName = entity.User.LastName,
                Color = entity.User.Color!
            }
        };

        dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(Enumerable.Repeat(entity, 1).ToArray().BuildMockDbSet().Object);

        var query = new GetTimesheetDbQuery(dbContextMock.Object);

        // Act
        var timesheet = await query.ExecuteAsync(entity.Id, CancellationToken.None);

        // Assert
        timesheet.Should().BeEquivalentTo(expectedDomainObject);
    }
}


