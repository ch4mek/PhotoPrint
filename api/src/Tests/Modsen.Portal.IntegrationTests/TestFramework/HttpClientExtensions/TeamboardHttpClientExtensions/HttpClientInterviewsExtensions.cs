using System.Net.Http.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class HttpClientInterviewsExtensions
{
    public static Task<HttpResponseMessage> GetPageableInterviewsAsHttpResponseAsync(this HttpClient client,
        string query, CancellationToken cancellationToken)
    {
        return client.GetAsync($"api/interviews/search?{query}", cancellationToken);
    }

    public static Task<PageableListOfItems<OdpInterviewDto>?> GetPageableInterviewsAsync(this HttpClient client,
        string query, CancellationToken cancellationToken)
    {
        return client.GetFromJsonAsync<PageableListOfItems<OdpInterviewDto>>($"api/interviews/search?{query}",
            cancellationToken);
    }

    public static Task<UserInterviewDto?> CreateInterviewAsync(
        this HttpClient client, CreateOrUpdateInterviewInput input, CancellationToken cancellationToken)
    {
        return client.PostAsJsonAsync<CreateOrUpdateInterviewInput, UserInterviewDto>($"api/interviews", input,
            cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateInterviewAsHttpResponseAsync(
        this HttpClient client, CreateOrUpdateInterviewInput input, CancellationToken cancellationToken)
    {
        return client.PostAsJsonAsync($"api/interviews", input, IntegrationTestsConstants.DefaultSerializerOptions, cancellationToken);
    }

    public static Task<UserInterviewDto?> CreateInterviewInGroupAsync(
        this HttpClient client, Guid groupId, CreateOrUpdateInterviewInput input, CancellationToken cancellationToken)
    {
        return client.PostAsJsonAsync<CreateOrUpdateInterviewInput, UserInterviewDto>($"api/interviews/groups/{groupId}", input,
            cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateInterviewInGroupAsHttpResponseAsync(
        this HttpClient client, Guid groupId, CreateOrUpdateInterviewInput input, CancellationToken cancellationToken)
    {
        return client.PostAsJsonAsync($"api/interviews/groups/{groupId}", input, IntegrationTestsConstants.DefaultSerializerOptions, cancellationToken);
    }

    public static Task<UserInterviewDto?> UpdateInterviewAsync(
        this HttpClient client, int id, CreateOrUpdateInterviewInput input, CancellationToken cancellationToken)
    {
        return client.PutAsJsonAsync<CreateOrUpdateInterviewInput, UserInterviewDto>($"api/interviews/{id}", input,
            cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateInterviewAsHttpResponseAsync(
        this HttpClient client, int id, CreateOrUpdateInterviewInput input, CancellationToken cancellationToken)
    {
        return client.PutAsJsonAsync($"api/interviews/{id}", input, IntegrationTestsConstants.DefaultSerializerOptions, cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteInterviewAsync(
        this HttpClient client, int id, CancellationToken cancellationToken)
    {
        return client.DeleteAsync($"api/interviews/{id}", cancellationToken);
    }
}
