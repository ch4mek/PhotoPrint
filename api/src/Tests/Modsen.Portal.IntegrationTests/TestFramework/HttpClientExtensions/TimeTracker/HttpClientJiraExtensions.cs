using System.Net.Http.Json;
using MediatR;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

public static class HttpClientJiraExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<JiraSettingOutput?> CreateJiraSettingAsync(
        this HttpClient client,
        CreateJiraSettingInput input)
    {
        return client.PostAsJsonAsync<CreateJiraSettingInput, JiraSettingOutput>(
            "api/jira-settings",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateJiraSettingAsHttpMessageAsync(
        this HttpClient client,
        CreateJiraSettingInput input)
    {
        return client.PostAsJsonAsync(
            "api/jira-settings",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<JiraSettingOutput?> UpdateJiraSettingAsync(
        this HttpClient client,
        int id,
        UpdateJiraSettingInput input)
    {
        return client.PutAsJsonAsync<UpdateJiraSettingInput, JiraSettingOutput>(
            $"api/jira-settings/{id}",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateJiraSettingAsHttpMessageAsync(
        this HttpClient client,
        int id,
        UpdateJiraSettingInput input)
    {
        return client.PutAsJsonAsync(
            $"api/jira-settings/{id}",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<JiraSettingOutput?> GetJiraSettingByIdAsync(this HttpClient client, int id)
    {
        return client.GetFromJsonAsync<JiraSettingOutput>(
            $"api/jira-settings/{id}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetJiraSettingByIdAsHttpMessageAsync(this HttpClient client, int id)
    {
        return client.GetAsync(
            $"api/jira-settings/{id}",
            _cancellationToken);
    }

    public static Task<PageableListOfItems<JiraSettingOutput>?> SearchJiraSettingsAsync(
        this HttpClient client,
        int top = 25,
        int skip = 0,
        string? filter = null)
    {
        var filterSection = string.IsNullOrWhiteSpace(filter) ? string.Empty : $"&filter={filter}";

        return client.GetFromJsonAsync<PageableListOfItems<JiraSettingOutput>>(
            $"api/jira-settings/search?top={top}&skip={skip}{filterSection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> SearchJiraSettingsAsHttpMessageAsync(
        this HttpClient client,
        string query)
    {
        return client.GetAsync(
            $"api/jira-settings/search{query}",
            _cancellationToken);
    }

    public static Task<Unit> DeleteJiraSettingAsync(this HttpClient client, int id)
    {
        return client.DeleteAsync<Unit>(
            $"api/jira-settings/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteJiraSettingAsHttpMessageAsync(this HttpClient client, int id)
    {
        return client.DeleteAsync(
            $"api/jira-settings/{id}",
            _cancellationToken);
    }
}
