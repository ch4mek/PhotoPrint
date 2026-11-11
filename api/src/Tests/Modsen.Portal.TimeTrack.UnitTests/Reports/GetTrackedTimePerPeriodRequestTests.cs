using System.Linq.Expressions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTimePerPeriodRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authUserMock = new();
    private readonly Mock<IUserAccessService> _userAccessServiceMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IGetTimesheetsByPeriodWithFilterDbQuery> _getTimesheetDbQueryMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTrackedTimePerPeriodRequest _handler;

    public GetTrackedTimePerPeriodRequestTests()
    {
        _handler = new GetTrackedTimePerPeriodRequest(
            _authUserMock.Object,
            _userAccessServiceMock.Object,
            _timeTrackDbContextMock.Object,
            _getTimesheetDbQueryMock.Object
        );
    }

    [Fact]
    public async Task HandleWhenEmployeeTriesToGetOtherUserDataShouldThrowInvalidDataAppException()
    {
        // Arrange
        _authUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authUserMock.Setup(x => x.UserId).Returns(10);

        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            Year = 2023,
            UserId = 1
        };

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId.Value, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = async () => await _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You can't get other user's reports");
    }

    [Fact]
    public async Task HandleWhenOnlyTrackedTimeSpecifiedShouldReturnTrackedTimeGroupedByYear()
    {
        // Arrange
        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            TrackedTimeType = TrackedTimeType.Fact
        };

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                FactTrackedInMinutes = 300,
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    Name = "Project 1",
                    Color = "#FF0000"
                },
                ReportMonthOfYear = 5,
                ReportForDate = new DateOnly(2023, 06, 04),
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            },
            new ()
            {
                FactTrackedInMinutes = 300,
                ProjectId = 2,
                Project = new Project
                {
                    Id = 2,
                    Name = "Project 2",
                    Color = "#00FF00"
                },
                ReportMonthOfYear = 6,
                ReportForDate = new DateOnly(2023, 06, 04),
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            },
            new ()
            {
                FactTrackedInMinutes = 300,
                ProjectId = 3,
                Project = new Project
                {
                    Id = 3,
                    Name = "Project 3",
                    Color = "#0000FF"
                },
                ReportMonthOfYear = 7,
                ReportForDate = new DateOnly(2023, 06, 04),
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            }
        };

        var expectedResult = new TrackedTimePerPeriodReportDto
        {
            TotalTimeInMinutes = 900,
            TimePerPeriod =
            [
                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 900,
                    Projects =
                    [
                        new BarChartInfoResponseDto
                        {
                            ProjectId = 1,
                            ProjectName = "Project 1",
                            ProjectColor = "#FF0000",
                            TotalTime = 300,
                            IsBillableProject = true
                        },

                        new BarChartInfoResponseDto
                        {
                            ProjectId = 2,
                            ProjectName = "Project 2",
                            ProjectColor = "#00FF00",
                            TotalTime = 300,
                            IsBillableProject = true
                        },

                        new BarChartInfoResponseDto
                        {
                            ProjectId = 3,
                            ProjectName = "Project 3",
                            ProjectColor = "#0000FF",
                            TotalTime = 300,
                            IsBillableProject = false
                        }
                    ]
                }
            ],
            Projects = [1, 2, 3]
        };

        _authUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authUserMock.Setup(x => x.UserId).Returns(10);

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                IsBillable = true,
                Name = "test"
            },
            new()
            {
                Id = 2,
                IsBillable = true,
                Name = "test"
            }
        };

        _timeTrackDbContextMock
            .Setup(x => x.Projects)
            .Returns(projects.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenYearSpecifiedShouldReturnTrackedTimeGroupedByMonthInSpecifiedYear()
    {
        // Arrange
        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            Year = 2023,
            TrackedTimeType = TrackedTimeType.Fact
        };

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                FactTrackedInMinutes = 640,
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    Name = "Project 1",
                    Color = "#FF0000"
                },
                ReportMonthOfYear = 5,
                TaskName = "Test 1"
            },
            new ()
            {
                FactTrackedInMinutes = 380,
                ProjectId = 2,
                Project = new Project
                {
                    Id = 2,
                    Name = "Project 2",
                    Color = "#00FF00"
                },
                ReportMonthOfYear = 6,
                TaskName = "Test 1"
            },
            new ()
            {
                FactTrackedInMinutes = 250,
                ProjectId = 3,
                Project = new Project
                {
                    Id = 3,
                    Name = "Project 3",
                    Color = "#0000FF"
                },
                ReportMonthOfYear = 7,
                TaskName = "Test 1"
            }
        };

        var expectedResult = new TrackedTimePerPeriodReportDto
        {
            TotalTimeInMinutes = 1270,
            TimePerPeriod =
            [
                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 640,
                    Projects =
                    [
                        new BarChartInfoResponseDto
                        {
                            ProjectId = 1,
                            ProjectName = "Project 1",
                            ProjectColor = "#FF0000",
                            TotalTime = 640,
                            IsBillableProject = true
                        }
                    ]
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 380,
                    Projects =
                    [
                        new BarChartInfoResponseDto
                        {
                            ProjectId = 2,
                            ProjectName = "Project 2",
                            ProjectColor = "#00FF00",
                            TotalTime = 380,
                            IsBillableProject = true
                        }
                    ]
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 250,
                    Projects =
                    [
                        new BarChartInfoResponseDto
                        {
                            ProjectId = 3,
                            ProjectName = "Project 3",
                            ProjectColor = "#0000FF",
                            TotalTime = 250,
                            IsBillableProject = false
                        }
                    ]
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                },

                new TimePerPeriodReportDto
                {
                    Period = new DateTime(2023, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = 0,
                    Projects = []
                }
            ],
            Projects = [1, 2, 3]
        };

        _authUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authUserMock.Setup(x => x.UserId).Returns(10);

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                IsBillable = true,
                Name = "test"
            },
            new()
            {
                Id = 2,
                IsBillable = true,
                Name = "test"
            }
        };

        _timeTrackDbContextMock
            .Setup(x => x.Projects)
            .Returns(projects.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenYearAndOneMonthSpecifiedShouldReturnTrackedTimeGroupedByDaysInSpecifiedMonth()
    {
        // Arrange
        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            Year = 2023,
            MonthOfYear = [6],
            TrackedTimeType = TrackedTimeType.Fact
        };

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                ProjectId = 2,
                ReportForDate = DateOnly.FromDateTime(new DateTime(2023, 6, 15)),
                Project = new Project
                {
                    Id = 2,
                    Name = "Project 2",
                    Color = "#FF0000"
                },
                FactTrackedInMinutes = 640,
                ReportMonthOfYear = 6,
                ReportYear = 2023,
                UserId = 1,
                TaskName = "Test 1"
            },
            new ()
            {
                ProjectId = 4,
                ReportForDate = DateOnly.FromDateTime(new DateTime(2023, 6, 16)),
                Project = new Project
                {
                    Id = 4,
                    Name = "Project 4",
                    Color = "#FF0000"
                },
                FactTrackedInMinutes = 380,
                ReportMonthOfYear = 6,
                ReportYear = 2023,
                UserId = 1,
                TaskName = "Test 1"
            }
        };

        var periods = Enumerable.Range(1, DateTime.DaysInMonth(2023, 6))
            .Select(day => new TimePerPeriodReportDto
            {
                Period = new DateTime(2023, 6, day, 0, 0, 0, DateTimeKind.Utc),
                TotalTimeInMinutes = timesheetsData
                    .Where(t => t.ReportForDate.Day == day)
                    .Sum(t => t.FactTrackedInMinutes),
                Projects = timesheetsData
                    .Where(t => t.ReportForDate.Day == day)
                    .Select(t => new BarChartInfoResponseDto
                    {
                        ProjectId = t.ProjectId,
                        ProjectName = t.Project.Name,
                        ProjectColor = t.Project.Color,
                        TotalTime = t.FactTrackedInMinutes,
                        IsBillableProject = t.ProjectId == 2
                    })
                    .ToList()
            })
            .ToList();

        var expectedResult = new TrackedTimePerPeriodReportDto
        {
            TotalTimeInMinutes = 1020,
            TimePerPeriod = periods,
            Projects = [2, 4]
        };

        _authUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authUserMock.Setup(x => x.UserId).Returns(10);

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear.SequenceEqual(query.MonthOfYear);

        _getTimesheetDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        var projects = new List<Project>
        {
            new()
            {
                Id = 2,
                IsBillable = true,
                Name = "test"
            }
        };

        _timeTrackDbContextMock
            .Setup(x => x.Projects)
            .Returns(projects.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenYearAndMoreThanOneMonthSpecifiedShouldReturnTrackedTimeGroupedBySpecifiedMonths()
    {
        // Arrange
        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            Year = 2023,
            MonthOfYear = [6, 7],
            TrackedTimeType = TrackedTimeType.Fact
        };

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                ProjectId = 2,
                ReportForDate = DateOnly.FromDateTime(new DateTime(2023, 6, 15)),
                Project = new Project
                {
                    Id = 2,
                    Name = "Project 2",
                    Color = "#FF0000"
                },
                FactTrackedInMinutes = 380,
                ReportMonthOfYear = 6,
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            },
            new ()
            {
                ProjectId = 4,
                ReportForDate = DateOnly.FromDateTime(new DateTime(2023, 6, 15)),
                Project = new Project
                {
                    Id = 4,
                    Name = "Project 4",
                    Color = "#00FF00"
                },
                FactTrackedInMinutes = 360,
                ReportMonthOfYear = 7,
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            }
        };

        var periods = new List<TimePerPeriodReportDto>
        {
            new()
            {
                Period = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                TotalTimeInMinutes = 380,
                Projects =
                [
                    new BarChartInfoResponseDto
                    {
                        ProjectId = 2,
                        ProjectName = "Project 2",
                        ProjectColor = "#FF0000",
                        TotalTime = 380,
                        IsBillableProject = true
                    }
                ]
            },
            new()
            {
                Period = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                TotalTimeInMinutes = 360,
                Projects =
                [
                    new BarChartInfoResponseDto
                    {
                        ProjectId = 4,
                        ProjectName = "Project 4",
                        ProjectColor = "#00FF00",
                        TotalTime = 360,
                        IsBillableProject = false
                    }
                ]
            }
        };

        var expectedResult = new TrackedTimePerPeriodReportDto
        {
            TotalTimeInMinutes = 740,
            TimePerPeriod = periods,
            Projects = [2, 4]
        };

        _authUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authUserMock.Setup(x => x.UserId).Returns(10);

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear.SequenceEqual(query.MonthOfYear);

        _getTimesheetDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        var projects = new List<Project>
        {
            new()
            {
                Id = 2,
                IsBillable = true,
                Name = "test"
            }
        };

        _timeTrackDbContextMock
            .Setup(x => x.Projects)
            .Returns(projects.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenWeekSpecifiedShouldReturnTrackedTimeGroupedByDaysOfWeek()
    {
        // Arrange
        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            Year = 2023,
            WeekOfYear = 19,
            TrackedTimeType = TrackedTimeType.Fact
        };

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                ProjectId = 2,
                ReportForDate = DateOnly.FromDateTime(new DateTime(2023, 5, 11)),
                Project = new Project
                {
                    Id = 2,
                    Name = "Project 2",
                    Color = "#FF0000"
                },
                FactTrackedInMinutes = 180,
                ReportMonthOfYear = 6,
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            },
            new ()
            {
                ProjectId = 5,
                ReportForDate = DateOnly.FromDateTime(new DateTime(2023, 5, 12)),
                Project = new Project
                {
                    Id = 5,
                    Name = "Project 5",
                    Color = "#FF0000"
                },
                FactTrackedInMinutes = 250,
                ReportMonthOfYear = 7,
                ReportYear = 2023,
                ReportWeekOfYear = 25,
                ReportDayOfYear = 183,
                BillableTrackedInMinutes = 180,
                UserId = 1,
                TaskName = "Test 1"
            }
        };

        var startOfWeek = new DateOnly(2023, 5, 8);

        var expectedResult = new TrackedTimePerPeriodReportDto
        {
            TotalTimeInMinutes = 430,
            TimePerPeriod = Enumerable.Range(0, 7)
                .Select(dayOffset => startOfWeek.AddDays(dayOffset))
                .Select(date => new TimePerPeriodReportDto
                {
                    Period = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc),
                    TotalTimeInMinutes = timesheetsData
                        .Where(t => t.ReportForDate == date)
                        .Sum(t => t.FactTrackedInMinutes),
                    Projects = timesheetsData
                        .Where(t => t.ReportForDate == date)
                        .Select(t => new BarChartInfoResponseDto
                        {
                            ProjectId = t.ProjectId,
                            ProjectName = t.Project.Name,
                            ProjectColor = t.Project.Color,
                            TotalTime = t.FactTrackedInMinutes,
                            IsBillableProject = t.ProjectId == 2
                        })
                        .ToList()
                })
                .ToList(),
            Projects = [2, 5]
        };

        _authUserMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        _authUserMock.Setup(x => x.UserId).Returns(10);

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        var projects = new List<Project>
        {
            new()
            {
                Id = 2,
                IsBillable = true,
                Name = "test"
            }
        };

        _timeTrackDbContextMock
            .Setup(x => x.Projects)
            .Returns(projects.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }
}
