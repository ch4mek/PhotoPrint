using System.Net.Http.Json;
using System.Text.Json;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

internal static class HttpClientRequestsExtensions
{
    public static Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync<TRequest, TResponse>(
            url, request, IntegrationTestsConstants.DefaultSerializerOptions, cancellationToken);
    }

    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest request,
        JsonSerializerOptions options,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(url, request, options, cancellationToken);

        return await ReadResponseFromJson<TResponse>(response, cancellationToken);
    }

    public static async Task<HttpResponseMessage> DeleteFromJsonAsync<TRequest>(
        this HttpClient client,
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage();

        message.Content = JsonContent.Create(request, options: IntegrationTestsConstants.DefaultSerializerOptions);
        message.Method = HttpMethod.Delete;
        message.RequestUri = new Uri(client.BaseAddress!, url);

        return await client.SendAsync(message, cancellationToken);
    }

    public static async Task<TResponse?> DeleteAsync<TResponse>(
        this HttpClient client,
        string url,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage();

        message.Method = HttpMethod.Delete;
        message.RequestUri = new Uri(client.BaseAddress!, url);

        var response = await client.SendAsync(message, cancellationToken);

        return await ReadResponseFromJson<TResponse>(response, cancellationToken);
    }

    public static async Task<TResponse?> DeleteFromJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteFromJsonAsync(
            url,
            request,
            cancellationToken);

        return await ReadResponseFromJson<TResponse>(response, cancellationToken);
    }

    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(
            url,
            request,
            IntegrationTestsConstants.DefaultSerializerOptions,
            cancellationToken);

        return await ReadResponseFromJson<TResponse>(response, cancellationToken);
    }

    private static async Task<TResponse?> ReadResponseFromJson<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var stringResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new ApplicationException(stringResponse);
        }

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            IntegrationTestsConstants.DefaultSerializerOptions,
            cancellationToken);

        return result;
    }
}
