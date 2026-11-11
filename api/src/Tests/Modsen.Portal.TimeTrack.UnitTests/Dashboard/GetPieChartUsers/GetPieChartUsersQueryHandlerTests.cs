using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetPieChartUsers;

public class GetPieChartUsersQueryHandlerTests
{
    private readonly Mock<IGetAccessedDepartmentIdForCurrentUserBusinessRule> _getAccessedDepartmentMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<ITimesheetQueryBuilder> _timesheetQueryBuilderMock = new();

    private readonly GetPieChartUsersRequest _handler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetPieChartUsersQueryHandlerTests()
    {
        _handler = new GetPieChartUsersRequest(
            _getAccessedDepartmentMock.Object,
            _authenticatedUserContextMock.Object,
            _timesheetQueryBuilderMock.Object,
            _timeTrackDbContextMock.Object
        );
    }

    [Fact]
    public async Task GetPieCartUsersManagerWithValidRequestReturnsGetPieChartUsersResponse()
    {
        // Arrange
        var monthOfYear = new[] { 11, 12 };

        _getAccessedDepartmentMock.Setup(c => c.Execute("Net", _token))
            .ReturnsAsync("Net");
        var users = new List<TimeTrackUser>()
        {
            new()
            {
                UserId = 1, 
                LastName = "Ivanov",
                FirstName = "Ivan",
                Email = "test",
                Color = "#A83CH",
                IsActive = false,
                DepartmentId = "Net"
            },
            new()
            {
                UserId = 2,
                LastName = "Lastova",
                FirstName = "Anna",
                Email = "test",
                Color = "#A83CH",
                IsActive = true,
                DepartmentId = "Net"
            },
            new()
            {
                UserId = 3,
                LastName = "American",
                FirstName = "Anton",
                Email = "test",
                Color = "#A83CH",
                IsActive = false,
                DepartmentId = "Net"
            }
        };
        var data = new List<Timesheet>()
        {
            new()
            {
                UserId = users[0].UserId,
                User = users[0],
                FactTrackedInMinutes = 2600,TaskName = "Test 1"
            },
            new()
            {
                UserId = users[1].UserId,
                User = users[1],
                FactTrackedInMinutes = 1300,TaskName = "Test 1"
            },
            new()
            {
                UserId = users[2].UserId,
                User = users[2],
                FactTrackedInMinutes = 2000,TaskName = "Test 1"
            }
        }.BuildMockDbSet();

        _timesheetQueryBuilderMock
           .Setup(q => q.FromAsNoTracked(It.IsAny<int?>(), It.IsAny<int[]?>()))
           .Returns(_timesheetQueryBuilderMock.Object);

        _timesheetQueryBuilderMock
           .Setup(q => q.Build())
           .Returns(data.Object);

        _timeTrackDbContextMock.Setup(x => x.TimeTrackUsers)
            .Returns(users.BuildMockDbSet().Object);

        var expectedResult = new List<PieChartUsersCoreDto>
        {
            new ()
            {
                FirstName = "Ivan", LastName = "Ivanov", Color = "#A83CH", TrackedTime = 2600, IsDeleted = false
            },
            new ()
            {
                FirstName = "Anna", LastName = "Lastova", Color = "#A83CH", TrackedTime = 1300, IsDeleted = true
            },
            new ()
            {
                FirstName = "Anton", LastName = "American", Color = "#A83CH", TrackedTime = 2000, IsDeleted = false
            }
        };

        // Act
        var result = await _handler.Handle(new GetPieChartUsersRequest.Query(2023, monthOfYear, "Net"), _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
