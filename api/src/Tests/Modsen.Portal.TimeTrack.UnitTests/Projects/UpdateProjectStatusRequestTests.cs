using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class UpdateProjectStatusRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateProjectStatusRequest _handler;

    public UpdateProjectStatusRequestTests()
    {
        _handler = new UpdateProjectStatusRequest(
            _timeTrackDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _communicationBusMock.Object);
    }

    [Fact]
    public async Task HandleWhenProjectDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new UpdateProjectStatusRequest.Command(5, false);

        SetupProjects([]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenProjectHasSameStatusAsInCommandShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateProjectStatusRequest.Command(5, false);

        var project = new Project
        {
            Id = command.Id,
            ClientId = 2,
            ManagerUserId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = command.IsActive,
            IsBillable = true,
            IsPublic = false,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        SetupProjects([project]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenCommandIsToActivateProjectAndUserIsNotCEOorCTOShouldThrowActionNotAllowedException()
    {
        // Arrange
        var command = new UpdateProjectStatusRequest.Command(5, true);

        var project = new Project
        {
            Id = command.Id,
            ClientId = 2,
            ManagerUserId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = false,
            IsBillable = true,
            IsPublic = false,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        SetupProjects([project]);
        SetupUserRoles([PortalRoleNames.HeadOfDepartment]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.YouCantActiveProject);
    }

    [Fact]
    public async Task HandleWhenCommandIsToDeactivateProjectWithAssignedManagerShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateProjectStatusRequest.Command(5, false);

        var project = new Project
        {
            Id = command.Id,
            ClientId = 2,
            ManagerUserId = 17,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        SetupProjects([project]);
        SetupUserRoles([PortalRoleNames.HeadOfDepartment]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.CantDeactivateProjectWithManagerAndUsers);
    }

    [Fact]
    public async Task HandleWhenCommandIsToDeactivateProjectAndProjectContainsUsersShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateProjectStatusRequest.Command(5, false);

        var project = new Project
        {
            Id = command.Id,
            ClientId = 2,
            ManagerUserId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
            Name = "Test",
            ProjectUsers =
            [
                new()
            ],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        SetupProjects([project]);
        SetupUserRoles([PortalRoleNames.Ceo]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.CantDeactivateProjectWithManagerAndUsers);
    }

    [Fact]
    public async Task HandleWhenCommandIsValidShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateProjectStatusRequest.Command(5, false);

        var project = new Project
        {
            Id = command.Id,
            ClientId = 2,
            ManagerUserId = null,
            Color = "#000000",
            CreatedById = 5,
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
            Name = "Test",
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        SetupProjects([project]);
        SetupUserRoles([PortalRoleNames.Ceo]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().NotThrowAsync();
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupUserRoles(string[] roles)
    {
        _authenticatedUserContextMock
            .Setup(mock => mock.GetRoles())
            .Returns(roles);
    }
}
