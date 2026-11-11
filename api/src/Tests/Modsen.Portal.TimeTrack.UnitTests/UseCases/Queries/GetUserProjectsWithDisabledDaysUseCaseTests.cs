using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Project = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Project;
using ProjectUser = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.ProjectUser;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Queries;

public class GetUserProjectsWithDisabledDaysUseCaseTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IEnvironmentFeatureContext> _environmentFeatureContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetUserProjectsWithDisabledDaysRequest _handler;

    public GetUserProjectsWithDisabledDaysUseCaseTests()
    {
        _handler = new GetUserProjectsWithDisabledDaysRequest(
            _timeTrackDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _environmentFeatureContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task HandleWhenUserRoleCanGetOnlyOwnProjectsAndTryToGetOtherUserProjectsShouldThrowActionNotAllowedException(string role)
    {
        // Arrange
        const int currentUserId = 1;
        const int requestUserId = 2;
        var timeTrackIntervalFrom = new DateOnly(2024, 1, 1);
        var timeTrackIntervalTo = new DateOnly(2024, 2, 1);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(currentUserId);

        //Act
        var result = () => _handler.Handle(
            new GetUserProjectsWithDisabledDaysRequest.Query(requestUserId, false, timeTrackIntervalFrom, timeTrackIntervalTo),
            _cancellationToken);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotAccessOtherUserData);
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task HandleWhenUserRoleCanGetOnlyOwnProjectsShouldReturnProjects(string role)
    {
        // Arrange
        const int currentUserId = 1;
        const int requestUserId = 1;
        var timeTrackIntervalFrom = new DateOnly(2024, 1, 1);
        var timeTrackIntervalTo = new DateOnly(2024, 2, 1);

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                UserId = currentUserId,
                User = new()
                {
                    UserId = currentUserId,
                    ResourceGroupId = 1,
                    DepartmentId = ".Net",
                    FirstName = "test", LastName = "test", Email = "test"
                },
                UserOnProjectFrom = timeTrackIntervalFrom,
                UserOnProjectTo = timeTrackIntervalTo
            }
        };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "Project",
                Client = new()
                {
                    Name = "ClientName"
                },
                IsActive = true,
                IsPublic = false,
                IsBillable = false,
                ProjectUsers = projectUsers,
                Timesheets = [],
                ManagerUserId = currentUserId,
            }
        };

        SetupProjects(projects);
        SetupProjectUsers(projectUsers);
        SetupIsCheckAssignDatesFeatureEnabled(true);

        var isCurrentUserAbleToSeeBillableTrackedTime = false;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(currentUserId);
        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(isCurrentUserAbleToSeeBillableTrackedTime);

        //Act
        var result = await _handler.Handle(
            new GetUserProjectsWithDisabledDaysRequest.Query(requestUserId, false, timeTrackIntervalFrom, timeTrackIntervalTo),
            _cancellationToken);

        //Assert
        result.Count.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndTryToGetProjectsOfAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int requestUserId = 1;
        const int currentUserId = 2;
        var timeTrackIntervalFrom = new DateOnly(2024, 1, 1);
        var timeTrackIntervalTo = new DateOnly(2024, 2, 1);

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                UserId = requestUserId,
                User = new()
                {
                    UserId = requestUserId,
                    ResourceGroupId = 1,
                    DepartmentId = "JS",
                    FirstName = "test", LastName = "test", Email = "test"
                },
                UserOnProjectFrom = timeTrackIntervalFrom,
                UserOnProjectTo = timeTrackIntervalTo
            }
        };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = "Project",
                Client = new()
                {
                    Name = "ClientName"
                },
                IsActive = true,
                IsPublic = false,
                IsBillable = false,
                ProjectUsers = projectUsers,
                Timesheets = [],
                ManagerUserId = 1000,
            }
        };

        SetupProjects(projects);
        SetupProjectUsers(projectUsers);
        SetupIsCheckAssignDatesFeatureEnabled(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(currentUserId);
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(currentUserId);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Net");

        // Act
        var act = () => _handler.Handle(
            new GetUserProjectsWithDisabledDaysRequest.Query(requestUserId, OnlyManagedProjects: null,
                TrackIntervalFrom: timeTrackIntervalFrom, TrackIntervalTo: timeTrackIntervalTo),
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.HeadOfDepartmentDoesNotHaveAccessToProjectsOfUserFromAnotherDepartment);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldReturnEmptyCollection()
    {
        // Arrange
        var userId = 1;
        var timeTrackIntervalFrom = new DateOnly(2024, 1, 1);
        var timeTrackIntervalTo = new DateOnly(2024, 2, 1);

        SetupProjects([]);
        SetupProjectUsers([]);

        // Act
        var result = await _handler.Handle(
            new GetUserProjectsWithDisabledDaysRequest.Query(userId, OnlyManagedProjects: null, TrackIntervalFrom: timeTrackIntervalFrom, TrackIntervalTo: timeTrackIntervalTo),
            _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupProjectUsers(List<ProjectUser> projectUsers)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.ProjectUsers)
            .Returns(projectUsers.BuildMockDbSet().Object);
    }

    private void SetupIsCheckAssignDatesFeatureEnabled(bool result)
    {
        _environmentFeatureContextMock
            .Setup(mock => mock.IsCheckAssignDatesFeatureEnabledAsync())
            .ReturnsAsync(result);
    }
}
