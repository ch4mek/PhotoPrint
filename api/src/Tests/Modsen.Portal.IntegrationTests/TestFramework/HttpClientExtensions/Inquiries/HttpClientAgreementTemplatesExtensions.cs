using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientAgreementTemplatesExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static async Task<HttpResponseMessage> CreateAgreemenTemplateAsync(
        this HttpClient httpClient,
        CreateAgreementTemplateInput createAgreementTemplateInput)
    {
        return await httpClient.PostAsJsonAsync(
            "api/agreement-templates",
            createAgreementTemplateInput,
            _cancellationToken);
    }

    public static async Task<int> CreateAgreemenTemplateWithResultAsync(
        this HttpClient httpClient,
        CreateAgreementTemplateInput createAgreementTemplateInput)
    {
        var createdResponse = await httpClient.PostAsJsonAsync(
            "api/agreement-templates",
            createAgreementTemplateInput,
            _cancellationToken);

        return await createdResponse.Content.ReadFromJsonAsync<int>(
            _serializerOptions,
            _cancellationToken);
    }

    public static async Task<InquiryAgreementTemplateDto> UpdateAgreemenTemplateAsync(
        this HttpClient httpClient,
        int id,
        AgreementTemplateInput agreementTemplateInput)
    {
        var response = await httpClient.PutAsJsonAsync(
            $"api/agreement-templates/{id}",
            agreementTemplateInput,
            _cancellationToken);

        return await response.Content.ReadFromJsonAsync<InquiryAgreementTemplateDto>(
            _serializerOptions,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> UpdateAgreemenTemplateWithResponceAsync(
       this HttpClient httpClient,
       int id,
       AgreementTemplateInput agreementTemplateInput)
    {
        return await httpClient.PutAsJsonAsync(
            $"api/agreement-templates/{id}",
            agreementTemplateInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteAgreemenTemplateAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync(
            $"api/agreement-templates/{id}",
            _cancellationToken);
    }
}
