using System.Globalization;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using Project = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Project;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;
using TimeTrackUser = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.TimeTrackUser;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.DeleteTimesheet;

public class DeleteTimesheetRequestTests
{
    private readonly Mock<IGetProjectUserBillableStatusDbQuery> _getProjectUserBillableStatusDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IDeleteTimesheetValidator> _validator = new();
    private readonly Mock<IGetTimesheetDbQuery> _getTimesheetDbQueryMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly DeleteTimesheetRequest _handler;
    private readonly CancellationToken _token;

    public DeleteTimesheetRequestTests()
    {
        _handler = new DeleteTimesheetRequest(_getProjectUserBillableStatusDbQueryMock.Object,
            _authenticatedUserContextMock.Object,
            _validator.Object,
            _getTimesheetDbQueryMock.Object,
            _dbContextMock.Object);

        _token = CancellationToken.None;
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task DeleteTimesheetWhenTimesheetDoesNotExistReturnEntityNotFoundException(string userRole)
    {
        // Arrange
        var timesheetCommand = new DeleteTimesheetRequest.Command(Id: 1, true);

        _getTimesheetDbQueryMock
            .Setup(query => query.ExecuteAsync(timesheetCommand.Id, _token))
            .ReturnsAsync(default(Timesheet));

        // Act
        var result = () => _handler.Handle(timesheetCommand, _token);

        //Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.TimesheetNotFound);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task DeleteTimesheetWhenUserWithCorrectDataReturnDeleteTimesheetResponse(string userRole)
    {
        // Arrange
        var timesheetCommand = new DeleteTimesheetRequest.Command(Id: 1, true);

        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var timesheet = new Timesheet
        {
            Id = 1,
            TaskName = "TaskName",
            FactTrackedInMinutes = 777,
            BillableTrackedInMinutes = 0,
            ReportForDate = utcNow,
            ReportYear = utcNow.Year,
            ReportMonthOfYear = utcNow.Month,
            ReportWeekOfYear = utcNow.DayOfYear,
            ReportDayOfYear = utcNow.DayOfYear,
            UserId = 1,
            User = new TimeTrackUser
            {
                Color = "#ED1AE6",
                FirstName = "Test",
                Email = "test",
                LastName = "Tester"
            },
            Project = new Project
            {
                Name = "ProjectName",
                Color = "#ED1A67"
            },
            ProjectId = 1
        };

        var timesheetDto = new TimesheetDto
        {
            Id = 1,
            UserId = 1,
            UserName = "Test Tester",
            ProjectId = 1,
            ProjectName = "ProjectName",
            TaskName = "TaskName",
            Color = "#ED1AE6",
            FactTrackedInMinutes = 777,
            ReportForDate = utcNow
        };

        _authenticatedUserContextMock
            .Setup(
                context => context.HasClaimWithValue(
                    PortalClaimTypeNames.TrackBillableWorkingHoursOnProject,
                    timesheet.ProjectId.ToString(CultureInfo.InvariantCulture)))
            .Returns(true);

        _validator.Setup(validator => validator.ValidateAsync(timesheet, _token));

        _getTimesheetDbQueryMock
            .Setup(query => query.ExecuteAsync(timesheetCommand.Id, _token))
            .ReturnsAsync(timesheet);

        _getProjectUserBillableStatusDbQueryMock
            .Setup(query => query.ExecuteAsync(timesheet.ProjectId, timesheet.UserId, timesheet.ReportForDate, _token))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(timesheetCommand, _token);

        //Assert
        result.Should().BeEquivalentTo(timesheetDto);
    }
}
