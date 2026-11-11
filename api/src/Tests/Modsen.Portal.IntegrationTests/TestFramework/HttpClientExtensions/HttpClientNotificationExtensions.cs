using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Notification.Controllers.Contracts.Output;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;

internal static class HttpClientNotificationExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = CreateJsonSerializeOptions();

    public static Task<PageableListOfItems<OdpNotification>?> GetNotificationsAsync(this HttpClient client,
        int employeeId,
        int top,
        int skip)
    {
        return client.GetFromJsonAsync<PageableListOfItems<OdpNotification>>(
            $"api/users/{employeeId}/notifications/search?top={top}&skip={skip}", _jsonSerializerOptions, _cancellationToken);
    }

    public static Task<PageableListOfItems<OdpNotification>?> GetNotificationsAsync(this HttpClient client,
        int employeeId,
        int top,
        int skip,
        string filter)
    {
        return client.GetFromJsonAsync<PageableListOfItems<OdpNotification>>(
            $"api/users/{employeeId}/notifications/search?top={top}&skip={skip}&filter={filter}", _jsonSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> ReadNotificationsAsync(this HttpClient client,
        List<int> ids)
    {
        return client.PatchAsJsonAsync("api/Notifications/read", ids, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<bool> GetUnreadPreviewNotificationsAsync(
        this HttpClient httpClient,
        int employeeUserId)
    {
        return httpClient.GetFromJsonAsync<bool>(
            $"api/users/{employeeUserId}/notifications/unread/preview", _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteNotificationsByIdAsync(this HttpClient httpClient, int id)
    {
        return httpClient.DeleteAsync($"api/notifications/{id}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteNotificationsByUserIdAsync(this HttpClient httpClient, int employeeUserId)
    {
        return httpClient.DeleteAsync($"api/users/{employeeUserId}/notifications", _cancellationToken);
    }

    public static Task<DeleteNotificationsOutput?> DeleteNotificationsByUserIdAsOutputAsync(this HttpClient httpClient, int employeeUserId)
    {
        return httpClient.DeleteAsync<DeleteNotificationsOutput>($"api/users/{employeeUserId}/notifications", _cancellationToken);
    }

    private static JsonSerializerOptions CreateJsonSerializeOptions()
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        return jsonSerializerOptions;
    }
}
