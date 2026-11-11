using System.Linq.Expressions;
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

public class GetTrackedTimePerProjectsRequestTests
{
    private readonly Mock<IUserAccessService> _userAccessServiceMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery> _getTimesheetDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTrackedTimePerProjectsRequest _handler;

    public GetTrackedTimePerProjectsRequestTests()
    {
        _handler = new GetTrackedTimePerProjectsRequest(
            _userAccessServiceMock.Object,
            _timeTrackDbContextMock.Object,
            _getTimesheetDbQueryMock.Object,
            _authenticatedUserContextMock.Object
        );
    }

    [Fact]
    public async Task HandleWhenEmployeeTriesToGetOtherDataShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetTrackedTimePerProjectsRequest.Query(
            UserId: 1,
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: null,
            ProjectId: null,
            TrackedTimeType.Fact);

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = async () => await _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>(PortalMessages.CannotAccessOtherUserData);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var query = new GetTrackedTimePerProjectsRequest.Query(
            UserId: 1,
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: null,
            ProjectId: null,
            TrackedTimeType.Fact);

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId, _cancellationToken))
            .ReturnsAsync(true);

        _timeTrackDbContextMock
            .Setup(x => x.TimeTrackUsers)
            .Returns(new List<TimeTrack.DataAccess.Npgsql.DatabaseEntities.TimeTrackUser>().BuildMockDbSet().Object);

        // Act
        var action = async () => await _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenValidRequestShouldReturnTrackedTimePerProjects()
    {
        // Arrange
        var query = new GetTrackedTimePerProjectsRequest.Query(
            UserId: 1,
            Year: 2023,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: null,
            ProjectId: null,
            TrackedTimeType.Fact);

        var user = new TimeTrack.DataAccess.Npgsql.DatabaseEntities.TimeTrackUser
        {
            UserId = 1,
            FirstName = "Vasya",
            LastName = "Pupkin",
            Email = "test@modsen-software.com",
            DepartmentId = "Js",
            Color = "#C7F4A0"
        };

        var timesheetsData = new List<Timesheet>
        {
            new ()
            {
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    Name = "TestProject1",
                    Client = new Client
                    {
                        Name = "TestClient1"
                    }
                },
                FactTrackedInMinutes = 180,
                TaskName = "Test 1"
            },
            new ()
            {
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    Name = "TestProject1",
                    Client = new Client
                    {
                        Name = "TestClient1"
                    }
                },
                FactTrackedInMinutes = 280,
                TaskName = "Test 1"
            },
            new ()
            {
                ProjectId = 2,
                Project = new Project
                {
                    Id = 2,
                    Name = "TestProject2",
                    Client = new Client
                    {
                        Name = "TestClient2"
                    }
                },
                FactTrackedInMinutes = 250,
                TaskName = "Test 1"
            },
        };

        var expectedResult = new List<TrackedTimePerProjectReportDto>
        {
            new()
            {
                ProjectId = 1,
                ProjectName = "TestProject1",
                ClientName = "TestClient1",
                TasksCount = 2,
                TotalTimeInMinutes = 460,
                Color = user.Color
            },
            new()
            {
                ProjectId = 2,
                ProjectName = "TestProject2",
                ClientName = "TestClient2",
                TasksCount = 1,
                TotalTimeInMinutes = 250,
                Color = user.Color
            }
        };

        _userAccessServiceMock
            .Setup(x => x.CanAccessUserReportInformation(query.UserId, _cancellationToken))
            .ReturnsAsync(true);

        _timeTrackDbContextMock
            .Setup(x => x.TimeTrackUsers)
            .Returns(new List<TimeTrackUser>{user}.BuildMockDbSet().Object);

        Expression<Func<GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock.Setup(x =>
                x.ExecuteAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResult, opt => opt.WithStrictOrdering());
    }
}
