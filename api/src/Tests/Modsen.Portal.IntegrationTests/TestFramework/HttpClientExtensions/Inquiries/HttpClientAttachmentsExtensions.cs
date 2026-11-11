using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientAttachmentsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static async Task<UserInquiryFileOutput?> CreateAttachmentAsync(
        this HttpClient httpClient,
        int inquiryId,
        string fileId)
    {
        var createdResponse = await httpClient.PostAsJsonAsync(
            $"api/inquiries/{inquiryId}/attachments",
            new UserInquiryFileInput { FileId = fileId },
            _cancellationToken);

        return await createdResponse.Content.ReadFromJsonAsync<UserInquiryFileOutput>(
                _serializerOptions,
                _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateAttachmentAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId,
        string fileId)
    {
        return httpClient.PostAsJsonAsync(
            $"api/inquiries/{inquiryId}/attachments",
            new UserInquiryFileInput { FileId = fileId },
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteAttachmentAsHttpResponseAsync(
        this HttpClient httpClient,
        int inquiryId,
        int attachmentId)
    {
        return httpClient.DeleteAsync(
            $"api/inquiries/{inquiryId}/attachments/{attachmentId}",
            _cancellationToken);
    }

    public static Task<List<UserInquiryFileOutput>?> GetAttachmentsAsync(
        this HttpClient httpClient,
        int inquiryId)
    {
        return httpClient.GetFromJsonAsync<List<UserInquiryFileOutput>>(
                $"api/inquiries/{inquiryId}/attachments",
                _serializerOptions,
                _cancellationToken);
    }
}
