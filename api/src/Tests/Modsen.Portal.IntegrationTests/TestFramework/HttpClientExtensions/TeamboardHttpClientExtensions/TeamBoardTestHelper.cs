using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using Modsen.Portal.Utilities.Controllers.Configuration;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class TeamBoardTestHelper
{

    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public static async Task<ConfigurationOutput?> GetConfigurationAsync(this HttpClient client)
    {
        return await client.GetFromJsonAsync<ConfigurationOutput>("api/Configuration",
            _serializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateClient(this HttpClient client, CreateClientInput createClientInput)
    {
        return client.PostAsJsonAsync("api/clients",
            createClientInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateTimesheetFact(this HttpClient client, UpsertFactDto upsertFact)
    {
        return client.PostAsJsonAsync("/api/Timesheet/fact/upsert", upsertFact, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<T> WithResponseType<T>(this HttpResponseMessage message)
    {
        return message.Content.ReadFromJsonAsync<T>(_serializerOptions, _cancellationToken);
    }

    public static async Task<T> WithResponseType<T>(this Task<HttpResponseMessage> message)
    {
        var response = await message;
        return await response.Content.ReadFromJsonAsync<T>(_serializerOptions, _cancellationToken);
    }
}
