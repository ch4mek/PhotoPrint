//using System.Linq.Expressions;
//using System.Threading;
//using Bogus;
//using Modsen.Portal.Core;
//using Modsen.Portal.Core.Identity;
//using Modsen.Portal.Core.Communication;
//using Modsen.Portal.Core.Context;
//using Modsen.Portal.Core.Exceptions;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetClientForProjectUpdateByClientIdDbQuery;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetProjectWithCreatedByIdAndPublicityAndProjectUsersDepartments;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
//using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Repositories;
//using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;
//using Modsen.Portal.TimeTrack.Domain;
//using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
//using Modsen.Portal.TimeTrack.UseCases.Dtos;
//using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

//namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Commands;

//public class UpdateProjectUseCaseTests
//{
//    private readonly Mock<IProjectsProvider> _projectsProviderMock = new();
//    private readonly Mock<IProjectUsersRepository> _projectUsersRepositoryMock = new();
//    private readonly Mock<ITimesheetProvider> _timesheetProviderMock = new();
//    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
//    private readonly Mock<IGetClientForProjectUpdateByClientIdDbQuery> _getClientForProjectUpdateByClientIdDbQueryMock = new();
//    private readonly Mock<ICanUserBeProjectManagerBusinessRule> _canUserBeProjectManagerBusinessRuleMock = new();
//    private readonly Mock<IUpdateProjectDbQuery> _updateProjectDbQueryMock = new();
//    private readonly Mock<IIsUserManagerOfProjectDbQuery> _isUserManagerOfProjectDbQueryMock = new();
//    private readonly Mock<ICommunicationBus> _communicationBus = new();
//    private readonly Mock<IGetProjectInfo> _getProjectInfoMock = new();
//    private readonly Mock<IGetResourceGroupIdsByClientDbQuery> _getResourceGroupIdsByClientDbQueryMock = new();

//    private readonly Faker<UpdateProjectRequest.Command> _updateProjectCommandFaker = new();
//    private readonly Faker<Client> _clientFaker = new();

//    private readonly CancellationToken _cancellationToken = CancellationToken.None;
//    private readonly UpdateProjectRequest _handler;

//    public UpdateProjectUseCaseTests()
//    {
//        _handler = new UpdateProjectRequest(
//            _projectsProviderMock.Object,
//            _timesheetProviderMock.Object,
//            _authenticatedUserContextMock.Object,
//            _getClientForProjectUpdateByClientIdDbQueryMock.Object,
//            _canUserBeProjectManagerBusinessRuleMock.Object,
//            _updateProjectDbQueryMock.Object,
//            _isUserManagerOfProjectDbQueryMock.Object,
//            _getProjectInfoMock.Object,
//            _getResourceGroupIdsByClientDbQueryMock.Object,
//            _communicationBus.Object);

//        _updateProjectCommandFaker.CustomInstantiator(_ =>
//            new UpdateProjectRequest.Command(
//                Id: 1,
//                Name: "TestName",
//                Color: "#0f0f0f",
//                ClientId: 1,
//                ManagerId: null,
//                IsPublic: true,
//                IsBillable: false,
//                TechStack: null,
//                Industry: null,
//                CloudServices: null,
//                Architecture: null
//            ));

//        _clientFaker.CustomInstantiator(_ => new Client
//        {
//            Id = 1,
//            IsActive = true,
//            Name = "TestClientName",
//            Color = "#000000",
//            Comments = "TestComment",
//            IsProjectExist = false,
//            FactTrackedTimeInMinutes = 0,
//            Projects = []
//        });
//    }

//    [Fact]
//    public async Task Handle_WhenProjectDoesNotExist_ShouldThrowEntityNotFoundException()
//    {
//        // Arrange
//        var command = _updateProjectCommandFaker.Generate();

//        _projectsProviderMock
//            .Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(null as Project);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should().ThrowAsync<EntityNotFoundException>();
//        _updateProjectDbQueryMock.Verify(
//            x => x.ExecuteAsync(It.IsAny<UpdateProjectDbRequest>(), _cancellationToken),
//            Times.Never);
//    }

//    [Fact]
//    public async Task Handle_WhenSettingProjectAsPublicAndBillable_ShouldThrowInvalidDataAppException()
//    {
//        // Arrange
//        var command = _updateProjectCommandFaker.Clone()
//            .RuleFor(x => x.IsPublic, _ => true)
//            .RuleFor(x => x.IsBillable, _ => true)
//            .Generate();

