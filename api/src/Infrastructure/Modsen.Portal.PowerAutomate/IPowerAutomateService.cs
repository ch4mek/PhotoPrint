using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.PowerAutomate;

public interface IPowerAutomateService
{
    Task<string> AskQuestion(string email, string question, CancellationToken cancellationToken);
}

public sealed class PowerAutomateService : IPowerAutomateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PowerAutomateService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> AskQuestion(string email, string question, CancellationToken cancellationToken)
    {
        var client = GetHttpClient("PowerAutomateClient");
        var powerAutomateKey = _configuration.GetConnectionString("PowerAutomateHeaderKey");
        var url = _configuration.GetConnectionString("PowerAutomateUrl");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);

        httpRequest.Content = new StringContent(JsonSerializer.Serialize(new
        {
            Question = question,
            Email = email,
            Key = powerAutomateKey
        }), Encoding.UTF8, "application/json");

        var result = await client.SendAsync(httpRequest, cancellationToken);
        var response = await result.Content.ReadFromJsonAsync<PowerAutomateResponse>(cancellationToken);
        if ((response?.Answer).IsNullOrEmpty())
        {
            return string.Empty;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(response.Answer);

        return doc.DocumentNode.InnerText.Trim();

    }

    private HttpClient GetHttpClient(string httpClientName)
    {
        return _httpClientFactory.CreateClient(httpClientName);
    }
}

internal sealed record PowerAutomateResponse
{
    public string? Answer { get; init; }
}
