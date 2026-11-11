using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Agent.Contract.Models;

namespace Modsen.Portal.Agent.UnitTests;

public sealed class CopilotServiceTests
{
    private const string DirectlineClientName = "Directline";
    private const string DirectlineRegionalClientName = "DirectlineRegional";

    [Fact]
    public void ConstructorWhenApiVersionMissingThrowsInvalidOperationException()
    {
       // Arrange
       var factory = new FakeHttpClientFactory();
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["CopilotAgent:Secret"] = "secret"
        });

        // Act
        var act = () => new CopilotService(factory, config);

        // Assert
        var ex = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("CopilotAgent:ApiVersion is not configured.", ex.Message);
        Assert.True(true);
    }

    [Fact]
    public async Task StartNewConversationAsyncWhenSecretMissingThrowsInvalidOperationException()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["CopilotAgent:ApiVersion"] = "2022-03-01-preview"
        });

        var sut = new CopilotService(factory, config);

        // Act
        var act = () => sut.StartNewConversationAsync(null, 123, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("CopilotAgent:Secret is not configured.", ex.Message);
    }

    [Fact]
    public async Task StartNewConversationAsyncWhenRegionalUrlMissingThrowsInvalidOperationException()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.InternalServerError,
            "{}");

        var config = BuildDefaultConfig();
        var sut = new CopilotService(factory, config);

        // Act
        var act = () => sut.StartNewConversationAsync(null, 123, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Equal("Direct Line did not return regional url.", ex.Message);
    }

    [Fact]
    public async Task StartNewConversationAsyncWhenSuccessfulReturnsSessionAndBotMessages()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "conversationId": "conversation-1",
              "token": "conversation-token",
              "expiresIn": 1800
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "id": "activity-1"
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "watermark": "1",
              "activities": [
                {
                  "type": "message",
                  "from": {
                    "id": "bot"
                  },
                  "text": "Hello from bot"
                }
              ]
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "watermark": "2",
              "activities": []
            }
            """);

        var config = BuildDefaultConfig();
        var sut = new CopilotService(factory, config);

        // Act
        var result = await sut.StartNewConversationAsync(
            new Dictionary<string, object?>
            {
                ["FirstName"] = "John",
                ["Department"] = "IT"
            },
            userId: 123,
            CancellationToken.None);

        // Assert
        Assert.Equal("conversation-1", result.Session.ConversationId);
        Assert.Equal("conversation-token", result.Session.Token);
        Assert.Equal("2", result.Session.Watermark);

        Assert.Single(result.Messages);
        Assert.Equal("Hello from bot", result.Messages[0]);

        var requests = factory.Requests;

        Assert.Contains(requests, r =>
            r.ClientName == DirectlineClientName &&
            r.Method == HttpMethod.Get &&
            r.Url.Contains("powervirtualagents/regionalchannelsettings", StringComparison.Ordinal));

        Assert.Contains(requests, r =>
            r.ClientName == DirectlineClientName &&
            r.Method == HttpMethod.Post &&
            r.Url == "https://directline.example.com/v3/directline/conversations" &&
            r.Authorization == "Bearer secret");

        var startEventRequest = requests.Single(r =>
            r.ClientName == DirectlineRegionalClientName &&
            r.Method == HttpMethod.Post &&
            r.Url == "https://directline.example.com/v3/directline/conversations/conversation-1/activities");

        Assert.Equal("Bearer conversation-token", startEventRequest.Authorization);
        Assert.Contains("\"type\":\"event\"", startEventRequest.Body, StringComparison.Ordinal);
        Assert.Contains("\"name\":\"startConversation\"", startEventRequest.Body, StringComparison.Ordinal);
        Assert.Contains("\"id\":\"dhr_123\"", startEventRequest.Body, StringComparison.Ordinal);
        Assert.Contains("UserInternalInfo", startEventRequest.Body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendMessageAsyncWhenTextIsEmptyThrowsArgumentException()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        var sut = new CopilotService(factory, BuildDefaultConfig());

        var session = new ConversationSession
        {
            ConversationId = "conversation-1",
            Token = "token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        // Act
        var act = () => sut.SendMessageAsync(session, 123, "   ", CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("text", ex.ParamName);
    }

    [Fact]
    public async Task SendMessageAsyncWhenSuccessfulSendsMessageAndReturnsBotReplies()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "id": "activity-1"
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "watermark": "10",
              "activities": [
                {
                  "type": "message",
                  "from": {
                    "id": "dhr_123"
                  },
                  "text": "User echo should be ignored"
                },
                {
                  "type": "message",
                  "from": {
                    "id": "bot"
                  },
                  "text": "Bot answer"
                },
                {
                  "type": "event",
                  "from": {
                    "id": "bot"
                  },
                  "text": "Event should be ignored"
                }
              ]
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "watermark": "11",
              "activities": []
            }
            """);

        var sut = new CopilotService(
            factory,
            BuildDefaultConfig());

        var session = new ConversationSession
        {
            ConversationId = "conversation-1",
            Token = "token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        // Act
        var messages = await sut.SendMessageAsync(session, 123, "Hi", CancellationToken.None);

        // Assert
        Assert.Single(messages);
        Assert.Equal("Bot answer", messages[0]);
        Assert.Equal("11", session.Watermark);

        var post = factory.Requests.Single(r =>
            r.ClientName == DirectlineRegionalClientName &&
            r.Method == HttpMethod.Post);

        Assert.Equal(
            "https://directline.example.com/v3/directline/conversations/conversation-1/activities",
            post.Url);

        Assert.Equal("Bearer token", post.Authorization);
        Assert.Contains("\"type\":\"message\"", post.Body, StringComparison.Ordinal);
        Assert.Contains("\"text\":\"Hi\"", post.Body, StringComparison.Ordinal);
        Assert.Contains("\"id\":\"dhr_123\"", post.Body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SendMessageAsyncWhenPreviousWatermarkExistsUsesWatermarkInPollingUrl()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            "{}");

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "watermark": "6",
              "activities": [
                {
                  "type": "message",
                  "from": {
                    "id": "bot"
                  },
                  "text": "Reply"
                }
              ]
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "watermark": "7",
              "activities": []
            }
            """);

        var sut = new CopilotService(
            factory,
            BuildDefaultConfig());

        var session = new ConversationSession
        {
            ConversationId = "conversation-1",
            Token = "token",
            Watermark = "5",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        // Act
        await sut.SendMessageAsync(session, 123, "Hello", CancellationToken.None);

        // Assert
        Assert.Contains(factory.Requests, r =>
            r.ClientName == DirectlineRegionalClientName &&
            r.Method == HttpMethod.Get &&
            r.Url == "https://directline.example.com/v3/directline/conversations/conversation-1/activities?watermark=5");
    }

    [Fact]
    public async Task RefreshOrResumeConversationAsyncWhenReconnectSuccessfulResumesExistingSession()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "conversationId": "conversation-1",
              "token": "reconnected-token"
            }
            """);

        var sut = new CopilotService(factory, BuildDefaultConfig());

        var session = new ConversationSession
        {
            ConversationId = "conversation-1",
            Token = "old-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(20)
        };

        // Act
        var result = await sut.RefreshOrResumeConversationAsync(
            session,
            userData: null,
            userId: 123,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsNewConversation);
        Assert.Same(session, result.ConversationSession);
        Assert.Equal("reconnected-token", session.Token);

        Assert.Contains(factory.Requests, r =>
            r.ClientName == DirectlineRegionalClientName &&
            r.Method == HttpMethod.Get &&
            r.Url == "https://directline.example.com/v3/directline/conversations/conversation-1" &&
            r.Authorization == "Bearer old-token");
    }

    [Fact]
    public async Task RefreshOrResumeConversationAsyncWhenSessionExpiresSoonRefreshesTokenThenReconnects()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "token": "refreshed-token",
              "expires_in": 1800
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "conversationId": "conversation-1",
              "token": "reconnected-token"
            }
            """);

        var sut = new CopilotService(factory, BuildDefaultConfig());

        var session = new ConversationSession
        {
            ConversationId = "conversation-1",
            Token = "old-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(1)
        };

        // Act
        var result = await sut.RefreshOrResumeConversationAsync(
            session,
            userData: null,
            userId: 123,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsNewConversation);
        Assert.Same(session, result.ConversationSession);
        Assert.Equal("reconnected-token", session.Token);
        Assert.True(session.ExpiresAtUtc > DateTimeOffset.UtcNow.AddMinutes(20));

        Assert.Contains(factory.Requests, r =>
            r.ClientName == DirectlineRegionalClientName &&
            r.Method == HttpMethod.Post &&
            r.Url == "https://directline.example.com/v3/directline/tokens/refresh" &&
            r.Authorization == "Bearer old-token");

        Assert.Contains(factory.Requests, r =>
            r.ClientName == DirectlineRegionalClientName &&
            r.Method == HttpMethod.Get &&
            r.Url == "https://directline.example.com/v3/directline/conversations/conversation-1" &&
            r.Authorization == "Bearer refreshed-token");
    }

    [Fact]
    public async Task RefreshOrResumeConversationAsyncWhenReconnectTokenExpiredRefreshesAndReconnectsAgain()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.Forbidden,
            "Token expired");

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "token": "refreshed-token",
              "expires_in": 1800
            }
            """);

        factory.AddResponse(
            DirectlineRegionalClientName,
            HttpStatusCode.OK,
            """
            {
              "conversationId": "conversation-1",
              "token": "reconnected-token"
            }
            """);

        var sut = new CopilotService(factory, BuildDefaultConfig());

        var session = new ConversationSession
        {
            ConversationId = "conversation-1",
            Token = "expired-token",
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(20)
        };

        // Act
        var result = await sut.RefreshOrResumeConversationAsync(
            session,
            userData: null,
            userId: 123,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsNewConversation);
        Assert.Equal("reconnected-token", session.Token);

        var reconnectRequests = factory.Requests
            .Where(r =>
                r.ClientName == DirectlineRegionalClientName &&
                r.Method == HttpMethod.Get &&
                r.Url == "https://directline.example.com/v3/directline/conversations/conversation-1")
            .ToList();

        Assert.Equal(2, reconnectRequests.Count);
        Assert.Equal("Bearer expired-token", reconnectRequests[0].Authorization);
        Assert.Equal("Bearer refreshed-token", reconnectRequests[1].Authorization);
    }

    [Fact]
    public async Task StartNewConversationAsyncWhenStartConversationFailsThrowsInvalidOperationException()
    {
        // Arrange
        var factory = new FakeHttpClientFactory();

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.OK,
            """
            {
              "channelUrlsById": {
                "directline": "https://directline.example.com/"
              }
            }
            """);

        factory.AddResponse(
            DirectlineClientName,
            HttpStatusCode.BadRequest,
            "Bad request body");

        var sut = new CopilotService(factory, BuildDefaultConfig());

        // Act
        var act = () => sut.StartNewConversationAsync(null, 123, CancellationToken.None);

        // Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.Contains("Direct Line: failed to start conversation", ex.Message, StringComparison.Ordinal);
        Assert.Contains("HTTP 400", ex.Message, StringComparison.Ordinal);
        Assert.Contains("Bad request body", ex.Message, StringComparison.Ordinal);
    }

    private static IConfiguration BuildDefaultConfig()
    {
        return BuildConfig(new Dictionary<string, string?>
        {
            ["CopilotAgent:ApiVersion"] = "2022-03-01-preview",
            ["CopilotAgent:Secret"] = "secret",
            ["CopilotAgent:PollTimeoutSeconds"] = "1",
            ["CopilotAgent:PollIntervalMilliseconds"] = "1"
        });
    }

    private static IConfiguration BuildConfig(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly Dictionary<string, Queue<FakeHttpResponse>> _responsesByClientName = new();

        public List<CapturedRequest> Requests { get; } = [];

        public void AddResponse(string clientName, HttpStatusCode statusCode, string body)
        {
            if (!_responsesByClientName.TryGetValue(clientName, out var queue))
            {
                queue = new Queue<FakeHttpResponse>();
                _responsesByClientName[clientName] = queue;
            }

            queue.Enqueue(new FakeHttpResponse(statusCode, body));
        }

        public HttpClient CreateClient(string name)
        {
#pragma warning disable CA2000
            var handler = new FakeHttpMessageHandler(name, Requests, _responsesByClientName);

            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://directline.base/")
            };
#pragma warning restore CA2000
        }
    }

    private sealed class FakeHttpMessageHandler(
        string clientName,
        List<CapturedRequest> requests,
        Dictionary<string, Queue<FakeHttpResponse>> responsesByClientName)
        : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var body = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            requests.Add(new CapturedRequest(
                ClientName: clientName,
                Method: request.Method,
                Url: request.RequestUri!.ToString(),
                Authorization: request.Headers.Authorization?.ToString(),
                Body: body));

            if (!responsesByClientName.TryGetValue(clientName, out var queue) || queue.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No fake HTTP response configured for client '{clientName}'. Request: {request.Method} {request.RequestUri}");
            }

            var response = queue.Dequeue();

            return new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(
                    response.Body,
                    Encoding.UTF8,
                    "application/json")
            };
        }
    }

    private sealed record FakeHttpResponse(
        HttpStatusCode StatusCode,
        string Body);

    private sealed record CapturedRequest(
        string ClientName,
        HttpMethod Method,
        string Url,
        string? Authorization,
        string? Body);
}