//        var project = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = true,
//            IsPublic = false,
//            Name = command.Name,
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(project);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should().ThrowAsync<InvalidDataAppException>()
//            .WithMessage(PortalMessages.ProjectCantBePublicAndBillable);
//        _updateProjectDbQueryMock.Verify(
//            x => x.ExecuteAsync(It.IsAny<UpdateProjectDbRequest>(), _cancellationToken),
//            Times.Never);
//    }

//    [Fact]
//    public async Task Handle_WhenHeadOfDepartmentTriesToUpdateInactiveProject_ShouldThrowInvalidDataAppException()
//    {
//        // Arrange
//        var command = _updateProjectCommandFaker.Generate();

//        var project = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = false,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = "MMS",
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(project);
//        _authenticatedUserContextMock.Setup(x => x.GetRoles())
//            .Returns([PortalRoleNames.HeadOfDepartment]);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should().ThrowAsync<InvalidDataAppException>()
//            .WithMessage(PortalMessages.YouCantUpdateInactiveProject);
//        _updateProjectDbQueryMock.Verify(
//            x => x.ExecuteAsync(It.IsAny<UpdateProjectDbRequest>(), _cancellationToken),
//            Times.Never);
//    }

//    [Fact]
//    public async Task Handle_WhenHeadOfDepartmentTriesToAssignProjectManager_ShouldThrowActionNotAllowedException()
//    {
//        // Arrange
//        var command = _updateProjectCommandFaker.Clone()
//            .RuleFor(x => x.ManagerId, 1)
//            .Generate();

//        var project = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = "MMS",
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(project);
//        _authenticatedUserContextMock.Setup(x => x.GetRoles())
//            .Returns([PortalRoleNames.HeadOfDepartment]);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should().ThrowAsync<ActionNotAllowedException>()
//            .WithMessage(PortalMessages.HeadOfDepartmentCannotAssignProjectManager);
//        _updateProjectDbQueryMock.Verify(
//            x => x.ExecuteAsync(It.IsAny<UpdateProjectDbRequest>(), _cancellationToken),
//            Times.Never);
//    }

//    [Fact]
//    public async Task Handle_WhenManagerIdProvided_AndCannotBeManager_ShouldThrowActionNotAllowedException()
//    {
//        // Arrange
//        const int managerId = 1;

//        var command = _updateProjectCommandFaker.Clone()
//            .RuleFor(x => x.ManagerId, _ => managerId)
//            .Generate();

//        var project = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = "MMS",
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        _projectsProviderMock
//            .Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(project);

//        _canUserBeProjectManagerBusinessRuleMock
//            .Setup(x => x.IsCorrespondsToBusiness(managerId, _cancellationToken))
//            .ReturnsAsync(false);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should()
//            .ThrowAsync<ActionNotAllowedException>()
//            .WithMessage(PortalMessages.UserCantBeManager);
//    }

//    [Fact]
//    public async Task Handle_WhenProjectWithSameNameAlreadyExists_ShouldThrowEntityConflictException()
//    {
//        // Arrange
//        var command = _updateProjectCommandFaker.Generate();

//        var projectToUpdate = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = "MMS",
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        var otherExistingProject = new Project
//        {
//            Id = command.Id + 5,
//            ClientId = command.ClientId + 1,
//            Color = "#999999",
//            ManagerId = null,
//            CreatedById = 84,
//            IsActive = true,
//            IsBillable = false,
//            IsPublic = true,
//            Name = command.Name,
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(projectToUpdate);
//        _projectsProviderMock.Setup(x => x.GetProjectByNameAsync(command.Name, _cancellationToken))
//            .ReturnsAsync(otherExistingProject);
//        _authenticatedUserContextMock.Setup(x => x.GetRoles())
//            .Returns([PortalRoleNames.Ceo]);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should().ThrowAsync<EntityConflictException>()
//            .WithMessage(PortalMessages.ProjectWithTheSameNameExists);
//        _updateProjectDbQueryMock.Verify(
//            x => x.ExecuteAsync(It.IsAny<UpdateProjectDbRequest>(), _cancellationToken),
//            Times.Never);
//    }

//    [Fact]
//    public async Task Handle_WhenUpdatingProjectClientButClientDoesNotExist_ShouldThrowEntityNotFoundException()
//    {
//        // Arrange
//        var command = _updateProjectCommandFaker.Generate();

