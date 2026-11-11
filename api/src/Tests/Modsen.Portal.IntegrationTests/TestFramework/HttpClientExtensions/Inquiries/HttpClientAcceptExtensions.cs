using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientAcceptExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static Task<List<UserInquiryAcceptOutput>> GetInquiryAcceptsAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.GetFromJsonAsync<List<UserInquiryAcceptOutput>>(
            $"api/inquiries/{inquiryId}/accepts",
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetInquiryAcceptsAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.GetAsync(
            $"api/inquiries/{inquiryId}/accepts",
            _cancellationToken);
    }

    public static async Task<List<UserInquiryAcceptOutput>?> AcceptInquiryAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        var response = await httpClient.PostAsync(
            $"api/inquiries/{inquiryId}/accept",
            null,
            _cancellationToken);

        return await response.Content.ReadFromJsonAsync<List<UserInquiryAcceptOutput>>(
            _serializerOptions,
            _cancellationToken);

    }

    public static async Task<List<UserInquiryAcceptOutput>?> RejectInquiryAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        var response = await httpClient.PostAsync(
            $"api/inquiries/{inquiryId}/reject",
            null,
            _cancellationToken);

        return await response.Content.ReadFromJsonAsync<List<UserInquiryAcceptOutput>>(
            _serializerOptions,
            _cancellationToken);

    }

    public static Task<HttpResponseMessage> AcceptInquiryAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.PostAsync(
            $"api/inquiries/{inquiryId}/accept", null, _cancellationToken);
    }

    public static Task<HttpResponseMessage> RejectInquiryAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.PostAsync(
            $"api/inquiries/{inquiryId}/reject",
            null,
            _cancellationToken);
    }

    public static Task<UserInquiryOutput?> GetInquiryByIdAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.GetFromJsonAsync<UserInquiryOutput>(
            $"api/inquiries/{inquiryId}",
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<InquiryCoreDto>> GetPageableInquiriesAsync(
        this HttpClient httpClient, string? orderBy = null, string? filter = null, int top = 5, int skip = 0)
    {
        var listClause = new List<string>()
        {
            $"top={top}",
            $"skip={skip}"
        };

        if (orderBy != null)
        {
            listClause.Add($"$orderby={orderBy}");
        }

        if (filter != null)
        {
            listClause.Add($"$filter={filter}");
        }

        var clause = string.Join('&', listClause);

        return httpClient.GetFromJsonAsync<PageableListOfItems<InquiryCoreDto>>(
            $"api/inquiries?{clause}", _serializerOptions, _cancellationToken)!;
    }
}
