using Bogus;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class CreateProjectRequestTests
{
    private readonly Mock<IHexColorGenerator> _hexColorGeneratorMock = new();
    private readonly Mock<ICanUserBeProjectManagerBusinessRule> _canUserBeProjectManagerBusinessRuleMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();

    private readonly Faker<CreateProjectRequest.Command> _createProjectCommandFaker = new();
    private readonly Faker<Project> _projectFaker = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateProjectRequest _handler;

    public CreateProjectRequestTests()
    {
        _handler = new CreateProjectRequest(
            _hexColorGeneratorMock.Object,
            _canUserBeProjectManagerBusinessRuleMock.Object,
            _authenticatedUserContextMock.Object,
            _communicationBusMock.Object,
            _timeTrackDbContextMock.Object);

        _createProjectCommandFaker.CustomInstantiator(x => new CreateProjectRequest.Command(
            Name: "TestFirstName",
            Color: x.Internet.Color(),
            ManagerId: null,
            ClientId: 1,
            IsPublic: false,
            IsBillable: true,
            TechStack: null,
            Industry: null,
            CloudServices: null,
            Architecture: null
        ));

        _projectFaker.CustomInstantiator(_ => new Project
        {
            Id = 2,
            ClientId = 2,
            ManagerUserId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
            Name = "TestProjectName",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        });
    }

    [Fact]
    public async Task HandleWhenProjectIsPublicAndBillableShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = _createProjectCommandFaker.Clone()
            .RuleFor(x => x.IsPublic, true)
            .RuleFor(x => x.IsBillable, true)
            .Generate();

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenProjectWithSameNameAlreadyExistsShouldThrowEntityConflictException()
    {
        // Arrange
        var command = _createProjectCommandFaker.Generate();

        var existingProject = _projectFaker.Clone()
            .RuleFor(x => x.Name, command.Name)
            .Generate();

        SetupProjects([existingProject]);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task HandleWhenManagerIdIsNotNullAndUserCannotBeManagerShouldThrowActionNotAllowedException()
    {
        // Arrange
        var command = _createProjectCommandFaker.Clone()
            .RuleFor(x => x.ManagerId, 1)
            .Generate();

        SetupProjects([]);
        SetupCanUserBeProjectManager(false);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.UserCantBeManager);
    }

    [Fact]
    public async Task HandleWhenCommandIsValidShouldCreateProject()
    {
        // Arrange
        var command = _createProjectCommandFaker.Clone()
            .RuleFor(x => x.ManagerId, 1)
            .Generate();

        var project = new Project
        {
            Id = 0,
            ClientId = command.ClientId,
            ManagerUserId = command.ManagerId,
            Color = command.Color!,
            CreatedById = null,
            IsActive = true,
            IsBillable = command.IsBillable,
            IsPublic = command.IsPublic,
            Name = command.Name,
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        var projectDto = new ProjectDto
        {
            Id = 0,
            Name = project.Name,
            Color = project.Color,
            ClientId = project.ClientId,
            ManagerId = project.ManagerUserId,
            IsBillable = project.IsBillable,
            IsActive = project.IsActive,
            IsPublic = project.IsPublic
        };

        const string expectedColor = "#AAAAAA";

        var client = new Client()
        {
            Id = command.ClientId,
            Name = "Name"
        };

        SetupClients([client]);
        SetupProjects([]);
        SetupCanUserBeProjectManager(true);

        _hexColorGeneratorMock
            .Setup(x => x.GenerateColor())
            .Returns(expectedColor);

        // Act
        var result = await _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(projectDto);
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupCanUserBeProjectManager(bool result)
    {
        _canUserBeProjectManagerBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(result);
    }

    private void SetupClients(List<Client> clients)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }
}