//        var projectToUpdate = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = "MMS",
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(command.Id, _cancellationToken))
//            .ReturnsAsync(projectToUpdate);
//        _projectsProviderMock.Setup(x => x.GetProjectByNameAsync(command.Name, _cancellationToken))
//            .ReturnsAsync((Project?)null);
//        _authenticatedUserContextMock.Setup(x => x.GetRoles())
//            .Returns([PortalRoleNames.Ceo]);
//        _getClientForProjectUpdateByClientIdDbQueryMock.Setup(x => x.ExecuteAsync(command.ClientId, _cancellationToken))!
//            .ReturnsAsync((ClientForProjectUpdateResponse?)null);

//        // Act
//        var result = () => _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        await result.Should().ThrowAsync<EntityNotFoundException>()
//            .WithMessage(PortalMessages.ClientNotFound);
//        _updateProjectDbQueryMock.Verify(
//            x => x.ExecuteAsync(It.IsAny<UpdateProjectDbRequest>(), _cancellationToken),
//            Times.Never);
//    }

//    [Fact]
//    public async Task Handle_WhenCommandIsValid_ShouldUpdateProject()
//    {
//        // Arrange
//        const int currentUserId = 1;
//        const int trackedTime = 120;

//        var command = _updateProjectCommandFaker.Generate();

//        var projectToUpdate = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = command.ManagerId,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = command.Name,
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        var client = new Client
//        {
//            Id = command.ClientId,
//            IsActive = true,
//            Name = "Modsen",
//            Color = "#000000",
//            Comments = "modsen-software",
//            IsProjectExist = true,
//            FactTrackedTimeInMinutes = 0,
//            Projects = [command.Id]
//        };

//        var projectDto = new UpdateProjectDto
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ClientName = client.Name,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsCreatedByCurrentUser = currentUserId == projectToUpdate.CreatedById,
//            IsPublic = command.IsPublic,
//            Name = command.Name,
//            TrackedTime = trackedTime,
//            Color = command.Color,
//        };

//        var projectUsers = new List<ProjectUser>();
//        var requestUsers = new List<ProjectUser>();

//        var mappedProjectFromCommand = new Project
//        {
//            Id = command.Id,
//            ClientId = command.ClientId,
//            ManagerId = null,
//            Color = command.Color!,
//            CreatedById = 55,
//            IsActive = true,
//            IsBillable = command.IsBillable,
//            IsPublic = command.IsPublic,
//            Name = command.Name,
//            TechStack = null,
//            Industry = null,
//            CloudServices = null,
//            Architecture = null
//        };

//        var clientForProjectUpdate = new ClientForProjectUpdateResponse(client.IsActive, client.Name);

//        _projectsProviderMock.Setup(x => x.GetProjectByIdAsync(
//                command.Id,
//                _cancellationToken))
//            .ReturnsAsync(projectToUpdate);

//        _projectsProviderMock.Setup(x => x.GetProjectByNameAsync(
//                command.Name,
//                _cancellationToken))
//            .ReturnsAsync(null as Project);

//        _authenticatedUserContextMock.Setup(x => x.GetRoles())
//            .Returns([PortalRoleNames.HeadOfDepartment]);
//        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
//            .Returns("Java");
//        _authenticatedUserContextMock.Setup(x => x.UserId)
//            .Returns(currentUserId);

//        _getClientForProjectUpdateByClientIdDbQueryMock.Setup(x => x.ExecuteAsync(
//                command.ClientId,
//                _cancellationToken))
//            .ReturnsAsync(clientForProjectUpdate);

//        Expression<Func<UpdateProjectDbRequest, bool>> dbRequestMatch = dbRequest =>
//            dbRequest.ProjectId == projectToUpdate.Id
//            && dbRequest.ClientId == projectToUpdate.ClientId
//            && dbRequest.Color == projectToUpdate.Color
//            && dbRequest.IsBillable == projectToUpdate.IsBillable
//            && dbRequest.IsPublic == projectToUpdate.IsPublic
//            && dbRequest.ManagerId == projectToUpdate.ManagerId
//            && dbRequest.Name == projectToUpdate.Name;

//        _updateProjectDbQueryMock.Setup(x => x.ExecuteAsync(
//                It.Is(dbRequestMatch),
//                _cancellationToken))
//            .ReturnsAsync(mappedProjectFromCommand);

//        _timesheetProviderMock
//            .Setup(x => x.GetTrackedTimeInMinutesByProjectIdAsync(mappedProjectFromCommand.Id, _cancellationToken))
//            .ReturnsAsync(trackedTime);

//        // Act
//        var result = await _handler.Handle(
//            command,
//            _cancellationToken);

//        // Assert
//        result.Should().BeEquivalentTo(projectDto);
//    }
//}
