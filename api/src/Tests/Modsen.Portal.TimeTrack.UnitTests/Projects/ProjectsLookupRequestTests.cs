using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class ProjectsLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ICanUserBeProjectManagerBusinessRule> _canUserBeProjectManagerBusinessRuleMock = new();
    private readonly Mock<IProjectQueryBuilder> _projectQueryBuilder = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetProjectsLookupRequest _handler;

    public ProjectsLookupRequestTests()
    {
        _handler = new GetProjectsLookupRequest(
            _authenticatedUserContextMock.Object,
            _canUserBeProjectManagerBusinessRuleMock.Object,
            _projectQueryBuilder.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task HandleWhenUserIsCEOorCTOShouldReturnProjectsAndCallGetProjectsLookupDbQueryOnce(string userRole)
    {
        // Arrange
        const string? keyWord = null;
        bool? isPublic = false;
        bool? isActive = null;
        bool? onlyManagedProjects = null;

        var client = new Client
        {
            Name = "ClientName"
        };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#100000",
                IsPublic = true,
            },
            new()
            {
                Id = 2,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#200000",
                IsPublic = true,
            },
            new()
            {
                Id = 3,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#300000",
                IsPublic = true,
            },
        };

        SetupProjects(projects);

        var expectedProjectDtos = new List<ProjectLookupDto>
        {
            new()
            {
                Id = 1,
                Name = projects[0].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = "#100000"
            },
            new()
            {
                Id = 2,
                Name = projects[1].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = "#200000"
            },
            new()
            {
                Id = 3,
                Name = projects[2].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = "#300000"
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([userRole]);
        _authenticatedUserContextMock.Setup(x => x.IsManager)
            .Returns(true);
        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(false);

        var query = new GetProjectsLookupRequest.Query(keyWord, isPublic, isActive, onlyManagedProjects);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProjectDtos);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentShouldReturnProjectsAndCallGetProjectsLookupByDepartmentAndCreatorDbQueryOnce()
    {
        // Arrange
        const string? keyWord = null;
        bool? isPublic = false;
        bool? isActive = null;
        bool? onlyManagedProjects = false;

        const int userId = 1;
        const string userRole = PortalRoleNames.HeadOfDepartment;
        const string departmentId = "Js";

        var client = new Client
        {
            Name = "ClientName"
        };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#100000",
                ProjectUsers =
                [
                    new() {
                        User = new() {
                            DepartmentId = departmentId,
                            FirstName = "test", LastName = "test", Email = "test"
                        }
                    }
                ]
            },
            new()
            {
                Id = 2,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#200000",
                ProjectUsers =
                [
                    new() {
                        User = new() {
                            DepartmentId = departmentId,
                            FirstName = "test", LastName = "test", Email = "test"
                        }
                    }
                ]
            },
            new()
            {
                Id = 3,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#300000",
                ProjectUsers =
                [
                    new() {
                        User = new() {
                            DepartmentId = departmentId,
                            FirstName = "test", LastName = "test", Email = "test"
                        }
                    }
                ]
            },
        };

        SetupProjects(projects);

        var expectedProjectDtos = new List<ProjectLookupDto>
        {
            new()
            {
                Id = 1,
                Name = projects[0].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = "#100000"
            },
            new()
            {
                Id = 2,
                Name = projects[1].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = "#200000"
            },
            new()
            {
                Id = 3,
                Name = projects[2].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = "#300000"
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([userRole]);
        _authenticatedUserContextMock.Setup(x => x.IsManager)
            .Returns(true);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(false);

        var query = new GetProjectsLookupRequest.Query(keyWord, isPublic, isActive, onlyManagedProjects);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProjectDtos);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenUserIsEmployeeOrSysAdminShouldReturnProjectsAndCallGetProjectsLookupByUserIdDbQueryOnce(string userRole)
    {
        // Arrange
        const string? keyWord = null;
        bool? isPublic = true;
        bool? isActive = null;
        bool? onlyManagedProjects = null;

        const int userId = 1;

        var client = new Client
        {
            Name = "ClientName"
        };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#100000",
                IsPublic = true,
            },
            new()
            {
                Id = 2,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#200000",
                IsPublic = true,
            },
            new()
            {
                Id = 3,
                Name = Guid.CreateVersion7().ToString(),
                Client = client,
                IsBillable = false,
                Color = "#300000",
                IsPublic = true,
            },
        };

        SetupProjects(projects);

        var expectedProjectDtos = new List<ProjectLookupDto>
        {
            new()
            {
                Id = 1,
                Name = projects[0].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = false,
                Color = "#100000"
            },
            new()
            {
                Id = 2,
                Name = projects[1].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = false,
                Color = "#200000"
            },
            new()
            {
                Id = 3,
                Name = projects[2].Name,
                ClientName = "ClientName",
                IsBillable = false,
                IsCurrentUserAbleToSeeBillableTrackedTime = false,
                Color = "#300000"
            }
        };

        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(false);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([userRole]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        var query = new GetProjectsLookupRequest.Query(keyWord, isPublic, isActive, onlyManagedProjects);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProjectDtos);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenViewProjectExitsClaimAndNonPrivilegedRole_ShouldReturnEmptyList(string role)
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);

        var query = new GetProjectsLookupRequest.Query(null, null, null, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
        _projectQueryBuilder.Verify(x => x.Build(), Times.Never);
    }

    [Fact]
    public async Task HandleWhenManageAllProjectExitsClaimAndGroupManager_ShouldReturnAllProjectsIgnoringGroupFilter()
    {
        // Arrange
        const int userId = 1;
        const int userResourceGroupId = 1;

        var client = new Client { Name = "ClientName" };

        var projects = new List<Project>
        {
            new()
            {
                Id = 1, Name = "Project1", Client = client, IsBillable = false, Color = "#100000",
                IsPublic = false, CreatedById = 999,
                ProjectUsers = [new() { User = new() { ResourceGroupId = userResourceGroupId, FirstName = "a", LastName = "b", Email = "e" } }]
            },
            new()
            {
                Id = 2, Name = "Project2", Client = client, IsBillable = false, Color = "#200000",
                IsPublic = false, CreatedById = 999,
                ProjectUsers = [new() { User = new() { ResourceGroupId = 2, FirstName = "a", LastName = "b", Email = "e" } }]
            },
            new()
            {
                Id = 3, Name = "Project3", Client = client, IsBillable = false, Color = "#300000",
                IsPublic = true
            }
        };

        SetupProjects(projects);

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(userResourceGroupId);
        _authenticatedUserContextMock.Setup(x => x.IsManager).Returns(false);
        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(false);

        var query = new GetProjectsLookupRequest.Query(null, null, null, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task HandleWhenManageAllProjectExitsClaimShouldSkipValidation()
    {
        // Arrange
        const int userId = 1;
        const int userResourceGroupId = 1;

        var client = new Client { Name = "ClientName" };
        var projects = new List<Project>
        {
            new() { Id = 1, Name = "Project1", Client = client, IsBillable = false, Color = "#100000", IsPublic = true }
        };

        SetupProjects(projects);

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(userResourceGroupId);
        _authenticatedUserContextMock.Setup(x => x.IsManager).Returns(false);
        _authenticatedUserContextMock
            .Setup(x => x.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, It.IsAny<string>()))
            .Returns(false);

        var query = new GetProjectsLookupRequest.Query(null, null, null, true);

        // Act
        var act = async () => await _handler.Handle(query, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    private void SetupProjects(List<Project> projects)
    {
        _projectQueryBuilder
            .Setup(x=>x.WithKeyWord(It.IsAny<string?>()))
            .Returns(_projectQueryBuilder.Object);
        _projectQueryBuilder
            .Setup(x=>x.WithActiveStatus(It.IsAny<bool?>()))
            .Returns(_projectQueryBuilder.Object);
        _projectQueryBuilder
            .Setup(x=>x.WithPublicStatus(It.IsAny<bool?>()))
            .Returns(_projectQueryBuilder.Object);
        _projectQueryBuilder
            .Setup(x=>x.WithManagerId(It.IsAny<int?>()))
            .Returns(_projectQueryBuilder.Object);

        _projectQueryBuilder.Setup(x=>x.Build())
            .Returns(projects.BuildMockDbSet().Object);
    }
}
