using System.Net.Http.Json;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Attendance;

internal static class AttendanceHttpClientExtensions
{
    private const string AttendancesApiUrl = "api/attendances";

    public static Task<HttpResponseMessage> GetDetailedAttendancesAsHttpMessage(this HttpClient client,
        Dictionary<string, string?> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var requestUri = new Uri($"{AttendancesApiUrl}/detailed?{queryString}", UriKind.Relative);

        return client.GetAsync(requestUri);
    }

    public static Task<HttpResponseMessage> GetOdpAttendancesTotalAsHttpMessage(this HttpClient client,
        Dictionary<string, string?> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var requestUri = new Uri($"{AttendancesApiUrl}/total?{queryString}", UriKind.Relative);

        return client.GetAsync(requestUri);
    }

    public static Task<HttpResponseMessage> GetOdpAttendancesTotalAsHttpMessage(this HttpClient client,
        string query)
    {
        var requestUri = new Uri($"{AttendancesApiUrl}/total?{query}", UriKind.Relative);

        return client.GetAsync(requestUri);
    }

    public static Task<PageableListOfItems<OdpAttendanceTotal>?> GetOdpAttendancesTotal(this HttpClient client,
        Dictionary<string, string?> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var requestUri = new Uri($"{AttendancesApiUrl}/total?{queryString}", UriKind.Relative);

        return client.GetFromJsonAsync<PageableListOfItems<OdpAttendanceTotal>>(requestUri);
    }
}
