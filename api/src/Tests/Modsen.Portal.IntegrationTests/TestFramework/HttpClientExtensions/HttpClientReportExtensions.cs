namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;

internal static class HttpClientReportExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetReportAsync(
        this HttpClient httpClient,
        string format,
        int year,
        int userId)
    {
        return httpClient.GetAsync(
            new Uri($"api/Reports/{format}?year={year}&userId={userId}", UriKind.Relative),
            _cancellationToken);
    }
}
