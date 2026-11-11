using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Notification.UseCases;

namespace Modsen.Portal.Notification.Controllers;

internal class MicrosoftTeamsBotApiService : IMicrosoftTeamsBotApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MicrosoftTeamsBotApiService> _logger;
    private const string HttpClientName = "MicrosoftTeamsBotApi";

    public MicrosoftTeamsBotApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<MicrosoftTeamsBotApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SendNotificationAsync(string jsonModel, CancellationToken cancellationToken = default)
    {
        using var httpClient = _httpClientFactory.CreateClient(HttpClientName);
        using var request = new HttpRequestMessage(HttpMethod.Post, "")
        {
            Content = new StringContent(jsonModel, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to send Teams notification. StatusCode: {StatusCode}", response.StatusCode);
        }
    }
}
