using System.Net.Http.Json;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;

internal static class HttpClientProjectRatesExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> CreateProjectRateAsResponseMessageAsync(
        this HttpClient httpClient, CreateProjectRateInput input)
    {
        return httpClient.PostAsJsonAsync(
            $"api/project-rates",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static async Task<ProjectRateCoreDto?> CreateProjectRateAsync(this HttpClient httpClient,
        int userId, int projectId, decimal hourlyRate, string currencyId, DateOnly rateFrom)
    {
        var input = new CreateProjectRateInput(userId, projectId, hourlyRate, currencyId, rateFrom);

        var createdRate = await httpClient.PostAsJsonAsync<CreateProjectRateInput, ProjectRateCoreDto>(
            $"api/project-rates",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);

        return createdRate;
    }

    public static Task<HttpResponseMessage> EditProjectRateAsResponseMessageAsync(this HttpClient httpClient,
        int id, EditProjectRateInput input)
    {
        return httpClient.PatchAsJsonAsync(
            $"api/project-rates/{id}",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetRateHistoryAsResponseMessageAsync(this HttpClient httpClient,
        int projectId, int userId)
    {
        return httpClient.GetAsync(
            new Uri($"/api/projects/{projectId}/users/{userId}/rates", UriKind.Relative),            
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetRatesAsResponseMessageAsync(this HttpClient httpClient,
       int year, int month, GetProjectRatesInput input, int? pageSize = null, int? pageNumber = null)
    {
        var url = $"api/project-rates/{year}/{month}";

        var query = new List<string>();
        if (pageSize.HasValue)
        {
            query.Add($"pageSize={pageSize.Value}");
        }
        if (pageNumber.HasValue)
        {
            query.Add($"pageNumber={pageNumber.Value}");
        }
        if (query.Count > 0)
        {
            url += "?" + string.Join("&", query);
        }

        return httpClient.PostAsJsonAsync(
            url,
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
