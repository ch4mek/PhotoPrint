using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetDashboardTasks;

public class GetDashboardTasksQueryHandlerTests
{
    private readonly Mock<IGetAccessedDepartmentIdForCurrentUserBusinessRule> _getAccessedDepartmentMock = new();
    private readonly Mock<ITimesheetQueryBuilder> _timesheetQueryBuilder = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetDashboardTasksRequest _handler;

    public GetDashboardTasksQueryHandlerTests()
    {
        _handler = new GetDashboardTasksRequest(
            _getAccessedDepartmentMock.Object,
            _authenticatedUserContextMock.Object,
            _timesheetQueryBuilder.Object);
    }

    [Fact]
    public async Task GetDashboardUsersWithValidRequestReturnsDashboardManagerPagingUserViewModel()
    {
        // Arrange
        _getAccessedDepartmentMock.Setup(c => c.Execute("Net", _token))
            .ReturnsAsync("Net");

        var monthOfYear = new[] { 11, 12 };

        var user = new TimeTrackUser
        {
            UserId = 1,
            Color = "#ABC3E",
            Email = "test",
            FirstName = "test",
            LastName = "test"
        };
        var data = new List<Timesheet>()
        {
            new()
            {
                FactTrackedInMinutes = 360,
                ReportForDate = new DateOnly(2023, 11, 22),
                User = user,
                TaskName = "First"
            },
            new()
            {
                FactTrackedInMinutes = 360,
                ReportForDate = new DateOnly(2023, 11, 22),
                User = user,
                TaskName = "Second"
            }
        }.BuildMockDbSet();

        _timesheetQueryBuilder
            .Setup(q => q.FromAsNoTracked(It.IsAny<int?>(), It.IsAny<int[]?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithDepartmentFilter(It.IsAny<string?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithResourceGroupFilter(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithUserFilter(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithUserFilter(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithProjectFilter(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithBillableHoursFilter(It.IsAny<bool?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.Build())
            .Returns(data.Object);

        var expectedResult = new List<DashboardTasksCoreDto>
        {
            new ()
            {
                TrackedInMinutes = 360,
                ReportForDate = new DateOnly(2023,11,22),
                Color = "#ABC3E",
                TaskName = "First"
            },
            new ()
            {
                TrackedInMinutes = 360,
                ReportForDate = new DateOnly(2023,11,22),
                Color = "#ABC3E",
                TaskName = "Second"
            }
        };

        // Act
        var result = await _handler.Handle(new GetDashboardTasksRequest.Query(1, 1, 2023, monthOfYear, "Net"), _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
