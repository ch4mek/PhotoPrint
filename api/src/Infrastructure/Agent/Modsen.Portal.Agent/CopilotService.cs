using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Agent.Contract.Models;
using Modsen.Portal.Agent.Dto;

namespace Modsen.Portal.Agent;

public sealed class CopilotService : ICopilotService
{
    private const string DirectlineHttpClientName = "Directline";
    private const string DirectlineRegionalHttpClientName = "DirectlineRegional";

    private readonly string _apiVersion;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly TimeSpan _pollTimeout;
    private readonly TimeSpan _pollInterval;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public CopilotService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpFactory = httpFactory;
        _config = config;

        _apiVersion = _config.GetValue<string>("CopilotAgent:ApiVersion")
            ?? throw new InvalidOperationException("CopilotAgent:ApiVersion is not configured.");

        _pollTimeout = TimeSpan.FromSeconds(
            _config.GetValue("CopilotAgent:PollTimeoutSeconds", 30));

        _pollInterval = TimeSpan.FromMilliseconds(
            _config.GetValue("CopilotAgent:PollIntervalMilliseconds", 800));
    }

    public async Task<(ConversationSession Session, IReadOnlyList<string> Messages)> StartNewConversationAsync(
        Dictionary<string, object?>? userData, int userId, CancellationToken cancellationToken)
    {
        var httpClient = GetClient(DirectlineHttpClientName);

        var secret = _config.GetValue<string?>("CopilotAgent:Secret");
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("CopilotAgent:Secret is not configured.");
        }

        var regionalUrl = await TryGetRegionalDirectLineUrlAsync(cancellationToken)
            ?? throw new InvalidOperationException("Direct Line did not return regional url.");

        using var startReq = new HttpRequestMessage(HttpMethod.Post, $"{regionalUrl}/conversations");
        startReq.Headers.Authorization = GetAuthHeader(secret);

        using var startResp = await httpClient.SendAsync(startReq, cancellationToken);

        await EnsureOkAsync(startResp, "start conversation", cancellationToken);

        var conv = await startResp.Content.ReadFromJsonAsync<DirectLineConversationDto>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Direct Line returned an empty response when creating a dialog.");

        if (string.IsNullOrWhiteSpace(conv.ConversationId) || string.IsNullOrWhiteSpace(conv.Token))
        {
            throw new InvalidOperationException("Direct Line did not return a conversationId or token.");
        }

        var session = new ConversationSession
        {
            ConversationId = conv.ConversationId,
            Token = conv.Token,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(conv.ExpiresIn ?? 0)
        };

        var messages = await SendStartConversationEvent(session, userData, userId, regionalUrl, cancellationToken);

        return (session, messages);
    }

    private async Task<IReadOnlyList<string>> SendStartConversationEvent(
        ConversationSession session,
        Dictionary<string, object?>? userData, int userId, string regionalUrl,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);

        object activity = userData is not null
            ? new
            {
                type = "event",
                name = "startConversation",
                from = new { id = "dhr_" + userId },
                value = new
                {
                    UserInternalInfo = JsonSerializer.Serialize(userData)
                }
            }
            : new
            {
                type = "event",
                name = "startConversation",
                from = new { id = "dhr_" + userId }
            };

        await SendActivityAsync(regionalUrl, session, activity, cancellationToken);

        return await CollectRepliesAsync(regionalUrl, session, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> SendMessageAsync(
        ConversationSession session, int userId, string text, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);

        var regionalUrl = await TryGetRegionalDirectLineUrlAsync(cancellationToken)
            ?? throw new InvalidOperationException("Direct Line did not return regional url.");

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Message text cannot be empty.", nameof(text));
        }

        await SendActivityAsync(
            regionalUrl,
            session,
            new { type = "message", from = new { id = "dhr_" + userId }, text },
            cancellationToken);

        return await CollectRepliesAsync(regionalUrl, session, userId, cancellationToken);
    }

    public async Task<RefreshOrResumeConversationDto> RefreshOrResumeConversationAsync(
        ConversationSession? existingSession,
        Dictionary<string, object?>? userData, int userId,
        CancellationToken cancellationToken)
    {
        async Task<RefreshOrResumeConversationDto> StartNewAsync()
        {
            var (newSession, messages) = await StartNewConversationAsync(userData, userId, cancellationToken);
            return new RefreshOrResumeConversationDto(newSession, true, messages);
        }

        if (existingSession is null)
        {
            return await StartNewAsync();
        }

        var regionalUrl = await TryGetRegionalDirectLineUrlAsync(cancellationToken)
            ?? throw new InvalidOperationException("Direct Line did not return regional url.");

        if (existingSession.ExpiresAtUtc <= DateTimeOffset.UtcNow.AddMinutes(3))
        {
            var refreshStatus = await TryRefreshTokenAsync(existingSession, regionalUrl, cancellationToken);

            if (refreshStatus != DirectLineStatus.Success)
            {
                return await StartNewAsync();
            }
        }

        var reconnectStatus = await TryReconnectAsync(existingSession, regionalUrl, cancellationToken);

        if (reconnectStatus == DirectLineStatus.Success)
        {
            return new RefreshOrResumeConversationDto(existingSession, false);
        }

        if (reconnectStatus == DirectLineStatus.TokenExpired)
        {
            var refreshStatus = await TryRefreshTokenAsync(existingSession, regionalUrl, cancellationToken);

            if (refreshStatus == DirectLineStatus.Success)
            {
                reconnectStatus = await TryReconnectAsync(existingSession, regionalUrl, cancellationToken);

                if (reconnectStatus == DirectLineStatus.Success)
                {
                    return new RefreshOrResumeConversationDto(existingSession, false);
                }
            }
        }

        return await StartNewAsync();
    }

    private HttpClient GetClient(string httpClientName)
    {
        return _httpFactory.CreateClient(httpClientName);
    }

    private async Task SendActivityAsync<T>(
        string regionalUrl,
        ConversationSession session,
        T activity,
        CancellationToken cancellationToken)
    {
        var httpClient = GetClient(DirectlineRegionalHttpClientName);

        var json = JsonSerializer.Serialize(activity, JsonOptions);

        using var request = new HttpRequestMessage(
            HttpMethod.Post, $"{regionalUrl}/conversations/{session.ConversationId}/activities")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = GetAuthHeader(session.Token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        await EnsureOkAsync(response, "send a message", cancellationToken);
    }

    private async Task<IReadOnlyList<string>> CollectRepliesAsync(
        string regionalUrl,
        ConversationSession session, int userId,
        CancellationToken cancellationToken)
    {
        var messages = new List<string>();
        var deadline = DateTimeOffset.UtcNow.Add(_pollTimeout);
        var sawReply = false;

        var httpClient = GetClient(DirectlineRegionalHttpClientName);

        while (DateTimeOffset.UtcNow < deadline)
        {
            await Task.Delay(_pollInterval, cancellationToken);

            var url = $"{regionalUrl}/conversations/{session.ConversationId}/activities";

            if (!string.IsNullOrWhiteSpace(session.Watermark))
            {
                url += $"?watermark={Uri.EscapeDataString(session.Watermark)}";
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = GetAuthHeader(session.Token);

            using var response = await httpClient.SendAsync(request, cancellationToken);

            await EnsureOkAsync(response, "get an answer", cancellationToken);

            var set = await response.Content.ReadFromJsonAsync<ActivitySetDto>(
                JsonOptions,
                cancellationToken);

            if (set?.Watermark is not null)
            {
                session.Watermark = set.Watermark;
            }
            var botMessages = (set?.Activities ?? [])
                .Where(activity =>
                    activity.Type == "message" &&
                    activity.From?.Id != ("dhr_" + userId) &&
                    !string.IsNullOrWhiteSpace(activity.Text))
                .ToList();

            if (botMessages.Count > 0)
            {
                foreach (var activity in botMessages)
                {
                    messages.Add(activity.Text!);
                }

                sawReply = true;
            }
            else if (sawReply)
            {
                break;
            }
        }

        return messages;
    }

    private async Task<DirectLineStatus> TryRefreshTokenAsync(
        ConversationSession session, string regionalUrl,
        CancellationToken cancellationToken)
    {
        var httpClient = GetClient(DirectlineRegionalHttpClientName);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{regionalUrl}/tokens/refresh");
        request.Headers.Authorization = GetAuthHeader(session.Token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        var status = await GetDirectLineStatusAsync(response, "refresh the token", cancellationToken);

        if (status != DirectLineStatus.Success)
        {
            return status;
        }

        var refreshed = await response.Content.ReadFromJsonAsync<DirectLineConversationDto>(
            JsonOptions,
            cancellationToken);

        if (refreshed is null ||
            string.IsNullOrWhiteSpace(refreshed.Token) ||
            refreshed.ExpiresIn is null)
        {
            throw new InvalidOperationException("Direct Line refresh response is empty or invalid.");
        }

        session.Token = refreshed.Token;
        session.ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(refreshed.ExpiresIn.Value);

        return DirectLineStatus.Success;
    }

    private async Task<DirectLineStatus> TryReconnectAsync(
        ConversationSession session, string regionalUrl,
        CancellationToken cancellationToken)
    {
        var httpClient = GetClient(DirectlineRegionalHttpClientName);

        var url = string.IsNullOrWhiteSpace(session.Watermark)
            ? $"{regionalUrl}/conversations/{session.ConversationId}"
            : $"{regionalUrl}/conversations/{session.ConversationId}?watermark={Uri.EscapeDataString(session.Watermark)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = GetAuthHeader(session.Token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        var status = await GetDirectLineStatusAsync(response, "reconnect to conversation", cancellationToken);

        if (status != DirectLineStatus.Success)
        {
            return status;
        }

        var reconnect = await response.Content.ReadFromJsonAsync<DirectLineReconnectDto>(JsonOptions, cancellationToken);

        if (reconnect is null || string.IsNullOrWhiteSpace(reconnect.Token))
        {
            throw new InvalidOperationException("Direct Line reconnect response is empty or invalid.");
        }

        session.Token = reconnect.Token;

        return DirectLineStatus.Success;
    }

    private static async Task EnsureOkAsync(
        HttpResponseMessage response,
        string action,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        throw new InvalidOperationException(
            $"Direct Line: failed to {action} (HTTP {(int)response.StatusCode}). {body}");
    }

    private static async Task<DirectLineStatus> GetDirectLineStatusAsync(
        HttpResponseMessage response, string action, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return DirectLineStatus.Success;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            return DirectLineStatus.TokenExpired;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return DirectLineStatus.NotFound;
        }

        throw new InvalidOperationException(
            $"Direct Line: failed to {action} (HTTP {(int)response.StatusCode}). {body} ");
    }

    private async Task<string?> TryGetRegionalDirectLineUrlAsync(
        CancellationToken cancellationToken)
    {
        var httpClient = GetClient(DirectlineHttpClientName);

        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"powervirtualagents/regionalchannelsettings?api-version={Uri.EscapeDataString(_apiVersion)}");
        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var settings = await response.Content.ReadFromJsonAsync<RegionalChannelSettingsDto>(
            JsonOptions,
            cancellationToken);

        if (settings?.ChannelUrlsById is null ||
            !settings.ChannelUrlsById.TryGetValue("directline", out var directLineRootUrl) ||
            string.IsNullOrWhiteSpace(directLineRootUrl))
        {
            return null;
        }

        return new Uri(new Uri(directLineRootUrl), "v3/directline")
            .ToString()
            .TrimEnd('/');
    }

    private static AuthenticationHeaderValue GetAuthHeader(string key)
    {
        return new AuthenticationHeaderValue("Bearer", key);
    }

    private enum DirectLineStatus
    {
        Success,
        TokenExpired,
        NotFound
    }

    private sealed class RegionalChannelSettingsDto
    {
        [JsonRequired]
        public Dictionary<string, string>? ChannelUrlsById { get; set; }
    }
}
