using Bogus;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Queries;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Queries;

public class GetUsersLookupWithFiltersForReportsQueryHandlerTests
{
    private readonly Mock<ITimeTrackUserProvider> _usersProviderMock = new();
    private readonly Mock<IProjectsProvider> _projectsProviderMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ICanUserBeProjectManagerBusinessRule> _canUserBeProjectManagerBusinessRule = new();
    private readonly Mock<IIsUserManagerOfAnyPublicProjectDbQuery> _isAnyPublicProjectManagedByUserIdDbQueryMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly Faker _faker;
    private readonly GetUsersLookupWithFiltersForReportsUseCase _handler;


    public GetUsersLookupWithFiltersForReportsQueryHandlerTests()
    {
        _handler = new GetUsersLookupWithFiltersForReportsUseCase(
            _usersProviderMock.Object,
            _projectsProviderMock.Object,
            _authenticatedUserContextMock.Object,
            _canUserBeProjectManagerBusinessRule.Object,
            _isAnyPublicProjectManagedByUserIdDbQueryMock.Object
            );

        _faker = new Faker();
    }

    [Fact]
    public async Task HandleWhenDataExistsWithoutFilterPropertiesReturnListOfUserDto()
    {
        //Arrange
        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: null,
            ProjectId: null,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: null,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 2,
                FirstName = name2,
                LastName = lastName2,
                Email = "modsen2@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            },

            new ()
            {
                UserId = 4,
                FirstName = name4,
                LastName = lastName4,
                 Email = "modsen4@modsen-software.com"
            }
        };

