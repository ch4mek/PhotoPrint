using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Agent.Contract.Models;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Messages.Core.Composers;
using Modsen.Portal.Identity.Requests.Messages.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Messages.Core.Loader;
using Modsen.Portal.Identity.Requests.Messages.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Messages;

public sealed class GetChatHistoryRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserMessageLoader> _userMessageLoaderMock = new();
    private readonly Mock<ICopilotService> _copilotServiceMock = new();
    private readonly Mock<IUserDataComposer> _userDataComposerMock = new();

    [Fact]
    public async Task HandleShouldCreateNewConversationAndSaveGreetingMessagesWhenConversationDoesNotExist()
    {
        // Arrange
        var userId = 10;
        var email = "ivan.petrov@test.com";
        var cancellationToken = CancellationToken.None;

        var sessions = new List<DigitalHrConversationSession>();
        var userMessages = new List<UserMessage>();

        var userData = new Dictionary<string, object?>
        {
            ["FirstName"] = "Ivan",
            ["LastName"] = "Petrov"
        };

        var newSession = new ConversationSession
        {
            ConversationId = "new-conversation-id",
            Token = "new-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "new-watermark"
        };

        var greetingMessages = new List<string>{"Hello!"};

        var queryOptions = default(ODataQueryOptions<MessageCoreDto>)!;
        var expectedHistoryResult = default(PageableListOfItems<MessageCoreDto>)!;

        SetupUser(userId, email);
        SetupDbSets(sessions, userMessages);

        _userDataComposerMock
            .Setup(x => x.ComposeAsync(email, cancellationToken))
            .ReturnsAsync(userData);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                null,
                userData,
                userId,
                cancellationToken))
            .ReturnsAsync(new RefreshOrResumeConversationDto(
                newSession,
                true,
                greetingMessages));

        _userMessageLoaderMock
            .Setup(x => x.LoadAsync(userId, queryOptions, cancellationToken))
            .ReturnsAsync(expectedHistoryResult);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetChatHistoryRequest.Command(queryOptions),
            cancellationToken);

        // Assert
        result.Should().BeSameAs(expectedHistoryResult);

        sessions.Should().ContainSingle();

        var savedSession = sessions.Single();
        savedSession.UserId.Should().Be(userId);
        savedSession.ConversationId.Should().Be(newSession.ConversationId);
        savedSession.Token.Should().Be(newSession.Token);
        savedSession.ExpiresAtUtc.Should().Be(newSession.ExpiresAtUtc);
        savedSession.Watermark.Should().Be(newSession.Watermark);

        userMessages.Should().HaveCount(1);

        userMessages[0].UserId.Should().Be(userId);
        userMessages[0].Message.Should().Be("Hello!");
        userMessages[0].Role.Should().Be(MessageRole.Assistant);

        _identityDbContextMock.Verify(
            x => x.SaveChanges(cancellationToken),
            Times.Once);

        _userDataComposerMock.Verify(
            x => x.ComposeAsync(email, cancellationToken),
            Times.Once);

        _copilotServiceMock.Verify(
            x => x.RefreshOrResumeConversationAsync(null, userData, userId, cancellationToken),
            Times.Once);

        _userMessageLoaderMock.Verify(
            x => x.LoadAsync(userId, queryOptions, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleShouldNotCreateNewConversationWhenExistingConversationExists()
    {
        // Arrange
        var userId = 10;
        var email = "ivan.petrov@test.com";
        var cancellationToken = CancellationToken.None;

        var existingSessionEntity = new DigitalHrConversationSession
        {
            UserId = userId,
            ConversationId = "existing-conversation-id",
            Token = "existing-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            Watermark = "existing-watermark",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-5)
        };

        var sessions = new List<DigitalHrConversationSession>
        {
            existingSessionEntity
        };

        var userMessages = new List<UserMessage>();

        var userData = new Dictionary<string, object?>
        {
            ["FirstName"] = "Ivan"
        };

        var resumedSession = new ConversationSession
        {
            ConversationId = "existing-conversation-id",
            Token = "refreshed-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "refreshed-watermark"
        };

        var queryOptions = default(ODataQueryOptions<MessageCoreDto>)!;
        var expectedHistoryResult = default(PageableListOfItems<MessageCoreDto>)!;

        SetupUser(userId, email);
        SetupDbSets(sessions, userMessages);

        _userDataComposerMock
            .Setup(x => x.ComposeAsync(email, cancellationToken))
            .ReturnsAsync(userData);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                It.Is<ConversationSession>(s =>
                    s.ConversationId == existingSessionEntity.ConversationId &&
                    s.Token == existingSessionEntity.Token &&
                    s.ExpiresAtUtc == existingSessionEntity.ExpiresAtUtc &&
                    s.Watermark == existingSessionEntity.Watermark),
                userData,
                userId,
                cancellationToken))
            .ReturnsAsync(new RefreshOrResumeConversationDto(
                resumedSession,
                false));

        _userMessageLoaderMock
            .Setup(x => x.LoadAsync(userId, queryOptions, cancellationToken))
            .ReturnsAsync(expectedHistoryResult);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetChatHistoryRequest.Command(queryOptions),
            cancellationToken);

        // Assert
        result.Should().BeSameAs(expectedHistoryResult);

        sessions.Should().ContainSingle();
        userMessages.Should().BeEmpty();

        _identityDbContextMock.Verify(
            x => x.DigitalHrConversationSessions.Add(It.IsAny<DigitalHrConversationSession>()),
            Times.Never);

        _identityDbContextMock.Verify(
            x => x.UserMessages.AddRange(It.IsAny<IEnumerable<UserMessage>>()),
            Times.Never);

        _identityDbContextMock.Verify(
            x => x.SaveChanges(It.IsAny<CancellationToken>()),
            Times.Never);

        _userMessageLoaderMock.Verify(
            x => x.LoadAsync(userId, queryOptions, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleShouldSaveConversationOnlyWhenNewConversationHasNoGreetingMessages()
    {
        // Arrange
        var userId = 10;
        var email = "ivan.petrov@test.com";
        var cancellationToken = CancellationToken.None;

        var sessions = new List<DigitalHrConversationSession>();
        var userMessages = new List<UserMessage>();

        var userData = new Dictionary<string, object?>
        {
            ["FirstName"] = "Ivan"
        };

        var newSession = new ConversationSession
        {
            ConversationId = "new-conversation-id",
            Token = "new-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30),
            Watermark = "new-watermark"
        };

        var queryOptions = default(ODataQueryOptions<MessageCoreDto>)!;
        var expectedHistoryResult = default(PageableListOfItems<MessageCoreDto>)!;

        SetupUser(userId, email);
        SetupDbSets(sessions, userMessages);

        _userDataComposerMock
            .Setup(x => x.ComposeAsync(email, cancellationToken))
            .ReturnsAsync(userData);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                null,
                userData,
                userId,
                cancellationToken))
            .ReturnsAsync(new RefreshOrResumeConversationDto(
                newSession,
                true,
                []));

        _userMessageLoaderMock
            .Setup(x => x.LoadAsync(userId, queryOptions, cancellationToken))
            .ReturnsAsync(expectedHistoryResult);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(
            new GetChatHistoryRequest.Command(queryOptions),
            cancellationToken);

        // Assert
        result.Should().BeSameAs(expectedHistoryResult);

        sessions.Should().ContainSingle();
        userMessages.Should().BeEmpty();

        _identityDbContextMock.Verify(
            x => x.SaveChanges(cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleShouldUseLatestConversationSessionWhenUserHasMultipleSessions()
    {
        // Arrange
        var userId = 10;
        var email = "ivan.petrov@test.com";
        var cancellationToken = CancellationToken.None;

        var oldSession = new DigitalHrConversationSession
        {
            UserId = userId,
            ConversationId = "old-conversation-id",
            Token = "old-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            Watermark = "old-watermark",
            CreatedAtUtc = DateTime.UtcNow.AddHours(-2)
        };

        var latestSession = new DigitalHrConversationSession
        {
            UserId = userId,
            ConversationId = "latest-conversation-id",
            Token = "latest-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            Watermark = "latest-watermark",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-1)
        };

        var sessions = new List<DigitalHrConversationSession>
        {
            oldSession,
            latestSession
        };

        var userMessages = new List<UserMessage>();

        var userData = new Dictionary<string, object?>
        {
            ["FirstName"] = "Ivan"
        };

        var queryOptions = default(ODataQueryOptions<MessageCoreDto>)!;
        var expectedHistoryResult = default(PageableListOfItems<MessageCoreDto>)!;

        SetupUser(userId, email);
        SetupDbSets(sessions, userMessages);

        _userDataComposerMock
            .Setup(x => x.ComposeAsync(email, cancellationToken))
            .ReturnsAsync(userData);

        _copilotServiceMock
            .Setup(x => x.RefreshOrResumeConversationAsync(
                It.Is<ConversationSession>(s =>
                    s.ConversationId == latestSession.ConversationId &&
                    s.Token == latestSession.Token &&
                    s.Watermark == latestSession.Watermark),
                userData,
                userId,
                cancellationToken))
            .ReturnsAsync(new RefreshOrResumeConversationDto(
                new ConversationSession
                {
                    ConversationId = latestSession.ConversationId,
                    Token = latestSession.Token,
                    ExpiresAtUtc = latestSession.ExpiresAtUtc,
                    Watermark = latestSession.Watermark
                },
                false));

        _userMessageLoaderMock
            .Setup(x => x.LoadAsync(userId, queryOptions, cancellationToken))
            .ReturnsAsync(expectedHistoryResult);

        var handler = CreateHandler();

        // Act
        await handler.Handle(
            new GetChatHistoryRequest.Command(queryOptions),
            cancellationToken);

        // Assert
        _copilotServiceMock.Verify(
            x => x.RefreshOrResumeConversationAsync(
                It.Is<ConversationSession>(s =>
                    s.ConversationId == latestSession.ConversationId),
                userData,
                userId,
                cancellationToken),
            Times.Once);
    }

    private GetChatHistoryRequest CreateHandler()
    {
        return new GetChatHistoryRequest(
            _authenticatedUserContextMock.Object,
            _identityDbContextMock.Object,
            _userMessageLoaderMock.Object,
            _copilotServiceMock.Object,
            _userDataComposerMock.Object);
    }

    private void SetupUser(int userId, string email)
    {
        _authenticatedUserContextMock
            .Setup(x => x.UserEmail)
            .Returns(email);

        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(userId);
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
