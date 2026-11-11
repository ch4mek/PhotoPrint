using System.Net.Http.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientInquiryExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetInquiryByIdAsHttpResponseAsync(this HttpClient httpClient, int id)
    {
        return httpClient.GetAsync(
            $"api/inquiries/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetInquiryByUserIdAsHttpResponseAsync(this HttpClient httpClient, int userId)
    {
        return httpClient.GetAsync(
            $"api/users/{userId}/inquiries",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetInquiryByApproverIdAsHttpResponseAsync(this HttpClient httpClient, int approverId)
    {
        return httpClient.GetAsync(
            $"api/users/approvers/{approverId}/inquiries",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> SearchInquiryAsHttpResponseAsync(this HttpClient httpClient)
    {
        return httpClient.GetAsync(
            "api/users/inquiries/search",
            _cancellationToken);
    }

    public static Task<UserInquiryAgreementOutput?> CreateInquiryAsync(
        this HttpClient httpClient,
        int userId,
        UserInquiryInput userInquiryInput)
    {
        return httpClient.PostAsJsonAsync<UserInquiryInput, UserInquiryAgreementOutput>($"api/users/{userId}/inquiries",
            userInquiryInput,
            _cancellationToken);
    }

    public static Task<UserInquiryAgreementOutput?> UpdateInquiryAsync(
        this HttpClient httpClient,
        int id,
        UpdateUserInquiryInput updateInquiryInput)
    {
        return httpClient.PutAsJsonAsync<UpdateUserInquiryInput, UserInquiryAgreementOutput>(
            $"api/inquiries/{id}",
            updateInquiryInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateInquiryAsHttpResponseAsync(
        this HttpClient httpClient,
        int id,
        UpdateUserInquiryInput updateInquiryInput)
    {
        return httpClient.PutAsJsonAsync(
            $"api/inquiries/{id}",
            updateInquiryInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateInquiryAsHttpResponseMessageAsync(
        this HttpClient httpClient,
        int userId,
        UserInquiryInput userInquiryInput)
    {
        return httpClient.PostAsJsonAsync(
            $"api/users/{userId}/inquiries",
            userInquiryInput,
            _cancellationToken);
    }

    public static Task<UserInquiryAgreementOutput?> DeleteInquiryAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync<UserInquiryAgreementOutput>(
            $"api/inquiries/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteInquiryAsHttpResponseAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync(
            $"api/inquiries/{id}",
            _cancellationToken);
    }

    public static Task<List<LeaveOutput>?> GetLeavesAsync(
        this HttpClient httpClient,
        int skip,
        int top,
        int year,
        int month)
    {
        return httpClient.GetFromJsonAsync<List<LeaveOutput>>(
            $"api/users/leaves?skip={skip}&top={top}&year={year}&month={month}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetLeavesAsHttpResponseAsync(
        this HttpClient httpClient,
        int skip,
        int top,
        int year,
        int month)
    {
        return httpClient.GetAsync(
            $"api/users/leaves?skip={skip}&top={top}&year={year}&month={month}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTransactionInquiryHistoryAsHttpResponse(this HttpClient client,
        int userId, int skip, int top)
    {
        return client.GetAsync($"/api/users/{userId}/history?$skip={skip}&$top={top}", _cancellationToken);
    }
}
