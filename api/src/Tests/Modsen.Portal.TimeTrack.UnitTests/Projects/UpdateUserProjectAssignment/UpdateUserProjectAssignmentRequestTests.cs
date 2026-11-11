using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects.UpdateUserProjectAssignment;

public class UpdateUserProjectAssignmentRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<ICanTimeTrackUserBeRetrievedBusinessRule> _canTimeTrackUserBeRetrievedBusinessRuleMock = new();
    private readonly Mock<ICanUserProjectBeCreatedBusinessRule> _canUserProjectBeCreatedBusinessRuleMock = new();
    private readonly Mock<ICanUserProjectBeDecreasedBusinessRule> _canUserProjectBeDecreasedBusinessRuleMock = new();
    private readonly Mock<ICommunicationBus> _outboxContractMock = new();
    private readonly Mock<ICanUserProjectAssignmentBeUpdatedBusinessRule> _canUserProjectAssignmentBeUpdatedBusinessRule = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();
    private readonly UpdateUserProjectAssignmentRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateUserProjectAssignmentRequestTests()
    {
        _handler = new UpdateUserProjectAssignmentRequest(
            _timeTrackDbContextMock.Object,
            _canTimeTrackUserBeRetrievedBusinessRuleMock.Object,
            _canUserProjectBeCreatedBusinessRuleMock.Object,
            _canUserProjectBeDecreasedBusinessRuleMock.Object,
            _outboxContractMock.Object,
            _canUserProjectAssignmentBeUpdatedBusinessRule.Object,
            _teamBoardContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenProjectIsNotFoundShouldThrowEntityNotFound()
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(int.MaxValue,
            UserId: 0,
            ProjectId: 0,
            IsTimeOnProjectBillable: true,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null);

        SetupProjects([]);
        SetupProjectUsers([]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenThereAreNoRightsShouldThrowActionNotAllowed()
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(Id: 1,
            UserId: 1,
            ProjectId: 0,
            IsTimeOnProjectBillable: true,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null);

        var user = new TimeTrackUser()
        {
            UserId = command.UserId,
            FirstName = "test",
            LastName = "test",
            Email = "test"
        };

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "test"
        };

        var projectUser = new ProjectUser
        {
            Id = 1,
            ProjectId = command.ProjectId,
            UserId = command.UserId,
            User = user,
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupProjectUsers([projectUser]);
        SetupSeniorityLevelId(command.UserOnProjectSeniorityLevelId);

        _canTimeTrackUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new CanTimeTrackUserBeRetrievedBusinessRuleModel(projectUser.UserId), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("x"));

        _canUserProjectAssignmentBeUpdatedBusinessRule.Setup(mock => mock.IsCorrespondsToBusiness(
            It.IsAny<CanUserProjectAssignmentBeUpdatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenEndDateLessThanCurrentDateAndTryToUpdateItShouldReturnOk()
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(Id: 1,
            UserId: 1,
            ProjectId: 1,
            IsTimeOnProjectBillable: false,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
            UserOnProjectTo: null);

        var client = new Client()
        {
            Id = 1,
            Name = "Modsen"
        };

        var user = new TimeTrackUser()
        {
            UserId = command.UserId,
            FirstName = "Test",
            Email = "test",
            LastName = "Test"
        };

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "Proj",
            Client = client,
            IsActive = true
        };

        var projectUser = new ProjectUser
        {
            Id = 1,
            ProjectId = command.ProjectId,
            UserId = command.UserId,
            IsTimeOnProjectBillable = false,
            UserOnProjectFrom = DateOnly.FromDateTime(DateTime.Now).AddDays(-10),
            UserOnProjectTo = null,
            Project = project,
            User = user,
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupProjectUsers([projectUser]);
        SetupSeniorityLevelId(command.UserOnProjectSeniorityLevelId);

        _canTimeTrackUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanTimeTrackUserBeRetrievedBusinessRuleModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectBeCreatedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeCreatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());
        _canUserProjectBeDecreasedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeDecreasedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _canUserProjectAssignmentBeUpdatedBusinessRule.Setup(mock => mock.IsCorrespondsToBusiness(
            It.IsAny<CanUserProjectAssignmentBeUpdatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        // Act
        var result = await _handler.Handle(command, _cancellationToken);
        var expected = new UserProjectAssignmentDto
        {
            Id = 1,
            UserId = 1,
            ProjectId = 1,
            UserOnProjectFrom = projectUser.UserOnProjectFrom,
            UserOnProjectTo = projectUser.UserOnProjectTo,
            ProjectName = "Proj",
            ClientName = "Modsen",
            UserFullName = "Test Test",
            IsActive = true
        };
        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task HandleWhenIntervalsAreMatchingShouldThrowInvalidDataApp()
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(Id: 1,
            UserId: 1,
            ProjectId: 1,
            IsTimeOnProjectBillable: false,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-11)),
            UserOnProjectTo: null);

        var client = new Client()
        {
            Id = 1,
            Name = "Modsen"
        };

        var user = new TimeTrackUser()
        {
            UserId = command.UserId,
            FirstName = "Test",
            Email = "test",
            LastName = "Test"
        };

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "Proj",
            Client = client,
            IsActive = true
        };

        var projectUser = new ProjectUser
        {
            Id = 1,
            ProjectId = command.ProjectId,
            UserId = command.UserId,
            IsTimeOnProjectBillable = false,
            UserOnProjectFrom = DateOnly.FromDateTime(DateTime.Now).AddDays(-10),
            UserOnProjectTo = null,
            Project = project,
            User = user,
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupProjectUsers([projectUser]);
        SetupSeniorityLevelId(command.UserOnProjectSeniorityLevelId);

        _canTimeTrackUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanTimeTrackUserBeRetrievedBusinessRuleModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectBeCreatedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeCreatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("x"));

        _canUserProjectAssignmentBeUpdatedBusinessRule.Setup(mock => mock.IsCorrespondsToBusiness(
            It.IsAny<CanUserProjectAssignmentBeUpdatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenTimesheetsAreLostShouldThrowInvalidDataApp()
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(Id: 1,
            UserId: 0,
            ProjectId: 0,
            IsTimeOnProjectBillable: true,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: DateOnly.FromDateTime(DateTime.UtcNow));

        var client = new Client()
        {
            Id = 1,
            Name = "Modsen"
        };

        var user = new TimeTrackUser()
        {
            UserId = command.UserId,
            FirstName = "Test",
            Email = "test",
            LastName = "Test"
        };

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "Proj",
            Client = client,
            IsActive = true
        };

        var projectUser = new ProjectUser
        {
            Id = 1,
            ProjectId = command.ProjectId,
            UserId = command.UserId,
            IsTimeOnProjectBillable = false,
            UserOnProjectFrom = DateOnly.FromDateTime(DateTime.Now).AddDays(-10),
            UserOnProjectTo = null,
            Project = project,
            User = user,
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupProjectUsers([projectUser]);
        SetupSeniorityLevelId(command.UserOnProjectSeniorityLevelId);

        _canTimeTrackUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanTimeTrackUserBeRetrievedBusinessRuleModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectBeCreatedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeCreatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectBeDecreasedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeDecreasedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("x"));

        _canUserProjectAssignmentBeUpdatedBusinessRule.Setup(mock => mock.IsCorrespondsToBusiness(
            It.IsAny<CanUserProjectAssignmentBeUpdatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldUpdate()
    {
        // Arrange
        var command = new UpdateUserProjectAssignmentRequest.Command(Id: 1,
            UserId: 1,
            ProjectId: 1,
            IsTimeOnProjectBillable: false,
            Fte: 0.5m,
            UserOnProjectSeniorityLevelId: "MiddleL2",
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.Now).AddDays(-10),
            UserOnProjectTo: null);

        var client = new Client()
        {
            Id = 1,
            Name = "Modsen"
        };

        var user = new TimeTrackUser()
        {
            UserId = command.UserId,
            FirstName = "Test",
            Email = "test",
            LastName = "Test"
        };

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "Proj",
            Client = client,
            IsActive = true
        };

        var projectUser = new ProjectUser
        {
            Id = 1,
            ProjectId = command.ProjectId,
            UserId = command.UserId,
            IsTimeOnProjectBillable = false,
            UserOnProjectFrom = DateOnly.FromDateTime(DateTime.Now).AddDays(-10),
            UserOnProjectTo = null,
            Project = project,
            User = user,
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupProjectUsers([projectUser]);
        SetupSeniorityLevelId(command.UserOnProjectSeniorityLevelId);

        _canTimeTrackUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new CanTimeTrackUserBeRetrievedBusinessRuleModel(projectUser.UserId), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectBeCreatedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeCreatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectBeDecreasedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<CanUserProjectBeDecreasedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canUserProjectAssignmentBeUpdatedBusinessRule.Setup(mock => mock.IsCorrespondsToBusiness(
            It.IsAny<CanUserProjectAssignmentBeUpdatedModel>(), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        _outboxContractMock.Setup(x => x.PublishAsync(It.IsAny<ProjectUserUpdatedEvent>(),
            _cancellationToken));


        var expected = new UserProjectAssignmentDto
        {
            Id = 1,
            UserId = 1,
            ProjectId = 1,
            UserOnProjectFrom = projectUser.UserOnProjectFrom,
            UserOnProjectTo = projectUser.UserOnProjectTo,
            ProjectName = "Proj",
            ClientName = "Modsen",
            UserFullName = "Test Test",
            IsActive = true
        };

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
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

    private void SetupUsers(List<TimeTrackUser> timeTrackUsers)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.TimeTrackUsers)
            .Returns(timeTrackUsers.BuildMockDbSet().Object);
    }

    private void SetupSeniorityLevelId(string levelId)
    {
        _teamBoardContractMock
            .Setup(mock => mock.GetSeniorityLevelByUserProjectId(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(new SeniorityLevelOutput {Id = levelId, Name = levelId});
    }
}
