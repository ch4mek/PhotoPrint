using MockQueryable;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetDashboardCurrentUserAllTrackedHours;

public class GetDashboardCurrentUserAllTrackedHoursQueryHandlerTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
    { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<ITimesheetQueryBuilder> _timesheetQueryBuilderMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetDashboardCurrentUserAllTrackedHoursRequest _handler;

    public GetDashboardCurrentUserAllTrackedHoursQueryHandlerTests()
    {
        _handler = new GetDashboardCurrentUserAllTrackedHoursRequest(
            _authenticatedUserContextMock.Object,
            _clockServiceMock.Object,
            _timesheetQueryBuilderMock.Object);
    }

    [Fact]
    public async Task GetDashboardByUserWithValidDataShouldReturnGetDashboardCurrentUserResponse()
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(a => a.GetRequiredUserId()).Returns(1);
        _clockServiceMock.Setup(c => c.UtcNow).Returns(new DateTime(2023, 11, 5));

        var list = new List<Timesheet>
        {
            new()
            {
                ReportForDate = new DateOnly(2023, 11, 05),
                FactTrackedInMinutes =  180,
                TaskName = "Test 1"
            },
            new()
            {
                ReportForDate = new DateOnly(2023, 11, 02),
                FactTrackedInMinutes =  120,
                TaskName = "Test 1"
            },
            new()
            {
                ReportForDate = new DateOnly(2023, 11, 03),
                FactTrackedInMinutes =  240,
                TaskName = "Test 1"
            }
        };

        var queryableMock = list.BuildMock();

        _timesheetQueryBuilderMock
            .Setup(q => q.FromAsNoTracked(2023, new[] { 11 }))
            .Returns(_timesheetQueryBuilderMock.Object);

        _timesheetQueryBuilderMock
            .Setup(q => q.WithUserFilter(1))
            .Returns(_timesheetQueryBuilderMock.Object);

        _timesheetQueryBuilderMock
            .Setup(q => q.Build())
            .Returns(queryableMock);

        //Act
        var result = await _handler.Handle(new GetDashboardCurrentUserAllTrackedHoursRequest.Query(), _token);

        //Assert
        result.Should().NotBeNull();
    }
}
