using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.LookupProjects.SortedLookup;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class ProjectsSortedLookupQueryHandlerTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetProjectsSortedLookupRequest _handler;

    public ProjectsSortedLookupQueryHandlerTests()
    {
        _handler = new GetProjectsSortedLookupRequest(
            _authenticatedUserContextMock.Object,
            _timeTrackDbContextMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task HandleWhenUserIsCEOorCTOShouldReturnAllProjects(string userRole)
    {
        // Arrange
        const string? keyWord = null;

        const int ceoUserId = 5;
        const int ctoUserId = 6;
        const int headUserId = 7;
        const int employeeUserId = 8;

        const string headDepartment = "Js";

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ceoUserId,
                IsActive = true,
                IsBillable = true,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers = [],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 2,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ctoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = true,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = ceoUserId,
                            FirstName = "Nina",
                            LastName = "Ivanova",Email = "test",
                            DepartmentId = null
                        },
                        UserId = ceoUserId
                    },
                    new() {
                        User = new()
                        {
                            UserId = employeeUserId,
                            FirstName = "John",
                            LastName = "Doe",Email = "test",
                            DepartmentId = headDepartment
                        },
                        UserId = employeeUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 3,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = headUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = ctoUserId,
                            FirstName = "Julia",
                            LastName = "Roberts",Email = "test",
                            DepartmentId = null
                        },
                        UserId = ctoUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 4,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ceoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = true,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = employeeUserId,
                            FirstName = "John",
                            LastName = "Doe",Email = "test",
                            DepartmentId = headDepartment
                        },
                        UserId = employeeUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                User = new()
                {
                    UserId = employeeUserId,
                    FirstName = "John",
                    LastName = "Doe",Email = "test",
                    DepartmentId = headDepartment
                }
            },
            new()
            {
                User = new()
                {
                    UserId = headUserId,
                    FirstName = "Ben",
                    LastName = "Alen",Email = "test",
                    DepartmentId = headDepartment
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ctoUserId,
                    FirstName = "Julia",
                    LastName = "Roberts",Email = "test",
                    DepartmentId = null
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ceoUserId,
                    FirstName = "Nina",
                    LastName = "Ivanova",Email = "test",
                    DepartmentId = null
                }
            }
        };

        SetupProjects(projects);

        var mappedUsers = projectUsers.ConvertAll(x => new ProjectUserInfoDto
        {
            Id = x.User.UserId,
            FirstName = x.User.FirstName!,
            LastName = x.User.LastName!,
            DepartmentId = x.User.DepartmentId
        });

        projects.ConvertAll(x => new SortedProjectsLookupDbQueryModel
        (
            Id: x.Id,
            CreatedById: x.CreatedById,
            Color: x.Color,
            IsPublic: x.IsPublic,
            Name: x.Name,
            Type: x.GetProjectType(),
            Client: "Modsen",
            ProjectUsers: mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .Select(projectUser => new ProjectUserInfoModel(
                    projectUser.Id,
                    projectUser.FirstName,
                    projectUser.LastName,
                    projectUser.DepartmentId))
                .ToList()
        ));

        var mappedProjects = projects.ConvertAll(x => new ProjectSortedLookupDto
        {
            Id = x.Id,
            CreatedById = x.CreatedById,
            Color = x.Color,
            IsPublic = x.IsPublic,
            Name = x.Name,
            Type = x.GetProjectType(),
            ClientName = "Modsen",
            ProjectUsers = mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .OrderBy(x => x.Id)
                .ToList()
        });

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([userRole]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userRole == PortalRoleNames.Ceo ? ceoUserId : ctoUserId);

        var expectedProjects = mappedProjects
            .OrderBy(project => project.Type)
            .ThenBy(project => project.Id)
            .ToList();

        var query = new GetProjectsSortedLookupRequest.Query(keyWord);

        // Act
        var result = await _handler.Handle(
            query,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedProjects,
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentShouldReturnProjectsWhereAssignedUserFromSameDepartmentOrCreatedBySelfOrPublic()
    {
        // Arrange
        const string? keyWord = null;

        const int ceoUserId = 5;
        const int ctoUserId = 6;
        const int headUserId = 7;
        const int employeeUserId = 8;

        const string headDepartmentId = "Js";

        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ceoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = true,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers = [],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 2,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ctoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = ceoUserId,
                            FirstName = "Nina",
                            LastName = "Ivanova",Email = "test",
                            DepartmentId = null
                        },
                        UserId = ceoUserId
                    },
                    new() {
                        User = new()
                        {
                            UserId = employeeUserId,
                            FirstName = "John",
                            LastName = "Doe",Email = "test",
                            DepartmentId = headDepartmentId
                        },
                        UserId = employeeUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 3,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = headUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = ctoUserId,
                            FirstName = "Julia",
                            LastName = "Roberts",Email = "test",
                            DepartmentId = null
                        },
                        UserId = ctoUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                User = new()
                {
                    UserId = employeeUserId,
                    FirstName = "John",
                    LastName = "Doe",Email = "test",
                    DepartmentId = headDepartmentId
                },
            },
            new()
            {
                User = new()
                {
                    UserId = headUserId,
                    FirstName = "Ben",
                    LastName = "Alen",Email = "test",
                    DepartmentId = headDepartmentId
                },
            },
            new()
            {
                User = new()
                {
                    UserId = ctoUserId,
                    FirstName = "Julia",
                    LastName = "Roberts",Email = "test",
                    DepartmentId = null
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ceoUserId,
                    FirstName = "Nina",
                    LastName = "Ivanova",Email = "test",
                    DepartmentId = null
                }
            }
        };

        var mappedUsers = projectUsers.ConvertAll(x => new ProjectUserInfoDto
        {
            Id = x.User.UserId,
            FirstName = x.User.FirstName!,
            LastName = x.User.LastName!,
            DepartmentId = x.User.DepartmentId
        });

        var mappedProjects = projects.ConvertAll(x => new ProjectSortedLookupDto
        {
            Id = x.Id,
            CreatedById = x.CreatedById,
            Color = x.Color,
            IsPublic = x.IsPublic,
            Name = x.Name,
            Type = x.GetProjectType(),
            ClientName = "Modsen",
            ProjectUsers = mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .OrderBy(x => x.Id)
                .ToList()
        });

        projects.ConvertAll(x => new SortedProjectsLookupDbQueryModel
        (
            Id: x.Id,
            CreatedById: x.CreatedById,
            Color: x.Color,
            IsPublic: x.IsPublic,
            Name: x.Name,
            Type: x.GetProjectType(),
            Client: "Modsen",
            ProjectUsers: mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .Select(projectUser => new ProjectUserInfoModel(
                    projectUser.Id,
                    projectUser.FirstName,
                    projectUser.LastName,
                    projectUser.DepartmentId))
                .ToList()
        ));

        SetupProjects(projects);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(headDepartmentId);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(headUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(headUserId);

        var expectedProjects = mappedProjects
            .OrderBy(project => project.Type)
            .ThenBy(project => project.Id)
            .ToList();

        var query = new GetProjectsSortedLookupRequest.Query(keyWord);

        // Act
        var result = await _handler.Handle(
            query,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedProjects,
            options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(8)]
    public async Task HandleWhenUserIsEmployeeAndUserIdIsNullOrHisIdShouldReturnProjectsWhereUserIsAssignedToWithPublicProjects(int? userId)
    {
        // Arrange
        const string? keyWord = null;

        const int ceoUserId = 5;
        const int ctoUserId = 6;
        const int headUserId = 7;
        const int employeeUserId = 8;

        const string headDepartment = "Js";

        var projects = new List<Project>
        {
            new()
            {
                Id = 2,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ctoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = true,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = ceoUserId,
                            FirstName = "Nina",
                            LastName = "Ivanova",Email = "test",
                            DepartmentId = null
                        },
                        UserId = ceoUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 4,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ceoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = employeeUserId,
                            FirstName = "John",
                            LastName = "Doe",Email = "test",
                            DepartmentId = headDepartment
                        },
                        UserId = employeeUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                User = new()
                {
                    UserId = employeeUserId,
                    FirstName = "John",
                    LastName = "Doe",Email = "test",
                    DepartmentId = headDepartment
                }
            },
            new()
            {
                User = new()
                {
                    UserId = headUserId,
                    FirstName = "Ben",
                    LastName = "Alen",Email = "test",
                    DepartmentId = headDepartment
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ctoUserId,
                    FirstName = "Julia",
                    LastName = "Roberts",Email = "test",
                    DepartmentId = null
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ceoUserId,
                    FirstName = "Nina",
                    LastName = "Ivanova",Email = "test",
                    DepartmentId = null
                }
            }
        };

        var mappedUsers = projectUsers.ConvertAll(x => new ProjectUserInfoDto
        {
            Id = x.User.UserId,
            FirstName = x.User.FirstName!,
            LastName = x.User.LastName!,
            DepartmentId = x.User.DepartmentId
        });

        var mappedProjects = projects.ConvertAll(x => new ProjectSortedLookupDto
        {
            Id = x.Id,
            CreatedById = x.CreatedById,
            Color = x.Color,
            IsPublic = x.IsPublic,
            Name = x.Name,
            Type = x.GetProjectType(),
            ClientName = "Modsen",
            ProjectUsers = mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .OrderBy(project => project.Id)
                .ToList()
        });

        projects.ConvertAll(x => new SortedProjectsLookupDbQueryModel
        (
            Id: x.Id,
            CreatedById: x.CreatedById,
            Color: x.Color,
            IsPublic: x.IsPublic,
            Name: x.Name,
            Type: x.GetProjectType(),
            Client: "Modsen",
            ProjectUsers: mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .Select(projectUser => new ProjectUserInfoModel(
                    projectUser.Id,
                    projectUser.FirstName,
                    projectUser.LastName,
                    projectUser.DepartmentId))
                .ToList()
        ));

        SetupProjects(projects);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(headDepartment);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(employeeUserId);

        var expectedProjects = mappedProjects
                .OrderBy(project => project.Type)
                .ThenBy(project => project.Id)
                .ToList();

        var query = new GetProjectsSortedLookupRequest.Query(keyWord);

        // Act
        var result = await _handler.Handle(
            query,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedProjects,
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeAndUserIdIsNotHisShouldReturnOnlyPublicProjects()
    {
        // Arrange
        const string? keyWord = null;
        const int userId = 5;

        const int ceoUserId = 5;
        const int ctoUserId = 6;
        const int headUserId = 7;
        const int employeeUserId = 8;

        const string headDepartmentId = "Js";

        var projects = new List<Project>
        {
            new()
            {
                Id = 2,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ctoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = true,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = headUserId,
                            FirstName = "Ben",
                            LastName = "Alen",Email = "test",
                            DepartmentId = headDepartmentId
                        },
                        UserId = headUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            new()
            {
                Id = 4,
                Client = new()
                {
                    Id = 1,
                    IsActive = true,
                    Name = "Modsen",
                    Color = "#0f0f0f",
                },
                ManagerUserId = null,
                Color = "#000000",
                CreatedById = ceoUserId,
                IsActive = true,
                IsBillable = false,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToString(),
                ProjectUsers =
                [
                    new() {
                        User = new()
                        {
                            UserId = employeeUserId,
                            FirstName = "John",
                            LastName = "Doe",Email = "test",
                            DepartmentId = headDepartmentId
                        },
                        UserId = employeeUserId
                    }
                ],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        var projectUsers = new List<ProjectUser>
        {
            new()
            {
                User = new()
                {
                    UserId = employeeUserId,
                    FirstName = "John",
                    LastName = "Doe",Email = "test",
                    DepartmentId = headDepartmentId
                }
            },
            new()
            {
                User = new()
                {
                    UserId = headUserId,
                    FirstName = "Ben",
                    LastName = "Alen",Email = "test",
                    DepartmentId = headDepartmentId
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ctoUserId,
                    FirstName = "Julia",
                    LastName = "Roberts",Email = "test",
                    DepartmentId = null
                }
            },
            new()
            {
                User = new()
                {
                    UserId = ceoUserId,
                    FirstName = "Nina",
                    LastName = "Ivanova",Email = "test",
                    DepartmentId = null
                }
            }
        };

        var mappedUsers = projectUsers.ConvertAll(x => new ProjectUserInfoDto
        {
            Id = x.User.UserId,
            FirstName = x.User.FirstName!,
            LastName = x.User.LastName!,
            DepartmentId = x.User.DepartmentId
        });

        projects.ConvertAll(x => new SortedProjectsLookupDbQueryModel
        (
            Id: x.Id,
            CreatedById: x.CreatedById,
            Color: x.Color,
            IsPublic: x.IsPublic,
            Name: x.Name,
            Type: x.GetProjectType(),
            Client: "Modsen",
            ProjectUsers: mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .Select(projectUser => new ProjectUserInfoModel(
                    projectUser.Id,
                    projectUser.FirstName,
                    projectUser.LastName,
                    projectUser.DepartmentId))
                .ToList()
        ));

        var mappedProjects = projects.ConvertAll(x => new ProjectSortedLookupDto
        {
            Id = x.Id,
            CreatedById = x.CreatedById,
            Color = x.Color,
            IsPublic = x.IsPublic,
            Name = x.Name,
            Type = x.GetProjectType(),
            ClientName = "Modsen",
            ProjectUsers = mappedUsers
                .Where(user => x.ProjectUsers.Exists(projectUser => projectUser.User.UserId == user.Id))
                .ToList()
        });

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(headDepartmentId);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);

        SetupProjects(projects);

        var expectedProjects = mappedProjects
            .Where(project => project.IsPublic)
            .OrderBy(project => project.Type)
            .ThenBy(project => project.Id)
            .ToList();

        var query = new GetProjectsSortedLookupRequest.Query(
            keyWord);

        // Act
        var result = await _handler.Handle(
            query,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedProjects,
            options => options.WithStrictOrdering());
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }
}
