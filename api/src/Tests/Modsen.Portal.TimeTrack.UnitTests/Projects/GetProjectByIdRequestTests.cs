using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class GetProjectByIdRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<ICanUserGetProjectBusinessRule> _canUserGetProjectBusinessRuleMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetProjectByIdRequest _handler;

    public GetProjectByIdRequestTests()
    {
        _handler = new GetProjectByIdRequest(
            _timeTrackDbContextMock.Object,
            _canUserGetProjectBusinessRuleMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenProjectDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var request = new GetProjectByIdRequest.Query(55);

        SetupCanUserGetProject(BusinessRuleResult.Success());
        SetupProjects([]);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ProjectNotFound);
    }

    [Fact]
    public async Task HandleUserCannotGetProjectByBusinessRuleShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int projectId = 55;

        var project = new Project
        {
            Id = projectId,
            Name = "Test project",
            ClientId = 1,
            CreatedById = 1,
            Color = "#000000",
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
        };

        const string failReason = "Test reason";

        SetupCanUserGetProject(BusinessRuleResult.Fail(failReason));
        SetupProjects([project]);

        // Act
        var result = () => _handler.Handle(
            new GetProjectByIdRequest.Query(projectId),
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(failReason);
    }

    [Fact]
    public async Task HandleWhenDataIsValidShouldReturnProject()
    {
        // Arrange
        const int projectId = 55;

        var project = new Project
        {
            Id = projectId,
            Name = "Test project",
            ClientId = 1,
            CreatedById = 1,
            Color = "#000000",
            IsActive = true,
            IsBillable = true,
            IsPublic = false,
        };

        var expectedProject = new ProjectWithManagerDto
        {
            Id = project.Id,
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = project.IsActive,
            IsBillable = project.IsBillable,
            IsSelfCreatedProject = false,
            IsPublic = project.IsPublic,
            Name = project.Name,
            ManagerFullName = null,
        };

        SetupCanUserGetProject(BusinessRuleResult.Success());
        SetupProjects([project]);

        // Act
        var result = await _handler.Handle(
            new GetProjectByIdRequest.Query(projectId),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProject);
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupCanUserGetProject(BusinessRuleResult result)
    {
        _canUserGetProjectBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(result);
    }
}
