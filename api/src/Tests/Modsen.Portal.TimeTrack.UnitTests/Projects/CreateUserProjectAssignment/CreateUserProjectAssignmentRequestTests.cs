using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects.CreateUserProjectAssignment;

public class CreateUserProjectAssignmentRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<ICanTimeTrackUserBeRetrievedBusinessRule> _canTimeTrackUserBeRetrievedBusinessRuleMock = new();
    private readonly Mock<ICanUserProjectBeCreatedBusinessRule> _canUserProjectBeCreatedBusinessRuleMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();
    private readonly Mock<ICanUserGetProjectBusinessRule> _canUserGetProjectBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateUserProjectAssignmentRequest _handler;

    public CreateUserProjectAssignmentRequestTests()
    {
        _handler = new CreateUserProjectAssignmentRequest(
            _timeTrackDbContextMock.Object,
            _canTimeTrackUserBeRetrievedBusinessRuleMock.Object,
            _canUserProjectBeCreatedBusinessRuleMock.Object,
            _communicationBusMock.Object,
            _teamBoardContractMock.Object,
            _canUserGetProjectBusinessRuleMock.Object);
    }

    [Fact]
    public async Task CreateUserProjectAssignmentWhenUserDoesNotExistsShouldThrowEntityNotFound()
    {
        // Arrange
        var command = new CreateUserProjectAssignmentRequest.Command(UserId: 1, ProjectId: 1,
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null,
            IsTimeOnProjectBillable: true, Fte: 0.5m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "test"
        };

        SetupUsers([]);
        SetupProjects([project]);
        SetupCanTimeTrackUserBeRetrieved(BusinessRuleResult.Success());
        SetupCanUserProjectBeCreated(BusinessRuleResult.Success());

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task CreateUserProjectAssignmentWhenProjectDoesNotExistsShouldThrowEntityNotFound()
    {
        // Arrange
        var command = new CreateUserProjectAssignmentRequest.Command(UserId: 1, ProjectId: 1,
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null,
            IsTimeOnProjectBillable: true, Fte: 0.5m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var user = new TimeTrackUser
        {
            UserId = command.UserId,
            FirstName = "test",
            LastName = "test",
            Email = "test"
        };

        SetupUsers([user]);
        SetupProjects([]);
        SetupCanTimeTrackUserBeRetrieved(BusinessRuleResult.Success());
        SetupCanUserProjectBeCreated(BusinessRuleResult.Success());

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task CreateUserProjectAssignmentWhenNoRightsToRetrieveTimeTrackUserShouldThrowActionNotAllowed()
    {
        // Arrange
        var command = new CreateUserProjectAssignmentRequest.Command(UserId: 1, ProjectId: 1,
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null,
            IsTimeOnProjectBillable: true, Fte: 0.5m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "test"
        };

        var user = new TimeTrackUser
        {
            UserId = command.UserId,
            FirstName = "test",
            LastName = "test",
            Email = "test"
        };

        var seniorityLevels = new List<SeniorityLevelOutput>
        {
            new()
            {
                Id = command.UserOnProjectSeniorityLevelId,
                Name = "Name"
            }
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupSeniorityLevels(seniorityLevels);
        SetupCanTimeTrackUserBeRetrieved(BusinessRuleResult.Fail(""));
        SetupCanUserProjectBeCreated(BusinessRuleResult.Success());

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task CreateUserProjectAssignmentWhenIntervalsAreMatchingShouldThrowInvalidDataApp()
    {
        // Arrange
        var command = new CreateUserProjectAssignmentRequest.Command(UserId: 1, ProjectId: 1,
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null,
            IsTimeOnProjectBillable: true, Fte: 0.5m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var project = new Project
        {
            Id = command.ProjectId,
            Name = "test"
        };

        var user = new TimeTrackUser
        {
            UserId = command.UserId,
            FirstName = "test",
            LastName = "test",
            Email = "test"
        };

        var seniorityLevels = new List<SeniorityLevelOutput>
        {
            new()
            {
                Id = command.UserOnProjectSeniorityLevelId,
                Name = "Name"
            }
        };

        SetupUsers([user]);
        SetupProjects([project]);
        SetupSeniorityLevels(seniorityLevels);
        SetupCanTimeTrackUserBeRetrieved(BusinessRuleResult.Success());
        SetupCanUserProjectBeCreated(BusinessRuleResult.Fail(""));

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    private void SetupUsers(List<TimeTrackUser> timeTrackUsers)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.TimeTrackUsers)
            .Returns(timeTrackUsers.BuildMockDbSet().Object);
    }

    private void SetupProjects(List<Project> projects)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Projects)
            .Returns(projects.BuildMockDbSet().Object);
    }

    private void SetupCanTimeTrackUserBeRetrieved(BusinessRuleResult result)
    {
        _canTimeTrackUserBeRetrievedBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<CanTimeTrackUserBeRetrievedBusinessRuleModel>(), _cancellationToken))
            .ReturnsAsync(result);
    }

    private void SetupCanUserProjectBeCreated(BusinessRuleResult result)
    {
        _canUserProjectBeCreatedBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<CanUserProjectBeCreatedModel>(), _cancellationToken))
            .ReturnsAsync(result);
    }

    private void SetupSeniorityLevels(List<SeniorityLevelOutput> seniorityLevelOutputs)
    {
        _teamBoardContractMock
            .Setup(mock => mock.GetSeniorityLevels(It.IsAny<SeniorityLevelScopes>(), _cancellationToken))
            .ReturnsAsync(seniorityLevelOutputs);
    }
}
