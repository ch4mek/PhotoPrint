using Microsoft.AspNetCore.OData.Query;
using MockQueryable;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetDashboardCurrentUser;

public class GetDashboardCurrentUserQueryHandlerTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
    { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ITimesheetQueryBuilder> _timesheetQueryBuilder = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IDashboardOdpLoader> _dashboardOdpLoaderMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetDashboardCurrentUserRequest _handler;

    public GetDashboardCurrentUserQueryHandlerTests()
    {
        _handler = new GetDashboardCurrentUserRequest(
            _authenticatedUserContextMock.Object,
            _clockServiceMock.Object,
            _timesheetQueryBuilder.Object,
            _dashboardOdpLoaderMock.Object);
    }

    [Fact]
    public async Task GetDashboardByUserWithValidDataShouldReturnGetDashboardCurrentUserResponse()
    {
        //Arrange
        _authenticatedUserContextMock.Setup(a => a.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(a => a.GetRequiredUserId()).Returns(1);
        _clockServiceMock.Setup(c => c.UtcNow).Returns(new DateTime(2023, 11, 5));

        var queryOptions = It.IsAny<ODataQueryOptions<DashboardActivity>>();

        var project = new TimeTrack.DataAccess.Npgsql.DatabaseEntities.Project
        {
            Id = 1,
            Name = "Project1",
            Color = "#ABC3E"
        };

        var pages = new PageableListOfItems<DashboardActivity>
        {
            Items = new List<DashboardActivity>()
            {
                new()
                {
                    ProjectName = project.Name,
                    ReportForDate = new DateOnly(2023, 11, 05),
                    TaskName = "First",
                    TrackedInMinutes = 180,
                    CreatedAtUtc = new DateTime(2023, 11, 05),
                    OrderByDateUtc = new DateTime(2023, 11, 05)
                },

                new()
                {
                    ProjectName = project.Name,
                    ReportForDate = new DateOnly(2023, 11, 02),
                    TaskName = "Second",
                    TrackedInMinutes = 120,
                    CreatedAtUtc = new DateTime(2023, 11, 02),
                    OrderByDateUtc = new DateTime(2023, 11, 02)
                },

                new()
                {
                    ProjectName = project.Name,
                    ReportForDate = new DateOnly(2023, 11, 03),
                    TaskName = "Third",
                    TrackedInMinutes = 240,
                    CreatedAtUtc = new DateTime(2023, 11, 03),
                    OrderByDateUtc = new DateTime(2023, 11, 03)
                }
            }
        };

        var list = new List<DashboardActivity>
        {
            new()
            {
                ProjectName = project.Name,
                ReportForDate = new DateOnly(2023, 11, 05),
                TaskName = "First",
                TrackedInMinutes = 180,
                CreatedAtUtc = new DateTime(2023, 11, 05),
                OrderByDateUtc = new DateTime(2023, 11, 05)
            },

            new()
            {
                ProjectName = project.Name,
                ReportForDate = new DateOnly(2023, 11, 02),
                TaskName = "Second",
                TrackedInMinutes = 120,
                CreatedAtUtc = new DateTime(2023, 11, 02),
                OrderByDateUtc = new DateTime(2023, 11, 02)
            },

            new()
            {
                ProjectName = project.Name,
                ReportForDate = new DateOnly(2023, 11, 03),
                TaskName = "Third",
                TrackedInMinutes = 240,
                CreatedAtUtc = new DateTime(2023, 11, 03),
                OrderByDateUtc = new DateTime(2023, 11, 03)
            }
        };

        _dashboardOdpLoaderMock.Setup(q => q.LoadAsync(1, new DateOnly(2023, 11, 5), queryOptions, _token))
            .ReturnsAsync(new TimeTrack.UseCases.Dashboard.Core.CoreDto.DashboardOdpCoreDto()
            {
                Pages = pages,
                AllItems = list
            });

        var timesheets = new List<TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet>();
        foreach (var item in pages.Items)
        {
            timesheets.Add(new TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet()
            {
                Project = project,
                ReportForDate = item.ReportForDate,
                TaskName = item.TaskName,
                FactTrackedInMinutes = item.TrackedInMinutes,
                CreatedAtUtc = item.CreatedAtUtc,
                OrderByDateUtc = item.OrderByDateUtc.Value,
            });
        }

        var queryableMock = timesheets.BuildMock();

        _timesheetQueryBuilder
            .Setup(q => q.FromAsNoTracked(It.IsAny<int?>(), It.IsAny<int[]?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithWeekOfYear(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.WithUserFilter(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilder.Object);

        _timesheetQueryBuilder
            .Setup(q => q.Build())

            .Returns(queryableMock);



        var expectedActivities = new List<DashboardActivity>
        {
            new ()
            {
                ProjectName = "Project1",
                ReportForDate = new DateOnly(2023, 11, 05),
                TaskName = "First",
                TrackedInMinutes = 180,
                CreatedAtUtc = new DateTime(2023, 11, 05),
                OrderByDateUtc = new DateTime(2023, 11, 05)
            },
            new ()
            {
                ProjectName = "Project1",
                ReportForDate = new DateOnly(2023, 11, 02),
                TaskName = "Second",
                TrackedInMinutes = 120,
                CreatedAtUtc = new DateTime(2023, 11, 02),
                OrderByDateUtc = new DateTime(2023, 11, 02)
            },
            new ()
            {
                ProjectName = "Project1",
                ReportForDate = new DateOnly(2023, 11, 03),
                TaskName = "Third",
                TrackedInMinutes = 240,
                CreatedAtUtc = new DateTime(2023, 11, 03),
                OrderByDateUtc = new DateTime(2023, 11, 03)
            }
        };

        //Act
        var result = await _handler.Handle(new GetDashboardCurrentUserRequest.Query(queryOptions), _token);

        //Assert
        result.Should().NotBeNull();
        result.Activities.Items.Should().BeEquivalentTo(expectedActivities);
        result.TrackedInMinutesDay.Should().Be(180);
        result.TrackedInMinutesWeek.Should().Be(540);
        result.TrackedInMinutesMonth.Should().Be(540);
    }
}
