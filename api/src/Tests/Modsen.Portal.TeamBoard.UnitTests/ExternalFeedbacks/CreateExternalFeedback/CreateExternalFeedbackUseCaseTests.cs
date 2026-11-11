using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ExternalFeedbacks.CreateExternalFeedback;

public class CreateExternalFeedbackUseCaseTests
{
    private readonly Mock<IIsTeamBoardUserExistWithAccessControlDbQuery> _isTeamBoardUserExistingByUserIdDbQueryMock = new();
    private readonly Mock<IIsConversationExistingDbQuery> _isConversationExistingDbQueryMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
    private readonly Mock<IIsUserHavingConversationBusinessRule> _isUserHavingConversationBusinessRuleMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
    private readonly CreateExternalFeedbackRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateExternalFeedbackUseCaseTests()
    {
        _handler = new CreateExternalFeedbackRequest(_isTeamBoardUserExistingByUserIdDbQueryMock.Object,
            _isConversationExistingDbQueryMock.Object,
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object,
            _isUserHavingConversationBusinessRuleMock.Object,
            _teamboardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenProjectDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateExternalFeedbackRequest.Command(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: Guid.CreateVersion7().ToString(), ActionsTodo: null, Reviewer: null,
            ConversationId: null, ProjectId: int.MaxValue, UserId: 5);

        _teamboardDbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new()
                {
                    Id = command.ProjectId,
                    IsActive = false,
                    ProjectName = "test", ClientName = "test",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateExternalFeedbackRequest.Command(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: Guid.CreateVersion7().ToString(), ActionsTodo: null, Reviewer: null,
            ConversationId: null, ProjectId: 5, UserId: int.MaxValue);

        _teamboardDbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new()
                {
                    Id = command.ProjectId,
                    IsActive = true,
                    ProjectName = "test", ClientName = "test",
                }
            }.BuildMockDbSet().Object);

        _isTeamBoardUserExistingByUserIdDbQueryMock.Setup(x => x.ExecuteAsync(command.UserId, false, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenConversationDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateExternalFeedbackRequest.Command(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: Guid.CreateVersion7().ToString(), ActionsTodo: null, Reviewer: null,
            ConversationId: int.MaxValue, ProjectId: 5, UserId: 5);

        _teamboardDbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new()
                {
                    Id = command.ProjectId,
                    IsActive = true,
                    ProjectName = "test", ClientName = "test",
                }
            }.BuildMockDbSet().Object);

        _isTeamBoardUserExistingByUserIdDbQueryMock.Setup(x => x.ExecuteAsync(command.UserId, false, _cancellationToken))
            .ReturnsAsync(true);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(false);

        _teamboardDbContextMock.Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = command.UserId,
                    ProjectId = command.ProjectId
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenConversationDoesNotBelongToUserThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateExternalFeedbackRequest.Command(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: Guid.CreateVersion7().ToString(), ActionsTodo: null, Reviewer: null,
            ConversationId: 1, ProjectId: 5, UserId: 5);

        _teamboardDbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new()
                {
                    Id = command.ProjectId,
                    IsActive = true,
                    ProjectName = "test", ClientName = "test",
                }
            }.BuildMockDbSet().Object);

        _isTeamBoardUserExistingByUserIdDbQueryMock.Setup(x => x.ExecuteAsync(command.UserId, false, _cancellationToken))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = command.UserId,
                    ProjectId = command.ProjectId
                }
            }.BuildMockDbSet().Object);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(true);

        _isUserHavingConversationBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new IsUserHavingConversationModel(command.UserId, command.ConversationId!.Value), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("x"));

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenUserCannotBeRetrievedThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateExternalFeedbackRequest.Command(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: Guid.CreateVersion7().ToString(), ActionsTodo: null, Reviewer: null,
            ConversationId: 1, ProjectId: 5, UserId: 5);

        _teamboardDbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new()
                {
                    Id = command.ProjectId,
                    IsActive = true,
                    ProjectName = "test", ClientName = "test",
                }
            }.BuildMockDbSet().Object);

        _isTeamBoardUserExistingByUserIdDbQueryMock.Setup(x => x.ExecuteAsync(command.UserId, false, _cancellationToken))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = command.UserId,
                    ProjectId = command.ProjectId
                }
            }.BuildMockDbSet().Object);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(true);

        _isUserHavingConversationBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new IsUserHavingConversationModel(command.UserId, command.ConversationId!.Value), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canTeamBoardUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                command.UserId, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidReturnCreatedExternalFeedback()
    {
        // Arrange
        var command = new CreateExternalFeedbackRequest.Command(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: Guid.CreateVersion7().ToString(), ActionsTodo: null, Reviewer: null,
            ConversationId: 1, ProjectId: 5, UserId: 5);

        _teamboardDbContextMock.Setup(x => x.TeamBoardProjects)
            .Returns(new List<TeamBoardProject>
            {
                new()
                {
                    Id = command.ProjectId,
                    IsActive = true,
                    ProjectName = "test", ClientName = "test",
                }
            }.BuildMockDbSet().Object);

        _isTeamBoardUserExistingByUserIdDbQueryMock.Setup(x => x.ExecuteAsync(command.UserId, false, _cancellationToken))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.ProjectUsers)
            .Returns(new List<ProjectUser>
            {
                new()
                {
                    UserId = command.UserId,
                    ProjectId = command.ProjectId
                }
            }.BuildMockDbSet().Object);

        _teamboardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
            }.BuildMockDbSet().Object);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(true);

        _isUserHavingConversationBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new IsUserHavingConversationModel(command.UserId, command.ConversationId!.Value), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _canTeamBoardUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                command.UserId, Portal.Core.AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.ExternalFeedbackDate.Should().Be(command.ExternalFeedbackDate);
        result.ExternalFeedbackType.Should().Be(command.ExternalFeedbackType);
        result.Feedback.Should().BeEquivalentTo(command.Feedback);
        result.ActionsTodo.Should().BeEquivalentTo(command.ActionsTodo);
        result.Reviewer.Should().BeEquivalentTo(command.Reviewer);
        result.ConversationId.Should().Be(command.ConversationId);
        result.ProjectId.Should().Be(command.ProjectId);
        result.UserId.Should().Be(command.UserId);
    }
}
