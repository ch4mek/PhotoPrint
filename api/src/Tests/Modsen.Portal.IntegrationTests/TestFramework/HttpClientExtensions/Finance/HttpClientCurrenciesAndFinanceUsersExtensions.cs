namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;

internal static class HttpClientCurrenciesAndFinanceUsersExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;


    public static Task<HttpResponseMessage> FinanceUsersLookupAsResponseMessageAsync(this HttpClient httpClient,
        int year, int month)
    {
        return httpClient.GetAsync(
            $"api/finance-users/lookup/{year}/{month}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CurrenciesLookupAsResponseMessageAsync(this HttpClient httpClient)
    {
        return httpClient.GetAsync(
            $"api/currencies/lookup",
            _cancellationToken);
    }
}
