using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientAgreementsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static async Task<UserAgreementOutput?> CreateAgreementAsync(
        this HttpClient httpClient,
        int userId,
        UserAgreementInput userAgreementInput)
    {
        var createdResponse = await httpClient.PostAsJsonAsync(
            $"api/users/{userId}/agreements",
            userAgreementInput,
            _cancellationToken);

        return await createdResponse.Content.ReadFromJsonAsync<UserAgreementOutput>(
            _serializerOptions,
            _cancellationToken);
    }

    public static async Task<InquiryAgreementTemplateDto?> UpdateAgreementAsync(
        this HttpClient httpClient,
        int agreementId,
        UserAgreementUpdateInput userAgreementUpdateInput)
    {
        var updatedAgreementResponse = await httpClient.PutAsJsonAsync(
            $"api/agreements/{agreementId}",
            userAgreementUpdateInput,
            _serializerOptions,
            _cancellationToken);

        return await updatedAgreementResponse.Content.ReadFromJsonAsync<InquiryAgreementTemplateDto>(
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateAgreementAsHttpResponseAsync(
        this HttpClient httpClient,
        int agreementId,
        UserAgreementUpdateInput userAgreementUpdateInput)
    {
        return httpClient.PutAsJsonAsync(
            $"api/agreements/{agreementId}",
            userAgreementUpdateInput,
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateAgreementAsHttpResponseAsync(
        this HttpClient httpClient,
        int userId,
        UserAgreementInput userAgreementInput)
    {
        return httpClient.PostAsJsonAsync(
            $"api/users/{userId}/agreements",
            userAgreementInput,
            _cancellationToken);
    }

    public static Task<List<UserAgreementOutput>?> GetAgreementsAsync(
        this HttpClient httpClient,
        int userId)
    {
        return httpClient.GetFromJsonAsync<List<UserAgreementOutput>>(
                $"api/users/{userId}/agreements",
                _serializerOptions,
                _cancellationToken);
    }

    public static Task<List<UserAgreementOutput>?> GetAllAgreementsAsync(
        this HttpClient httpClient)
    {
        return httpClient.GetFromJsonAsync<List<UserAgreementOutput>>(
                "api/users/agreements",
                _serializerOptions,
                _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetAgreementHistoryAsResponseMessage(
        this HttpClient httpClient,
        int userId,
        AgreementType agreementType,
        DateOnly? startDate,
        DateOnly? endDate)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["UserId"] = userId.ToString(),
            ["AgreementType"] = ((int)agreementType).ToString(),
            ["StartDate"] = startDate?.ToString("yyyy-MM-dd"),
            ["EndDate"] = endDate?.ToString("yyyy-MM-dd")
        };

        var url = QueryHelpers.AddQueryString("api/users/agreements/history", queryParams);

        //Act
        return httpClient.GetAsync(url, _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetAgreementBalanceAsHttpResponseAsync(
        this HttpClient httpClient,
        int agreementId)
    {
        return httpClient.GetAsync(
            $"api/agreements/{agreementId}/balance",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteAgreementAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync($"api/agreements/{id}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateAgreementTransactionsAsync(
        this HttpClient httpClient,
        int userId, int agreementTemplateId, CorrectionAgreementTransactionInput input)
    {
        return httpClient.PostAsJsonAsync(
            $"api/users/{userId}/agreements/{agreementTemplateId}/correction",
            input,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<UserAbsenceHistoryCoreDto>?> GetUserAgreementHistoryAsync(
        this HttpClient httpClient,
        int userId)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<UserAbsenceHistoryCoreDto>>(
            $"api/users/{userId}/history?top=10&skip=0",
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<UserAgreementAssignCoreDto>?> GetUserAgreementAssignsAsync(
        this HttpClient httpClient,
        int agreementId)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<UserAgreementAssignCoreDto>>(
            $"api/agreements/{agreementId}/assigns/search?top=10&skip=0",
            _serializerOptions,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<UserAssignDto>?> GetUserInquiryTemplatesAssignsAsync(
        this HttpClient httpClient,
        int templateId)
    {
        return httpClient.GetFromJsonAsync<PageableListOfItems<UserAssignDto>>(
            $"api/templates/{templateId}/assigns/search?top=10&skip=0",
            _serializerOptions,
            _cancellationToken);
    }
}
