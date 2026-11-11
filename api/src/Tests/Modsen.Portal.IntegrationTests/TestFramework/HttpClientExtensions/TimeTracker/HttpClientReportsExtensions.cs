using System.Net.Http.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

internal static class HttpClientReportsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;
    private static readonly int defaultYear = DateTime.UtcNow.Year;
    private static readonly int defaultMonth = DateTime.UtcNow.Month;

    public static Task<TrackedTimePerPeriodReportOutput> GetTrackedTimePerPeriodReportAsync(this HttpClient client,
        TrackedTimeType? trackedTimeType = null,
        int? userId = null,
        int? projectId = null,
        int? year = null,
        int? month = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetFromJsonAsync<TrackedTimePerPeriodReportOutput>(
                new Uri($"api/reports?{userIdSection}{projectIdSection}year={year}&monthOfYear[0]={month}&{trackedTimeSection}",
                    UriKind.Relative), _cancellationToken);
    }

    public static Task<List<TrackedTimePerProjectReportOutput>> GetTrackedTimePerProjectReportAsync(this HttpClient client,
        int userId,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null,
        int? week = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType}&";
        var yearSection = year is null ? "" : $"year={year}&";
        var monthSection = month is null ? "" : $"monthOfYear[0]={month}&";
        var weekSection = week is null ? "" : $"weekOfYear={week}&";

        return client.GetFromJsonAsync<List<TrackedTimePerProjectReportOutput>>(
            new Uri($"api/reports/{userId}?" +
                $"{trackedTimeSection}{yearSection}{monthSection}{weekSection}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTrackedTimePerProjectReportAsHttpMessageAsync(
        this HttpClient client,
        int userId,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null,
        int? week = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType}&";
        var yearSection = year is null ? "" : $"year={year}&";
        var monthSection = month is null ? "" : $"monthOfYear[0]={month}&";
        var weekSection = week is null ? "" : $"weekOfYear={week}&";

        return client.GetAsync(
            new Uri($"api/reports/{userId}?" +
                $"{trackedTimeSection}{yearSection}{monthSection}{weekSection}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<PageableListOfItems<TrackedTimePerUserReportOutput>> GetTrackedTimePerUsersReportAsync(this HttpClient client,
        int? userId = null,
        int? projectId = null,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null,
        int? page =  1,
        int? pageSize = 100)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType}&";
        var yearSection = year is null ? "" : $"year={year}&";
        var monthSection = month is null ? "" : $"month={month}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        var projectSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetFromJsonAsync<PageableListOfItems<TrackedTimePerUserReportOutput>>(
            new Uri($"api/reports/users?" +
                $"page={page}&pageSize={pageSize}&{projectSection}{userIdSection}{trackedTimeSection}{yearSection}{monthSection}",
                UriKind.Relative), _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTrackedTimePerUsersReportAsHttpMessageAsync(this HttpClient client,
        int? userId = null,
        int? projectId = null,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null,
        int? page =  1,
        int? pageSize = 100)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType}&";
        var yearSection = year is null ? "" : $"year={year}&";
        var monthSection = month is null ? "" : $"month={month}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        var projectSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetAsync(
            new Uri($"api/reports/users?" +
                $"page={page}&pageSize={pageSize}&{projectSection}{userIdSection}{trackedTimeSection}{yearSection}{monthSection}",
                UriKind.Relative), _cancellationToken);
    }

    public static Task<List<TrackedTaskReportOutput>> GetTrackedTasksReportAsync(this HttpClient client,
        int userId,
        int projectId,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null,
        int? week = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        var yearSection = year is null ? "" : $"year={year}&";
        var monthSection = month is null ? "" : $"month={month}&";
        var weekSection = week is null ? "" : $"weekOfYear={week}&";

        return client.GetFromJsonAsync<List<TrackedTaskReportOutput>>(
            new Uri($"api/reports/{userId}/{projectId}?{yearSection}{monthSection}{weekSection}" +
                $"{trackedTimeSection}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<ClientsUsersAndProjectsWithTrackedTimeReportOutput> GetPieChartReportsAsync(this HttpClient client,
        TrackedTimeType? trackedTimeType = null,
        int? userId = null,
        int? year = null,
        int? month = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        year ??= defaultYear;
        month ??= defaultMonth;

        return client.GetFromJsonAsync<ClientsUsersAndProjectsWithTrackedTimeReportOutput>(
            $"api/reports/pieCharts?year={year}&monthOfYear[0]={month}&{trackedTimeSection}{userIdSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPieChartReportsAsHttpMessageAsync(this HttpClient client)
    {
        return client.GetAsync("api/reports/pieCharts", _cancellationToken);
    }

    public static Task<List<ClientWithTrackedTimeReportOutput>> GetPieChartClientsAsync(this HttpClient client,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        year ??= defaultYear;
        month ??= defaultMonth;

        return client.GetFromJsonAsync<List<ClientWithTrackedTimeReportOutput>>(
            $"api/reports/pieChart/clients?year={year}&monthOfYear[0]={month}&{trackedTimeSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPieChartClientsAsHttpMessageAsync(this HttpClient client)
    {
        return client.GetAsync("api/reports/pieChart/clients", _cancellationToken);
    }

    public static Task<List<ClientWithTrackedTimeReportOutput>> GetPieChartProjectsAsync(this HttpClient client,
        TrackedTimeType? trackedTimeType = null,
        int? year = null,
        int? month = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        year ??= defaultYear;
        month ??= defaultMonth;

        return client.GetFromJsonAsync<List<ClientWithTrackedTimeReportOutput>>(
            $"api/reports/pieChart/projects?year={year}&monthOfYear[0]={month}&{trackedTimeSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPieChartProjectsAsHttpMessageAsync(this HttpClient client)
    {
        return client.GetAsync("api/reports/pieChart/projects", _cancellationToken);
    }

    public static Task<List<UserWithTrackedTimeReportOutput>> GetPieChartUsersAsync(this HttpClient client,
        TrackedTimeType? trackedTimeType = null,
        int? userId = null,
        int? year = null,
        int? month = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        year ??= defaultYear;
        month ??= defaultMonth;

        return client.GetFromJsonAsync<List<UserWithTrackedTimeReportOutput>>(
            $"api/reports/pieChart?year={year}&monthOfYear[0]={month}&{userIdSection}{trackedTimeSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPieChartUsersAsHttpMessageAsync(this HttpClient client,
        TrackedTimeType? trackedTimeType = null,
        int? userId = null,
        int? year = null,
        int? month = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        year ??= defaultYear;
        month ??= defaultMonth;

        return client.GetAsync(
            $"api/reports/pieChart?year={year}&monthOfYear[0]={month}&{userIdSection}{trackedTimeSection}",
            _cancellationToken);
    }

    public static Task<Stream> GetReportsAsCsvAsync(this HttpClient client)
    {
        return client.GetStreamAsync("api/Reports/csv", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetReportsWithFormatAsync(this HttpClient client,
        string format,
        int? year = null,
        int? projectId = null,
        int? userId = null,
        int? clientId = null,
        int? month = null,
        TrackedTimeType? trackedTimeType = null,
        bool? isPerUserExport = null)
    {
        var trackedTimeSection = trackedTimeType is null ? "" : $"trackedTimeType={trackedTimeType.Value}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        var clientIdSection = clientId is null ? "" : $"clientId={clientId}&";
        var yearSection = year is null ? "" : $"year={year}&";
        var monthSection = month is null ? "" : $"month={month}&";
        var projectSection = projectId is null ? "" : $"projectId={projectId}&";
        var perUserExportSection = isPerUserExport is null ? "" : $"isPerUserExport={isPerUserExport}";

        return client.GetAsync(
            new Uri($"api/Reports/{format}?" +
                $"{userIdSection}" +
                $"{clientIdSection}" +
                $"{yearSection}" +
                $"{monthSection}" +
                $"{projectSection}" +
                $"{trackedTimeSection}" +
                $"{perUserExportSection}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetReportsAsExcelAsync(this HttpClient client,
        int? year = null,
        int? projectId = null)
    {
        var yearSection = year is null ? "" : $"year={year}&";
        var projectSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetAsync(
            new Uri($"api/Reports/excelAccount?" +
                $"{yearSection}" +
                $"{projectSection}", UriKind.Relative),
            _cancellationToken);
    }
}
