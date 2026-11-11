using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;

internal static class HttpClientCalendarExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly JsonSerializerOptions _serializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;

    public static async Task<CalendarExclusionInput?> CreateCalendarExclusionAsync(
        this HttpClient httpClient,
        CalendarExclusionInput calendarExclusionInput)
    {
        var response = await httpClient.PostAsJsonAsync("api/calendar/exclusions", calendarExclusionInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);

        return await response.Content.ReadFromJsonAsync<CalendarExclusionInput>(_serializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateCalendarExclusionAsHttpResponseAsync(
       this HttpClient httpClient,
       CalendarExclusionInput calendarExclusionInput)
    {
        return httpClient.PostAsJsonAsync("api/calendar/exclusions", calendarExclusionInput, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<PageableListOfItems<CalendarExclusionCoreDto>?> GetAllCalendarExclusionAsync(
        this HttpClient httpClient,
        int countryId,
        int top, int skip)
    {
        var response = await httpClient
            .GetAsync(
            $"api/calendar/exclusions?filter=(countryId eq {countryId})&top={top}&skip={skip}&",
            _cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<PageableListOfItems<CalendarExclusionCoreDto>>(_serializerOptions, _cancellationToken);
    }
}
