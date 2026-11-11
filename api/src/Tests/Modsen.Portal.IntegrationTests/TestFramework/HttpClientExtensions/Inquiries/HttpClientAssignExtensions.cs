using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientAssignExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<InquiriesAssignsDto> CreateInquiryAssignAsync(this HttpClient client,
        CreateInquiryAssignInput input, JsonSerializerOptions serializerOptions)
    {
        return client.PostAsJsonAsync<CreateInquiryAssignInput, InquiriesAssignsDto>(
            "api/inquiries/assigns",
            input,
            serializerOptions,
            _cancellationToken)!;
    }

    public static Task<HttpResponseMessage> GetAssignsByTemplateIdAsHttpResponseAsync(this HttpClient client,
        int templateId, int top, int skip)
    {
        return client.GetAsync($"api/templates/{templateId}/assigns/search?top={top}&skip={skip}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetAssignsByUserIdAsHttpResponseAsync(this HttpClient client, int userId)
    {
        return client.GetAsync($"api/users/{userId}/inquiries/assigns", _cancellationToken);
    }

    public static Task<List<UserAssignRequestDto>> GetAssignsByUserIdAsync(this HttpClient client, int userId)
    {
        return client.GetFromJsonAsync<List<UserAssignRequestDto>>($"api/users/{userId}/inquiries/assigns",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken)!;
    }
}
