using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientCommentsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static async Task<UserInquiryCommentOutput?> CreateInquiryCommentAsync(
        this HttpClient httpClient,
        int inquiryId,
        string comment)
    {
        var createdResponse = await httpClient.PostAsJsonAsync(
            $"api/inquiries/{inquiryId}/comments",
            new UserInquiryCommentInput { Comment = comment },
            _cancellationToken);

        return await createdResponse.Content.ReadFromJsonAsync<UserInquiryCommentOutput>(
                _serializerOptions,
                _cancellationToken);
    }

    public static async Task<UserInquiryCommentOutput?> UpdateInquiryCommentAsync(
        this HttpClient httpClient,
        int inquiryId,
        int commentId,
        string comment)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"api/inquiries/{inquiryId}/comments/{commentId}",
            new UserInquiryCommentInput { Comment = comment },
            _cancellationToken);

        return await response.Content.ReadFromJsonAsync<UserInquiryCommentOutput>(
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateInquiryCommentAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId,
        int commentId,
        string comment)
    {
        return httpClient.PutAsJsonAsync(
            $"api/inquiries/{inquiryId}/comments/{commentId}",
            new UserInquiryCommentInput { Comment = comment },
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteInquiryCommentAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId,
        int commentId)
    {
        return httpClient.DeleteAsync(
            $"api/inquiries/{inquiryId}/comments/{commentId}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateInquiryCommentAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId,
        string comment)
    {
        return httpClient.PostAsJsonAsync(
            $"api/inquiries/{inquiryId}/comments",
            new UserInquiryCommentInput { Comment = comment },
            _cancellationToken);
    }

    public static Task<List<UserInquiryCommentOutput>?> GetInquiryCommentsAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.GetFromJsonAsync<List<UserInquiryCommentOutput>>(
            $"api/inquiries/{inquiryId}/comments",
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetInquiryCommentsAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.GetAsync(
            $"api/inquiries/{inquiryId}/comments",
            _cancellationToken);
    }
}
