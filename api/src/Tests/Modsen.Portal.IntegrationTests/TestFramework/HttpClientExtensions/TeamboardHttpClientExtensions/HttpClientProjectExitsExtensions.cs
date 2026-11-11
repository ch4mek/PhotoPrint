namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class HttpClientProjectExitsExtensions
{
    public static Task<HttpResponseMessage> GetPageableProjectExitsAsHttpResponseAsync(this HttpClient client,
        string query, CancellationToken cancellationToken)
    {
        return client.GetAsync($"api/project-exits?{query}", cancellationToken);
    }
}
