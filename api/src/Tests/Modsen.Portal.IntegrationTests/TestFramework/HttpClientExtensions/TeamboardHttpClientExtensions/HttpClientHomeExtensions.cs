namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class HttpClientHomeExtension
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetUpcomingEvents(this HttpClient client, int userId, int overdue)
    {
        return client.GetAsync($"api/home/{userId}/events?overdueDaysCount={overdue}", _cancellationToken);
    }
}