        var isClientWithPublicProject = false;
        var isPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        _usersProviderMock.Setup(x =>
                x.GetLookupWithParametersForReportsAsync(
                    It.IsAny<FilterParams>(),
                    isClientWithPublicProject,
                    isPublicProject,
                    isAnyPublicProjectManagedByUser,
                    It.IsAny<string>(),
                    _cancellationToken))
            .ReturnsAsync(users);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            It.IsAny<string>(),
            _cancellationToken),
            Times.Once);
        result.Count.Should().Be(entities.Count);
    }

    [Fact]
    public async Task HandleWhenFilterHasProjectIdAndProjectIsNotPublicReturnListOfUserDto()
    {
        //Arrange
        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: null,
            ProjectId: 2,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: null,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            }
        };

        var project = new TimeTrack.Domain.Project
        {
            Id = query.ProjectId!.Value,
            IsPublic = false,
            Name = "Name",
            Color = "#ffffff",
            ClientId = 0,
            ManagerId = null,
            IsBillable = false,
            IsActive = true,
            CreatedById = 0,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersForReportsAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                _cancellationToken))
            .ReturnsAsync(users);

        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(query.ProjectId.Value, _cancellationToken))
            .ReturnsAsync(project);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        var isClientWithPublicProject = false;
        var isPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            It.IsAny<string>(),
            _cancellationToken),
            Times.Once);
        result.Count.Should().NotBe(entities.Count);
    }

    [Fact]
    public async Task HandleWhenFilterHasProjectIdAndProjectIsPublicReturnListOfUserDto()
    {
        //Arrange
        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: null,
            ProjectId: 2,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: null,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            }
        };

        var project = new TimeTrack.Domain.Project
        {
            Id = query.ProjectId!.Value,
            IsPublic = true,
            Name = "Name",
            Color = "#ffffff",
            ClientId = 0,
            ManagerId = null,
            IsBillable = false,
            IsActive = true,
            CreatedById = 0,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersForReportsAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                _cancellationToken))
            .ReturnsAsync(users);

        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(query.ProjectId.Value, _cancellationToken))
            .ReturnsAsync(project);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        var isClientWithPublicProject = false;
        var isPublicProject = true;
        var isAnyPublicProjectManagedByUser = false;

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            It.IsAny<string>(),
            _cancellationToken),
            Times.Once);
        result.Count.Should().NotBe(entities.Count);
    }

    [Fact]
    public async Task HandleWhenFilterHasClientIdAndProjectIsNotPublicReturnListOfUserDto()
    {
        //Arrange
        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: 1,
            ProjectId: null,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: null,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            }
        };

        var projects = new List<TimeTrack.Domain.Project>
        {
            new()
            {
                Id = 1,
                IsPublic = false,
                Name = "Name",
                Color = "#ffffff",
                ClientId = 0,
                ManagerId = null,
                IsBillable = false,
                IsActive = true,
                CreatedById = 0,
                ProjectUsers = [],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersForReportsAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                _cancellationToken))
            .ReturnsAsync(users);

        _projectsProviderMock.Setup(x => x.GetProjectsByClientIdAsync(query.ClientId!.Value, _cancellationToken))
            .ReturnsAsync(projects);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        var isClientWithPublicProject = false;
        var isPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            It.IsAny<string>(),
            _cancellationToken),
            Times.Once);
        result.Count.Should().NotBe(entities.Count);
    }

    [Fact]
    public async Task HandleWhenFilterHasClientIdAndProjectIsPublicReturnListOfUserDto()
    {
        //Arrange
        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: 1,
            ProjectId: null,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: null,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            }
        };

        var projects = new List<TimeTrack.Domain.Project>
        {
            new()
            {
                Id = 2,
                IsPublic = true,
                Name = "Name",
                Color = "#ffffff",
                ClientId = 0,
                ManagerId = null,
                IsBillable = false,
                IsActive = true,
                CreatedById = 0,
                ProjectUsers = [],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            }
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersForReportsAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                _cancellationToken))
            .ReturnsAsync(users);

        _projectsProviderMock.Setup(x => x.GetProjectsByClientIdAsync(query.ClientId!.Value, _cancellationToken))
            .ReturnsAsync(projects);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        var isClientWithPublicProject = true;
        var isPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            It.IsAny<string>(),
            _cancellationToken),
            Times.Once);
        result.Count.Should().NotBe(entities.Count);
    }

    [Fact]
    public async Task HandleWhenFilterHasClientIdAndProjectIdAndIsNotPublicReturnListOfUserDto()
    {
        //Arrange
        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: 1,
            ProjectId: 2,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: null,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4
            }
        };
        var users = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            },

            new ()
            {
                UserId = 3,
                FirstName = name3,
                LastName = lastName3,
                Email = "modsen3@modsen-software.com"
            }
        };

        var project = new TimeTrack.Domain.Project
        {
            Id = query.ProjectId!.Value,
            IsPublic = false,
            Name = "Name1",
            Color = "#ffffff",
            ClientId = 0,
            ManagerId = null,
            IsBillable = false,
            IsActive = true,
            CreatedById = 0,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        var projects = new List<TimeTrack.Domain.Project>
        {
            new()
            {
                Id = 111,
                IsPublic = true,
                Name = "Name1",
                Color = "#ffffff",
                ClientId = 0,
                ManagerId = null,
                IsBillable = false,
                IsActive = true,
                CreatedById = 0,
                ProjectUsers = [],
                TechStack = null,
                Industry = null,
                CloudServices = null,
                Architecture = null
            },
            project
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersForReportsAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                _cancellationToken))
            .ReturnsAsync(users);

        _projectsProviderMock.Setup(x => x.GetProjectsByClientIdAsync(query.ClientId!.Value, _cancellationToken))
            .ReturnsAsync(projects);

        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(query.ProjectId!.Value, _cancellationToken))
            .ReturnsAsync(project);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        var isClientWithPublicProject = false;
        var isPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            It.IsAny<string>(),
            _cancellationToken),
            Times.Once);
        result.Count.Should().NotBe(entities.Count);
    }

    [Fact]
    public async Task HandleWhenKeyWordIsNotEmptyReturnListOfUserDto()
    {
        //Arrange
        var name1 = _faker.Name.FirstName();
        var name2 = _faker.Name.FirstName();
        var name3 = _faker.Name.FirstName();
        var name4 = _faker.Name.FirstName();

        var lastName1 = _faker.Name.LastName();
        var lastName2 = _faker.Name.LastName();
        var lastName3 = _faker.Name.LastName();
        var lastName4 = _faker.Name.LastName();

        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            ClientId: null,
            ProjectId: null,
            DepartmentId: null,
            ResourceGroupId: null,
            Keyword: name1,
            OnlyManagedUsers: null,
            ReportForDateFrom: null,
            ReportForDateTo: null);

        var entities = new List<TimeTrackUser>
        {
            new ()
            {
                UserId = 1,
                FirstName = name1,Email = "test",
                LastName = lastName1,
                DepartmentId = "Js"
            },
            new ()
            {
                UserId = 2,
                FirstName = name2,Email = "test",
                LastName = lastName2,
                DepartmentId = "Net"
            },
            new ()
            {
                UserId = 3,
                FirstName = name3,Email = "test",
                LastName = lastName3,
                DepartmentId = "Net"
            },
            new ()
            {
                UserId = 4,
                FirstName = name4,Email = "test",
                LastName = lastName4,
                DepartmentId = "Java"
            }
        };

        var users = new List<TimeTrackUser>
        {
           new ()
            {
                UserId = 1,
                FirstName = name1,
                LastName = lastName1,
                Email = "modsen1@modsen-software.com"
            }
        };

        _usersProviderMock.Setup(x => x.GetLookupWithParametersForReportsAsync(
                It.IsAny<FilterParams>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                _cancellationToken))
            .ReturnsAsync(users);

        _canUserBeProjectManagerBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        var isClientWithPublicProject = false;
        var isPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        //Act
        var result = await _handler.Handle(query, _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        _usersProviderMock.Verify(x => x.GetLookupWithParametersForReportsAsync(
            It.IsAny<FilterParams>(),
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            name1,
            _cancellationToken), Times.Once);
        result.Count.Should().NotBe(entities.Count);
        result.FirstOrDefault()!.FirstName.Should().Be(name1);
    }
}
