using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Queries;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Queries;

public class GetTrackedTimePayloadForUsersUseCaseTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new ();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetTrackedTimePayloadForUsersUseCase _useCase;

    private readonly DateTime _currentDate = new(2025, 3, 24);

    public GetTrackedTimePayloadForUsersUseCaseTests()
    {
        _useCase = new GetTrackedTimePayloadForUsersUseCase(
            _timeTrackDbContextMock.Object,
            _clockServiceMock.Object,
            _identityContractMock.Object);

        SetupUtcNow();
    }

    [Fact]
    public async Task HandleWhenOneAssignmentShouldReturnCorrectData()
    {
        //Arrange
        var projectUsers = new List<ProjectUser>()
        {
            new()
            {
                Id = 1,
                Project = new()
                {
                    IsPublic = false,
                    Name = "test"
                },
                ProjectId = 1,
                UserId = 1,
                Fte = 1,
                UserOnProjectFrom = new DateOnly(2025, 3, 3),
                IsTimeOnProjectBillable = true,
            },
        };

        var timesheets = new List<Timesheet>()
        {
            new()
            {
                Id = 1,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 10,
                BillableTrackedInMinutes = 10,
                ReportForDate = new DateOnly(2025, 3, 4),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new() {  Name = "test"}
            },
            new()
            {
                Id = 2,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 5,
                BillableTrackedInMinutes = 3,
                ReportForDate = new DateOnly(2025, 3, 5),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new(){  Name = "test"}
            },
        };

        var projectName = "projectName";
        var projects = new List<Project>()
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                Name = projectName,
            }
        };

        var clientName = "clientName";
        var clientColor = "#ffffff";
        var clients = new List<Client>()
        {
            new()
            {
                Id = 1,
                Name = clientName,
                Color = clientColor,
            }
        };

        SetupProjectUsers(projectUsers);
        SetupTimesheets(timesheets);
        SetupProjects(projects);
        SetupClients(clients);

        var request = new GetTrackedTimePayloadForUsersUseCase.Query([1], 2025, null, null);

        //Act
        var result = await _useCase.Handle(request, _cancellationToken);

        //Assert
        result.Clients.First().Name.Should().Be(clientName);
        result.Clients.First().Color.Should().Be(clientColor);

        result.Projects.First().Name.Should().Be(projectName);

        result.Users.Count.Should().Be(1);
        result.Users.First().HourDetails.First().MonthOfYear.Should().Be(3);
        result.Users.First().HourDetails.First().TotalBillableTrackedInMinutes.Should().Be(13);
        result.Users.First().HourDetails.First().TotalFactTrackedInMinutes.Should().Be(15);
    }

    [Fact]
    public async Task HandleWhenTwoAssignmentForOneProjectShouldReturnCorrectData()
    {
        //Arrange
        var projectUsers = new List<ProjectUser>()
        {
            new()
            {
                Id = 1,
                Project = new()
                {
                    IsPublic = false,
                    Name = "test"
                },
                ProjectId = 1,
                UserId = 1,
                Fte = 1,
                UserOnProjectFrom = new DateOnly(2025, 3, 3),
                UserOnProjectTo = new DateOnly(2025, 3, 5),
                IsTimeOnProjectBillable = true,
            },
            new()
            {
                Id = 2,
                Project = new()
                {
                    IsPublic = false,
                    Name = "test"
                },
                ProjectId = 1,
                UserId = 1,
                Fte = 1,
                UserOnProjectFrom = new DateOnly(2025, 3, 7),
                UserOnProjectTo = new DateOnly(2025, 3, 9),
                IsTimeOnProjectBillable = true
            },
        };

        var timesheets = new List<Timesheet>()
        {
            new()
            {
                Id = 1,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 10,
                BillableTrackedInMinutes = 10,
                ReportForDate = new DateOnly(2025, 3, 4),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new(){ Name = "test"}
            },
            new()
            {
                Id = 2,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 5,
                BillableTrackedInMinutes = 3,
                ReportForDate = new DateOnly(2025, 3, 5),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new(){ Name = "test"}
            },
            new()
            {
                Id = 2,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 1,
                BillableTrackedInMinutes = 22,
                ReportForDate = new DateOnly(2025, 3, 8),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new(){ Name = "test"}
            },
        };

        var projectName = "projectName";
        var projects = new List<Project>()
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                Name = projectName,
            }
        };

        var clientName = "clientName";
        var clientColor = "#ffffff";
        var clients = new List<Client>()
        {
            new()
            {
                Id = 1,
                Name = clientName,
                Color = clientColor,
            }
        };

        SetupProjectUsers(projectUsers);
        SetupTimesheets(timesheets);
        SetupProjects(projects);
        SetupClients(clients);

        var request = new GetTrackedTimePayloadForUsersUseCase.Query([1], 2025, null, null);

        //Act
        var result = await _useCase.Handle(request, _cancellationToken);

        //Assert
        result.Clients.First().Name.Should().Be(clientName);
        result.Clients.First().Color.Should().Be(clientColor);

        result.Projects.First().Name.Should().Be(projectName);

        result.Users.Count.Should().Be(1);

        result.Users.First().HourDetails.First().MonthOfYear.Should().Be(3);
        result.Users.First().HourDetails.First().TotalBillableTrackedInMinutes.Should().Be(35);
        result.Users.First().HourDetails.First().TotalFactTrackedInMinutes.Should().Be(16);

        result.Users.First().HourDetails.First().Projects
            .First(project => project.UserOnProjectFrom == new DateOnly(2025, 3, 3)).TotalBillableTrackedInMinutes.Should().Be(13);
        result.Users.First().HourDetails.First().Projects
            .First(project => project.UserOnProjectFrom == new DateOnly(2025, 3, 3)).TotalFactTrackedInMinutes.Should().Be(15);

        result.Users.First().HourDetails.First().Projects
            .First(project => project.UserOnProjectFrom == new DateOnly(2025, 3, 7)).TotalBillableTrackedInMinutes.Should().Be(22);
        result.Users.First().HourDetails.First().Projects
            .First(project => project.UserOnProjectFrom == new DateOnly(2025, 3, 7)).TotalFactTrackedInMinutes.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenTwoAssignmentForTwoProjectShouldReturnCorrectData()
    {
        //Arrange
        var projectUsers = new List<ProjectUser>()
        {
            new()
            {
                Id = 1,
                Project = new()
                {
                    IsPublic = false,
                    Name = "test"
                },
                ProjectId = 1,
                UserId = 1,
                Fte = 1,
                UserOnProjectFrom = new DateOnly(2025, 3, 3),
                UserOnProjectTo = new DateOnly(2025, 3, 5),
                IsTimeOnProjectBillable = true,
            },
            new()
            {
                Id = 2,
                Project = new()
                {
                    IsPublic = false,
                    Name = "test"
                },
                ProjectId = 2,
                UserId = 1,
                Fte = 1,
                UserOnProjectFrom = new DateOnly(2025, 3, 7),
                UserOnProjectTo = new DateOnly(2025, 3, 9),
                IsTimeOnProjectBillable = true,
            },
        };

        var timesheets = new List<Timesheet>()
        {
            new()
            {
                Id = 1,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 10,
                BillableTrackedInMinutes = 10,
                ReportForDate = new DateOnly(2025, 3, 4),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new(){ Name = "test"}
            },
            new()
            {
                Id = 2,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 5,
                BillableTrackedInMinutes = 3,
                ReportForDate = new DateOnly(2025, 3, 5),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 1,
                Project = new(){ Name = "test"}
            },
            new()
            {
                Id = 2,
                UserId = 1,
                TaskName = "TaskName",
                FactTrackedInMinutes = 1,
                BillableTrackedInMinutes = 22,
                ReportForDate = new DateOnly(2025, 3, 8),
                ReportYear = 2025,
                ReportMonthOfYear = 3,
                ProjectId = 2,
                Project = new(){ Name = "test"}
            },
        };

        var firstProjectName = "firstProjectName";
        var secondProjectName = "secondProjectName";
        var projects = new List<Project>()
        {
            new()
            {
                Id = 1,
                ClientId = 1,
                Name = firstProjectName,
            },
            new()
            {
                Id = 2,
                ClientId = 1,
                Name = secondProjectName,
            },
        };

        var clientName = "clientName";
        var clientColor = "#ffffff";
        var clients = new List<Client>()
        {
            new()
            {
                Id = 1,
                Name = clientName,
                Color = clientColor,
            }
        };

        SetupProjectUsers(projectUsers);
        SetupTimesheets(timesheets);
        SetupProjects(projects);
        SetupClients(clients);

        var request = new GetTrackedTimePayloadForUsersUseCase.Query([1], 2025, null, null);

        //Act
        var result = await _useCase.Handle(request, _cancellationToken);

        //Assert
        result.Clients.First().Name.Should().Be(clientName);
        result.Clients.First().Color.Should().Be(clientColor);

        result.Projects.First(project => project.ProjectId == 1).Name.Should().Be(firstProjectName);
        result.Projects.First(project => project.ProjectId == 2).Name.Should().Be(secondProjectName);

        result.Users.Count.Should().Be(1);

        result.Users.First().HourDetails.First().MonthOfYear.Should().Be(3);
        result.Users.First().HourDetails.First().TotalBillableTrackedInMinutes.Should().Be(35);
        result.Users.First().HourDetails.First().TotalFactTrackedInMinutes.Should().Be(16);

        result.Users.First().HourDetails.First().Projects
            .First(project => project.ProjectId == 1).TotalBillableTrackedInMinutes.Should().Be(13);
        result.Users.First().HourDetails.First().Projects
            .First(project => project.ProjectId == 1).TotalFactTrackedInMinutes.Should().Be(15);

        result.Users.First().HourDetails.First().Projects
            .First(project => project.ProjectId == 2).TotalBillableTrackedInMinutes.Should().Be(22);
        result.Users.First().HourDetails.First().Projects
            .First(project => project.ProjectId == 2).TotalFactTrackedInMinutes.Should().Be(1);
    }

    private void SetupProjectUsers(List<ProjectUser> projectUsers)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.ProjectUsers)
            .Returns(projectUsers.BuildMockDbSet().Object);
    }

    private void SetupTimesheets(List<Timesheet> timesheets)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Timesheets)
            .Returns(timesheets.BuildMockDbSet().Object);
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupClients(List<Client> clients)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }

    private void SetupUtcNow()
    {
        _clockServiceMock
            .Setup(mock => mock.UtcNow)
            .Returns(_currentDate);
    }
}
