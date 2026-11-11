using System.Linq.Expressions;
using Bogus;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetUsersWithTotalTrackedTimeRequestTests
{
    private readonly Mock<IUserAccessService> _userAccessServiceMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IGetTimesheetsByPeriodWithFilterDbQuery> _getTimesheetsDbQueryMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetUsersWithTotalTrackedTimeRequest _handler;

    private readonly Faker<Timesheet> _faker = new();

    public GetUsersWithTotalTrackedTimeRequestTests()
    {
        _handler = new GetUsersWithTotalTrackedTimeRequest(
            _userAccessServiceMock.Object,
            _authenticatedUserMock.Object,
            _timeTrackDbContextMock.Object
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
    public async Task HandleWhenEmployeeTriesToGetOtherUserDataShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetUsersWithTotalTrackedTimeRequest.Query(
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ResourceGroupId: null,
            ClientId: null,
            ProjectId: null,
            UserId: 1,
            TrackedTimeType.Fact);

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId.Value, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>(PortalMessages.CannotAccessOtherUserData);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnUsersWithTotalTrackedTime()
    {
        // Arrange
        var query = new GetUsersWithTotalTrackedTimeRequest.Query(
            Year: 2023,
            MonthOfYear: [5],
            WeekOfYear: null,
            DepartmentId: null,
            ResourceGroupId: null,
            ClientId: null,
            ProjectId: null,
            UserId: null,
            TrackedTimeType.Fact);

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

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                Id = 1,
                ProjectId = 1,
                FactTrackedInMinutes = 280,
                UserId = 1,
                ReportYear = 2023,
                ReportMonthOfYear = 5,
                TaskName = "Test 1"
            },
            new ()
            {
                Id = 2,
                ProjectId = 2,
                FactTrackedInMinutes = 180,
                UserId = 2,
                ReportYear = 2023,
                ReportMonthOfYear = 5,
                TaskName = "Test 1"
            },
            new ()
            {
                Id = 3,
                ProjectId = 2,
                FactTrackedInMinutes = 250,
                UserId = 1,
                ReportYear = 2023,
                ReportMonthOfYear = 5,
                TaskName = "Test 1"
            },
            new ()
            {
                Id = 4,
                ProjectId = 2,
                FactTrackedInMinutes = 100,
                UserId = 1,
                ReportYear = 2023,
                ReportMonthOfYear = 5,
                TaskName = "Test 1"
            },
        };

        var users = new List<TimeTrackUser>
        {
            new()
            {
                UserId = 1,
                Email = "test1@test.com",
                FirstName = "FFF",
                LastName = "GGG",
                IsActive = false
            },
            new()
            {
                UserId = 2,
                Email = "test2@test.com",
                FirstName = "AAA",
                LastName = "BBB",
                IsActive = true
            }
        };

        var expectedResult = new List<UserWithTrackedTimeReportDto>
        {
            new()
            {
                FirstName = users[1].FirstName,
                LastName = users[1].LastName,
                TotalTimeInMinutes = 180,
                IsActive = true
            },
            new()
            {
                FirstName = users[0].FirstName,
                LastName = users[0].LastName,
                TotalTimeInMinutes = 630,
                IsActive = false
            }
        };

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetsDbQueryMock.Setup(x =>
                x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheets);

        _timeTrackDbContextMock
            .Setup(x => x.Timesheets)
            .Returns(timesheetsData.BuildMockDbSet().Object);
        _timeTrackDbContextMock
            .Setup(x => x.TimeTrackUsers)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithoutStrictOrdering());
    }
}
