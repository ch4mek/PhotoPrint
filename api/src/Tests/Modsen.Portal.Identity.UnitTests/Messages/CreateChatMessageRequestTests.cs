using MockQueryable.Moq;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Agent.Contract.Models;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Messages.Core.Composers;
using Modsen.Portal.Identity.Requests.Messages.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Messages;

public sealed class CreateChatMessageRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICopilotService> _copilotServiceMock = new();
    private readonly Mock<IUserDataComposer> _userDataComposerMock = new();

    [Fact]
    public async Task HandleShouldCreateNewConversationSessionWhenSessionDoesNotExist()
    {
        // Arrange
        var userId = 10;
        var email = "ivan.petrov@test.com";
        var question = "Hello";

        var sessions = new List<DigitalHrConversationSession>();
        var userMessages = new List<UserMessage>();

        var newSession = new ConversationSession
        {
            ConversationId = "conversation-new",
            Token = "token-new",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "wm-new"
        };

        var userData = new Dictionary<string, object?>
        {
            ["FirstName"] = "Ivan"
        };

        SetupUser(userId, email);
        SetupDbSets(sessions, userMessages);

        _userDataComposerMock
            .Setup(x => x.ComposeAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userData);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                null,
                userData,
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshOrResumeConversationDto(newSession, true));

        _copilotServiceMock
            .Setup(x => x.SendMessageAsync(
                newSession,
                userId,
                question,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(["Assistant answer"]);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new CreateChatMessageRequest.Command(question),
            CancellationToken.None);

        // Assert
        sessions.Should().ContainSingle();

        var savedSession = sessions.Single();
        savedSession.UserId.Should().Be(userId);
        savedSession.ConversationId.Should().Be("conversation-new");
        savedSession.Token.Should().Be("token-new");
        savedSession.ExpiresAtUtc.Should().Be(newSession.ExpiresAtUtc);
        savedSession.Watermark.Should().Be("wm-new");

        userMessages.Should().HaveCount(2);
        userMessages[0].UserId.Should().Be(userId);
        userMessages[0].Message.Should().Be(question);
        userMessages[0].Role.Should().Be(MessageRole.User);

        userMessages[1].UserId.Should().Be(userId);
        userMessages[1].Message.Should().Be("Assistant answer");
        userMessages[1].Role.Should().Be(MessageRole.Assistant);

        result.Should().HaveCount(2);

        _identityDbContextMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleShouldUseExistingConversationSessionAndUpdateIt()
    {
        // Arrange
        var userId = 10;
        var email = "ivan.petrov@test.com";
        var question = "Question";

        var existingEntity = new DigitalHrConversationSession
        {
            UserId = userId,
            ConversationId = "old-conversation",
            Token = "old-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(1),
            Watermark = "old-watermark",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10)
        };

        var sessions = new List<DigitalHrConversationSession>
        {
            existingEntity
        };

        var userMessages = new List<UserMessage>();

        var refreshedSession = new ConversationSession
        {
            ConversationId = "old-conversation",
            Token = "new-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "new-watermark"
        };

        var userData = new Dictionary<string, object?>
        {
            ["FirstName"] = "Ivan"
        };

        SetupUser(userId, email);
        SetupDbSets(sessions, userMessages);

        _userDataComposerMock
            .Setup(x => x.ComposeAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userData);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                It.Is<ConversationSession>(s =>
                    s.ConversationId == "old-conversation" &&
                    s.Token == "old-token" &&
                    s.Watermark == "old-watermark"),
                userData,
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshOrResumeConversationDto(refreshedSession, false));

        _copilotServiceMock
            .Setup(x => x.SendMessageAsync(
                refreshedSession,
                userId,
                question,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(["Answer"]);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new CreateChatMessageRequest.Command(question),
            CancellationToken.None);

        // Assert
        sessions.Should().ContainSingle();

        existingEntity.ConversationId.Should().Be("old-conversation");
        existingEntity.Token.Should().Be("new-token");
        existingEntity.ExpiresAtUtc.Should().Be(refreshedSession.ExpiresAtUtc);
        existingEntity.Watermark.Should().Be("new-watermark");

        userMessages.Should().HaveCount(2);
        userMessages[0].Message.Should().Be(question);
        userMessages[0].Role.Should().Be(MessageRole.User);

        userMessages[1].Message.Should().Be("Answer");
        userMessages[1].Role.Should().Be(MessageRole.Assistant);

        _identityDbContextMock.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleShouldSaveFallbackMessageWhenCopilotReturnsEmptyResponse()
    {
        // Arrange
        var userId = 10;
        var question = "Question";

        var sessions = new List<DigitalHrConversationSession>();
        var userMessages = new List<UserMessage>();

        var session = new ConversationSession
        {
            ConversationId = "conversation-id",
            Token = "token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "watermark"
        };

        SetupUser(userId, email: null);
        SetupDbSets(sessions, userMessages);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                null,
                null,
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshOrResumeConversationDto(session, true));

        _copilotServiceMock
            .Setup(x => x.SendMessageAsync(
                session,
                userId,
                question,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new CreateChatMessageRequest.Command(question),
            CancellationToken.None);

        // Assert
        userMessages.Should().HaveCount(2);

        userMessages[0].Message.Should().Be(question);
        userMessages[0].Role.Should().Be(MessageRole.User);

        userMessages[1].Message.Should().Be(PortalMessages.DigitalHrEmptyMessage);
        userMessages[1].Role.Should().Be(MessageRole.Assistant);

        _userDataComposerMock.Verify(
            x => x.ComposeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleShouldNotComposeUserDataWhenEmailIsNull()
    {
        // Arrange
        var userId = 10;
        var question = "Hello";

        var sessions = new List<DigitalHrConversationSession>();
        var userMessages = new List<UserMessage>();

        var session = new ConversationSession
        {
            ConversationId = "conversation-id",
            Token = "token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "watermark"
        };

        SetupUser(userId, email: null);
        SetupDbSets(sessions, userMessages);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                null,
                null,
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefreshOrResumeConversationDto(session, true));

        _copilotServiceMock
            .Setup(x => x.SendMessageAsync(
                session,
                userId,
                question,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(["Answer"]);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new CreateChatMessageRequest.Command(question),
            CancellationToken.None);

        // Assert
        _userDataComposerMock.Verify(
            x => x.ComposeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void ValidatorShouldReturnErrorWhenQuestionIsEmpty()
    {
        // Arrange
        var validator = new CreateChatMessageRequest.Validator();
        var command = new CreateChatMessageRequest.Command("");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == nameof(CreateChatMessageRequest.Command.Question));
    }

    [Fact]
    public void ValidatorShouldReturnSuccessWhenQuestionIsValid()
    {
        // Arrange
        var validator = new CreateChatMessageRequest.Validator();
        var command = new CreateChatMessageRequest.Command("Hello");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    private CreateChatMessageRequest CreateHandler()
    {
        return new CreateChatMessageRequest(
            _authenticatedUserContextMock.Object,
            _identityDbContextMock.Object,
            _copilotServiceMock.Object,
            _userDataComposerMock.Object);
    }

    private void SetupUser(int userId, string? email)
    {
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _authenticatedUserContextMock
            .Setup(x => x.UserEmail)
            .Returns(email);
    }

    private void SetupDbSets(
        List<DigitalHrConversationSession> sessions,
        List<UserMessage> userMessages)
    {
        _identityDbContextMock
            .Setup(x => x.DigitalHrConversationSessions)
            .Returns(sessions.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.UserMessages)
            .Returns(userMessages.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.DigitalHrConversationSessions.Add(It.IsAny<DigitalHrConversationSession>()))
            .Callback<DigitalHrConversationSession>(sessions.Add);

        _identityDbContextMock
            .Setup(x => x.UserMessages.AddRange(It.IsAny<IEnumerable<UserMessage>>()))
            .Callback<IEnumerable<UserMessage>>(messages => userMessages.AddRange(messages));

        _identityDbContextMock
            .Setup(x => x.SaveChanges(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }
}
