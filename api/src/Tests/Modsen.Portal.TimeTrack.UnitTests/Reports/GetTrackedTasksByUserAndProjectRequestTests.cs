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
using TimeTrackUser = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.TimeTrackUser;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTasksByUserAndProjectRequestTests
{
    private readonly Mock<IUserAccessService> _userAccessServiceMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetTimesheetsByPeriodWithFilterDbQuery> _getTimesheetsDbQueryMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Faker<Timesheet> _faker = new();

    private readonly GetTrackedTasksByUserAndProjectRequest _useCase;

    public GetTrackedTasksByUserAndProjectRequestTests()
    {
        _useCase = new GetTrackedTasksByUserAndProjectRequest(
            _userAccessServiceMock.Object,
            _timeTrackDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _getTimesheetsDbQueryMock.Object
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
    public async Task HandleWhenEmployeeTriesToGetOtherUserTasksShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetTrackedTasksByUserAndProjectRequest.Query(
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            UserId: 1,
            ProjectId: 1,
            DepartmentId: null,
            ClientId: null,
            TrackedTimeType.Fact);

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = async () => await _useCase.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You can't get other user's information");
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var query = new GetTrackedTasksByUserAndProjectRequest.Query(
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            UserId: 1,
            ProjectId: 1,
            DepartmentId: null,
            ClientId: null,
            TrackedTimeType: TrackedTimeType.Fact);

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId, _cancellationToken))
            .ReturnsAsync(true);

        _timeTrackDbContextMock
            .Setup(x => x.TimeTrackUsers)
            .Returns(new List<TimeTrackUser>().BuildMockDbSet().Object);

        // Act
        var action = async () => await _useCase.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenValidRequestShouldReturnTrackedTasks()
    {
        // Arrange
        var query = new GetTrackedTasksByUserAndProjectRequest.Query(
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            UserId: 1,
            ProjectId: 1,
            DepartmentId: null,
            ClientId: null,
            TrackedTimeType: TrackedTimeType.Fact);

        var timesheetsData = _faker.Generate(count: 3);

        var user = new TimeTrackUser
        {
            UserId = 1,
            FirstName = "Vasya",
            LastName = "Pupkin",
            Email = "test@modsen-software.com",
            DepartmentId = "Js",
            Color = "#C7F4A0"
        };

        var expectedResult = timesheetsData.ConvertAll(x => new TrackedTaskReportDto
        {
            TaskName = x.TaskName,
            ReportForDate = x.ReportForDate,
            TrackedTimeInMinutes = x.FactTrackedInMinutes,
            Color = user.Color
        });

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId, _cancellationToken))
            .ReturnsAsync(true);

        _timeTrackDbContextMock
            .Setup(x => x.TimeTrackUsers)
            .Returns(new List<TimeTrackUser> { user }.BuildMockDbSet().Object);

        Expression<Func<GetTimesheetsByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear.Length == query.MonthOfYear.Length
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetsDbQueryMock
            .Setup(x => x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        // Act
        var result = await _useCase.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithStrictOrdering());
    }
}
