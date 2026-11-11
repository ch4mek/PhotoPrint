using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetDashboardOfUserProjects;

public class GetDashboardOfUserProjectsQueryHandlerTests
{
    private readonly Mock<IGetAccessedDepartmentIdForCurrentUserBusinessRule> _getAccessedDepartmentMock = new();
    private readonly Mock<ITimesheetQueryBuilder> _iTimesheetQueryBuilderMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetDashboardOfUserProjectsRequest _handler;

    public GetDashboardOfUserProjectsQueryHandlerTests()
    {
        _handler = new GetDashboardOfUserProjectsRequest(
            _getAccessedDepartmentMock.Object,
            _authenticatedUserContextMock.Object,
            _iTimesheetQueryBuilderMock.Object);
    }

    [Fact]
    public async Task GetDashboardUsersWithValidRequestReturnsDashboardManagerPagingUserViewModel()
    {
        // Arrange
        _getAccessedDepartmentMock.Setup(c => c.Execute("Net", _token))
            .ReturnsAsync("Net");
        var dbSet = GetMockDbSet();

        _iTimesheetQueryBuilderMock.Setup(x => x.FromAsNoTracked(It.IsAny<int>(), It.IsAny<int[]>()))
            .Returns(_iTimesheetQueryBuilderMock.Object);
        _iTimesheetQueryBuilderMock.Setup(x => x.WithDepartmentFilter(It.IsAny<string>()))
            .Returns(_iTimesheetQueryBuilderMock.Object);
        _iTimesheetQueryBuilderMock.Setup(x => x.WithResourceGroupFilter(It.IsAny<int?>()))
            .Returns(_iTimesheetQueryBuilderMock.Object);
        _iTimesheetQueryBuilderMock.Setup(x => x.WithUserFilter(It.IsAny<int>()))
            .Returns(_iTimesheetQueryBuilderMock.Object);
        _iTimesheetQueryBuilderMock.Setup(x => x.WithBillableHoursFilter(It.IsAny<bool>()))
            .Returns(_iTimesheetQueryBuilderMock.Object);

        _iTimesheetQueryBuilderMock.Setup(x => x.Build())
            .Returns(dbSet.Object);

        var expectedResult = new List<DashboardOfUserProjectsCoreDto>
        {
            new ()
            {
                ClientId = 1,
                ClientName = "First",
                ProjectColor = "#ABC3E",
                ProjectId = 1,
                ProjectName = "First",
                TasksCount = 3,
                TotalTrackedInMinutes = 180,
                UserIsActive = true,
            },
            new ()
            {
                ClientId = 2,
                ClientName = "Second",
                ProjectColor = "#ABC3E",
                ProjectId = 2,
                ProjectName = "Second",
                TasksCount = 3,
                TotalTrackedInMinutes = 180,
                UserIsActive = true,
            }
        };

        // Act
        var result = await _handler.Handle(new GetDashboardOfUserProjectsRequest.Query(1, 2023, [11, 12], "Net"), _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    private static Mock<Microsoft.EntityFrameworkCore.DbSet<Timesheet>> GetMockDbSet()
    {
        var user = new TimeTrackUser
        {
            IsActive = true,
            FirstName = "test",
            LastName = "test",
            Email = "test"
        };
        var project1 = new Project
        {
            Id = 1,
            Name = "First",
            Color = "#ABC3E",
            ClientId = 1,
            Client = new Client
            {
                Id = 1,
                Name = "First"
            }
        };
        var project2 = new Project
        {
            Id = 2,
            Name = "Second",
            Color = "#ABC3E",
            ClientId = 2,
            Client = new Client
            {
                Id = 2,
                Name = "Second"
            }
        };
        var values = new List<Timesheet> {
            new()
            {
                User = user,
                ProjectId = project1.Id,
                Project = project1,
                FactTrackedInMinutes = 60,
                TaskName = "Test 1"
            },
            new()
            {
                User = user,
                ProjectId = project1.Id,
                Project = project1,
                FactTrackedInMinutes = 60,TaskName = "Test 1"
            },
            new()
            {
                User = user,
                ProjectId = project1.Id,
                Project = project1,
                FactTrackedInMinutes = 60,TaskName = "Test 1"
            },

            new()
            {
                User = user,
                ProjectId = project2.Id,
                Project = project2,
                FactTrackedInMinutes = 60,TaskName = "Test 1"
            },
            new()
            {
                User = user,
                ProjectId = project2.Id,
                Project = project2,
                FactTrackedInMinutes = 60,TaskName = "Test 1"
            },
            new()
            {
                User = user,
                ProjectId = project2.Id,
                Project = project2,
                FactTrackedInMinutes = 60,TaskName = "Test 1"
            },
        }.BuildMockDbSet();
        return values;
    }
}
