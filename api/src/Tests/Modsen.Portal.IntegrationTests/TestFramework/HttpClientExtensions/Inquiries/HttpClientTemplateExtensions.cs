using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientTemplateExtensions
{
    private static readonly CancellationToken _cancellationToken;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static Task<HttpResponseMessage> GetTemplatesAsHttpResponseAsync(this HttpClient httpClient)
    {
        return httpClient.GetAsync(
            "api/inquiry-templates",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTemplateByIdAsHttpResponseAsync(this HttpClient httpClient, int id)
    {
        return httpClient.GetAsync(
           $"api/inquiry-templates/{id}",
           _cancellationToken);
    }

    public static Task<InquiryTemplateOutput?> CreateTemplateAsync(this HttpClient httpClient, InquiryTemplateInput inquiryTemplateInput)
    {
        return httpClient.PostAsJsonAsync<InquiryTemplateInput, InquiryTemplateOutput>(
            "api/inquiry-templates",
            inquiryTemplateInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateTemplateAsHttpResponseAsync(this HttpClient httpClient,
        InquiryTemplateInput inquiryTemplateInput, JsonSerializerOptions jsonOptions)
    {
        return httpClient.PostAsJsonAsync(
            "api/inquiry-templates",
            inquiryTemplateInput,
            jsonOptions,
            _cancellationToken);
    }

    public static Task<InquiryTemplateOutput?> UpdateTemplateAsync(this HttpClient httpClient, int id, InquiryTemplateInput updateTemplateInput)
    {
        return httpClient.PutAsJsonAsync<InquiryTemplateInput, InquiryTemplateOutput>(
            $"api/inquiry-templates/{id}",
            updateTemplateInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateTemplateAsHttpResponseAsync(this HttpClient httpClient, int id, InquiryTemplateInput updateTemplateInput)
    {
        return httpClient.PutAsJsonAsync(
            $"api/inquiry-templates/{id}",
            updateTemplateInput,
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<InquiryTemplateOutput?> DeleteTemplateAsync(this HttpClient httpClient, int id)
    {
        return httpClient.DeleteFromJsonAsync<InquiryTemplateOutput>(
            $"api/inquiry-templates/{id}",
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteTemplateAsHttpResponseAsync(this HttpClient httpClient, int id)
    {
        return httpClient.DeleteAsync(
            $"api/inquiry-templates/{id}",
            _cancellationToken);
    }
}
