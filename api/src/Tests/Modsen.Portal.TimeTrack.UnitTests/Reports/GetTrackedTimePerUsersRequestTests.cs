using System.Linq.Expressions;
using Bogus;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTimePerUsersRequestTests
{
    private readonly Mock<IUserAccessService> _userAccessServiceMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IGetTimesheetsByPeriodWithFilterDbQuery> _getTimesheetDbQueryMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTrackedTimePerUsersRequest _handler;

    private readonly Faker<Timesheet> _faker = new();

    public GetTrackedTimePerUsersRequestTests()
    {
        _handler = new GetTrackedTimePerUsersRequest(
            _userAccessServiceMock.Object,
            _authenticatedUserMock.Object,
            _timeTrackDbContextMock.Object,
            _getTimesheetDbQueryMock.Object
        );

        _faker.CustomInstantiator(f => new Timesheet
        {
            TaskName = f.Lorem.Sentence(),
            ReportForDate = new DateOnly(2023, 06, 04),
            ReportYear = 2023,
            ReportMonthOfYear = 6,
            ReportWeekOfYear = 25,
            ReportDayOfYear = 183,
            FactTrackedInMinutes = 180,
            BillableTrackedInMinutes = 180,
            UserId = 1,
            ProjectId = 1,
        });
    }

    [Fact]
    public async Task HandleWhenEmployeeTriesToGetOtherDataShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetTrackedTimePerUsersRequest.Query
        {
            Page = 1,
            PageSize = 50,
            UserId = 1,
            Year = 2023
        };

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId.Value, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>(PortalMessages.CannotAccessOtherUserData);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnUsersTrackedTime()
    {
        // Arrange
        var query = new GetTrackedTimePerUsersRequest.Query
        {
            Page = 1,
            PageSize = 50,
            Year = 2023,
            MonthOfYear = [5],
            TrackedTimeType = TrackedTimeType.Fact
        };

        // var timesheets = new List<Timesheet>
        // {
        //     new("task1", new DateTime(2023, 05, 01, 0, 0, 0, DateTimeKind.Utc), 280, 1, 1),
        //     new("task2", new DateTime(2023, 05, 02, 0, 0, 0, DateTimeKind.Utc), 180, 2, 2),
        //     new("task3", new DateTime(2023, 05, 03, 0, 0, 0, DateTimeKind.Utc), 250, 1, 2),
        //     new("task4", new DateTime(2023, 05, 03, 0, 0, 0, DateTimeKind.Utc), 100, 1, 2)
        // };

        var timesheets = new List<Timesheet>
        {
            _faker.Clone()
                .RuleFor(x => x.FactTrackedInMinutes, 280)
                .RuleFor(x => x.UserId, 1)
                .RuleFor(x => x.ProjectId, 1)
                .Generate(),
            _faker.Clone()
                .RuleFor(x => x.FactTrackedInMinutes, 180)
                .RuleFor(x => x.UserId, 2)
                .RuleFor(x => x.ProjectId, 2)
                .Generate(),
            _faker.Clone()
                .RuleFor(x => x.FactTrackedInMinutes, 250)
                .RuleFor(x => x.UserId, 1)
                .RuleFor(x => x.ProjectId, 2)
                .Generate(),
            _faker.Clone()
                .RuleFor(x => x.FactTrackedInMinutes, 100)
                .RuleFor(x => x.UserId, 1)
                .RuleFor(x => x.ProjectId, 2)
                .Generate()
        };

        var pageableUsers = new PageableListOfItems<TimeTrackUser>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Total = 2,
            Items = new List<TimeTrackUser>
            {
                new()
                {
                    UserId = 1,
                    Email = "test1@test.com",
                    FirstName = "Test1",
                    LastName = "Test1",
                    IsActive = false
                },
                new()
                {
                    UserId = 2,
                    Email = "test2@test.com",
                    FirstName = "Test2",
                    LastName = "Test2",
                    IsActive = true
                }
            }
        };

        var users = new List<TimeTrackUser>
        {
            new()
            {
                UserId = 1,
                Email = "test1@test.com",
                FirstName = "Test1",
                LastName = "Test1",
                IsActive = false
            },
            new()
            {
                UserId = 2,
                Email = "test2@test.com",
                FirstName = "Test2",
                LastName = "Test2",
                IsActive = true
            }
        };

        var expectedResult = new PageableListOfItems<TrackedTimePerUserReportDto>
        {
            Page = pageableUsers.Page,
            PageSize = pageableUsers.PageSize,
            Total = pageableUsers.Total,
            Items = new List<TrackedTimePerUserReportDto>
            {
                new()
                {
                    UserId = 1,
                    FirstName = "Test1",
                    LastName = "Test1",
                    ProjectsCount = 2,
                    TotalTimeInMinutes = 630,
                    IsActive = false
                },
                new()
                {
                    UserId = 2,
                    FirstName = "Test2",
                    LastName = "Test2",
                    ProjectsCount = 1,
                    TotalTimeInMinutes = 180,
                    IsActive = true
                }
            }
        };

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheets);

        //_userProviderMock
        //    .Setup(x => x.GetPageableUsersForReportsAsync(query.Page, query.PageSize, It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<FilterParams>(), _cancellationToken))
        //    .ReturnsAsync(pageableUsers);

        _timeTrackDbContextMock
            .Setup(x => x.TimeTrackUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult);
    }
}
