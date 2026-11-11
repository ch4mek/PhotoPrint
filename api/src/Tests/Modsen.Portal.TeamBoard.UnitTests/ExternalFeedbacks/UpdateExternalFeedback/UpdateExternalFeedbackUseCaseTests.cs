using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ExternalFeedbacks.UpdateExternalFeedback;

public class UpdateExternalFeedbackUseCaseTests
{
    private readonly Mock<IIsConversationExistingDbQuery> _isConversationExistingDbQueryMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
    private readonly Mock<IIsUserHavingConversationBusinessRule> _isUserHavingConversationBusinessRuleMock = new();
    private readonly Mock<ITeamBoardDbContext> _iTeamBoardDbContextMock = new();
    private readonly UpdateExternalFeedbackRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateExternalFeedbackUseCaseTests()
    {
        _handler = new UpdateExternalFeedbackRequest(
            _isConversationExistingDbQueryMock.Object,
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object,
            _isUserHavingConversationBusinessRuleMock.Object,
            _iTeamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenExternalFeedbackDoesNotExistThrowEntityNotFoundException()
    {
        // Assert
        var command = new UpdateExternalFeedbackRequest.Command(Id: int.MaxValue, ExternalFeedbackType.Normal,
            ActionsTodo: null, ConversationId: null, Reviewer: null);
        _iTeamBoardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
            }.BuildMockDbSet().Object);
        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserCannotBeRetrievedThrowActionNotAllowedException()
    {
        // Arrange
        var userId = 5;
        // Assert
        var command = new UpdateExternalFeedbackRequest.Command(Id: 1, ExternalFeedbackType.Normal,
            ActionsTodo: null, ConversationId: null, Reviewer: null);

        _canTeamBoardUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                userId, _cancellationToken))
            .ReturnsAsync(false);

        _iTeamBoardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
                new()
                {
                    Id = command.Id,
                    UserId = userId,
                    ConversationId = command.ConversationId,
                    Feedback = "test",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenUserIsInternThrowActionNotAllowedException()
    {
        // Arrange
        var user = new TeamBoardUser
        {
            UserId = 1,
            Email = "intern@gmail.com",
            FirstName = "test",
            LastName = "test",
            Position = PortalRoleNames.Intern
        };

        var command = new UpdateExternalFeedbackRequest.Command(Id: 1, ExternalFeedbackType.Normal,
            ActionsTodo: null, ConversationId: null, Reviewer: null);

        _iTeamBoardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
                new()
                {
                    Id = command.Id,
                    UserId = user.UserId,
                    User = user,
                    ConversationId = command.ConversationId,
                    Feedback = "test",
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.NotAllowedOperationWithIntern);
    }

    [Fact]
    public async Task HandleWhenConversationDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var command = new UpdateExternalFeedbackRequest.Command(Id: 1, ExternalFeedbackType.Normal,
            ActionsTodo: null, ConversationId: 1, Reviewer: null);
        var userId = 5;
        _canTeamBoardUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                userId, Portal.Core.AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(false);

        _iTeamBoardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
                new()
                {
                    Id = command.Id,
                    UserId = userId,Feedback = "test",
                    ConversationId = command.ConversationId
                }
            }.BuildMockDbSet().Object);
        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenConversationDoesNotBelongToUserThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateExternalFeedbackRequest.Command(Id: 1, ExternalFeedbackType.Normal,
            ActionsTodo: null, ConversationId: 1, Reviewer: null);
        var userId = 5;

        _canTeamBoardUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                userId, Portal.Core.AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(true);

        _isUserHavingConversationBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new IsUserHavingConversationModel(userId, command.ConversationId!.Value),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("X"));

        _iTeamBoardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
                new()
                {
                    Id = command.Id,
                    UserId = userId,Feedback = "test",
                    ConversationId = command.ConversationId
                }
            }.BuildMockDbSet().Object);
        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidReturnUpdatedExternalFeedback()
    {
        // Arrange
        var command = new UpdateExternalFeedbackRequest.Command(Id: 1, ExternalFeedbackType.Normal,
            ActionsTodo: null, ConversationId: 1, Reviewer: null);
        var userId = 5;

        _canTeamBoardUserBeRetrievedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                userId, Portal.Core.AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _isConversationExistingDbQueryMock.Setup(x => x.ExecuteAsync(command.ConversationId!.Value, _cancellationToken))
            .ReturnsAsync(true);

        _isUserHavingConversationBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                new IsUserHavingConversationModel(userId, command.ConversationId!.Value),
                _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _iTeamBoardDbContextMock.Setup(x => x.ExternalFeedbacks)
            .Returns(new List<ExternalFeedback>
            {
                new()
                {
                    Id = command.Id,
                    UserId = userId,Feedback = "test",
                    ConversationId = command.ConversationId
                }
            }.BuildMockDbSet().Object);

        // Act
        var result =  await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Id.Should().Be(command.Id);
        result.ActionsTodo.Should().BeEquivalentTo(command.ActionsTodo);
        result.Reviewer.Should().BeEquivalentTo(command.Reviewer);
        result.ConversationId.Should().Be(command.ConversationId);
    }
}
