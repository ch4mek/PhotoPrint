using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class GetProjectsLookupByUserIdRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetProjectsLookupByUserIdRequest _handler;

    public GetProjectsLookupByUserIdRequestTests()
    {
        _handler = new GetProjectsLookupByUserIdRequest(
            _authenticatedUserContextMock.Object,
            _timeTrackDbContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task
        HandleWhenUserRoleCanGetOnlyOwnPrivateAndProjectsAndTryToGetOtherUserProjectsShouldThrowActionNotAllowedException(string role)
    {
        // Arrange
        const int currentUserId = 1;
        const int requestUserId = 2;

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(currentUserId);

        //Act
        var result = () => _handler.Handle(
            new GetProjectsLookupByUserIdRequest.Query(requestUserId, OnlyManagedProjects: null, IsPublic: null, IsActive: false,
                TrackIntervalFrom: null, TrackIntervalTo: null),
            _cancellationToken);

        //Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotAccessOtherUserData);
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    public async Task
        HandleWhenUserRoleCanGetOnlyOwnPrivateAndProjectsAndTryToGetOwnUserProjectsShouldReturnProjects(string role)
    {
        // Arrange
        const int currentUserId = 1;
        const int requestUserId = 1;

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                UserId = currentUserId,
                User = new()
                {
                    UserId = currentUserId,
                    ResourceGroupId = 1,
                    DepartmentId = ".Net",FirstName = "test", LastName = "test", Email = "test"
                }
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
                IsBillable = false,
                ProjectUsers = projectUsers,
                Timesheets = [],
                ManagerUserId = currentUserId,
            }
        };

        SetupProjects(projects);
        SetupProjectUsers(projectUsers);

        var expectedProjectLookupDto = new List<ProjectLookupDto> {
            new()
            {
                Id = 1,
                Name = "Project",
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = false
            }
        };

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
            new GetProjectsLookupByUserIdRequest.Query(requestUserId, OnlyManagedProjects: null, IsPublic: null, IsActive: false,
                TrackIntervalFrom: null, TrackIntervalTo: null),
                _cancellationToken);

        //Assert
        result.Should().BeEquivalentTo(expectedProjectLookupDto);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndTryToGetProjectsOfAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int requestUserId = 1;
        const int currentUserId = 2;

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
                }
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
                IsBillable = false,
                ProjectUsers = projectUsers,
                Timesheets = [],
            }
        };

        SetupProjects(projects);
        SetupProjectUsers(projectUsers);

        // Act
        var act = () => _handler.Handle(
            new GetProjectsLookupByUserIdRequest.Query(requestUserId, OnlyManagedProjects: null, IsPublic: null, IsActive: false,
                TrackIntervalFrom: null, TrackIntervalTo: null),
            _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.HeadOfDepartmentDoesNotHaveAccessToProjectsOfUserFromAnotherDepartment);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndIsManagerOfSomeUserProjectsShouldReturnManagedUserProjects()
    {
        // Arrange
        const int requestUserId = 1;
        const int currentUserId = 2;
        List<int> managedProjectsIds = [1, 2];

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
                }
            }
        };

        var projects = managedProjectsIds.Select(id => new Project
        {
            Id = id,
            Name = $"Name{id}",
            Client = new()
            {
                Name = "testName"
            },
            IsBillable = false,
            ProjectUsers = projectUsers,
            Timesheets = [],
            ManagerUserId = currentUserId,
        }).ToList();

        SetupProjects(projects);
        SetupProjectUsers(projectUsers);

        var projectLookupDtos = managedProjectsIds
            .Select(x => new ProjectLookupDto
            {
                Id = x,
                Name = $"Name{x}",
                ClientName = "testName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true
            })
            .ToList();

        var isCurrentUserAbleToSeeBillableTrackedTime = true;

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

        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(isCurrentUserAbleToSeeBillableTrackedTime);

        // Act
        var result = await _handler.Handle(
            new GetProjectsLookupByUserIdRequest.Query(requestUserId, OnlyManagedProjects: null, IsPublic: null, IsActive: false,
            TrackIntervalFrom: null, TrackIntervalTo: null),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(projectLookupDtos);
    }


    [Fact]
    public async Task HandleWhenUserDoesNotHaveProjectsShouldReturnEmptyList()
    {
        // Arrange
        var userId = 1;

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        SetupProjects([]);
        SetupProjectUsers([]);

        // Act
        var result = await _handler.Handle(
            new GetProjectsLookupByUserIdRequest.Query(1, OnlyManagedProjects: null, IsPublic: null, IsActive: false, TrackIntervalFrom: null, TrackIntervalTo: null),
            _cancellationToken);

        // Assert
        result.Should().NotBeNull();
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
}
