using System.Net.Http.Json;
using System.Text;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

internal static class HttpClientTimesheetExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<PageableUserTimesheets> GetTimesheetsAsync(this HttpClient client,
        string? dateFrom = null,
        string? dateTo = null,
        int? userId = null,
        int? projectId = null,
        string? departmentId = null,
        bool? onlyManagedProjects = null,
        int top = 100,
        int skip = 0)
    {
        var onlyManagedProjectsSection = onlyManagedProjects is null ? "" : $"onlyManagedProjects={onlyManagedProjects}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";
        var departmentIdSection = departmentId is null ? "" : $"departmentId={departmentId}&";
        var dateFromSection = dateFrom is null ? "" : $"reportForDateFrom={dateFrom}&";
        var dateToSection = dateTo is null ? "" : $"reportForDateTo={dateTo}&";

        return client.GetFromJsonAsync<PageableUserTimesheets>(
            $"api/timesheet?top={top}&skip={skip}&{userIdSection}{dateFromSection}" +
            $"{dateToSection}{projectIdSection}{departmentIdSection}{onlyManagedProjectsSection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimesheetsAsHttpMessageAsync(this HttpClient client,
        string? dateFrom = null,
        string? dateTo = null,
        int? userId = null,
        int? projectId = null,
        string? departmentId = null,
        bool? onlyManagedProjects = null,
        int top = 100,
        int skip = 0)
    {
        var onlyManagedProjectsSection = onlyManagedProjects is null ? "" : $"onlyManagedProjects={onlyManagedProjects}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";
        var departmentIdSection = departmentId is null ? "" : $"departmentId={departmentId}&";
        var dateFromSection = dateFrom is null ? "" : $"reportForDateFrom={dateFrom}&";
        var dateToSection = dateTo is null ? "" : $"reportForDateTo={dateTo}&";

        return client.GetAsync(
            $"api/timesheet?top={top}&skip={skip}&{userIdSection}{dateFromSection}" +
            $"{dateToSection}{projectIdSection}{departmentIdSection}{onlyManagedProjectsSection}",
            _cancellationToken);
    }

    public static Task<StatusOutput> GetTimesheetStatusAsync(this HttpClient client,
        int userId,
        int month,
        int year)
    {
        return client.GetFromJsonAsync<StatusOutput>(
                $"api/users/{userId}/timesheets/status?month={month}&year={year}",
                IntegrationTestsConstants.DefaultSerializerOptions,
                _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimesheetStatusAsHttpMessageAsync(this HttpClient client,
        int userId,
        int month,
        int year)
    {
        return client.GetAsync($"api/users/{userId}/timesheets/status?month={month}&year={year}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimesheetsWithRawSearchAsHttpMessageAsync(this HttpClient client,
        string query)
    {
        return client.GetAsync($"api/timesheet/{query}", _cancellationToken);
    }

    public static Task<UserTimesheetsWithBlockedDaysDto> GetTimesheetByUserIdAsync(this HttpClient client,
        int userId,
        string? filter = null,
        bool? onlyManagedProjects = null,
        int? projectId = null,
        int? month = null,
        int? year = null,
        int top = 100,
        int skip = 0)
    {
        var onlyManagedProjectsSection = onlyManagedProjects is null ? "" : $"onlyManagedProjects={onlyManagedProjects}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";
        var filterSection = filter.IsNullOrEmpty() ? "" : $"filter={filter}&";
        var monthSection = month is null ? "" : $"monthOfYear={month}&";
        var yearSection = year is null ? "" : $"year={year}&";

        return client.GetFromJsonAsync<UserTimesheetsWithBlockedDaysDto>(
            $"api/timesheet/{userId}?top={top}&skip={skip}&{filterSection}{onlyManagedProjectsSection}" +
            $"{projectIdSection}{monthSection}{yearSection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimesheetByUserIdAsHttpMessageAsync(this HttpClient client,
        int userId,
        string? filter = null,
        bool? onlyManagedProjects = null,
        int? projectId = null,
        int top = 100,
        int skip = 0)
    {
        var onlyManagedProjectsSection = onlyManagedProjects is null ? "" : $"onlyManagedProjects={onlyManagedProjects}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";
        var filterSection = filter.IsNullOrEmpty() ? "" : $"filter={filter}&";

        return client.GetAsync($"api/timesheet/{userId}?top={top}&skip={skip}&{filterSection}{onlyManagedProjectsSection}{projectIdSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimesheetByUserIdWithRawSearchAsHttpMessageAsync(this HttpClient client,
        int userId,
        string query)
    {
        return client.GetAsync($"api/timesheet/{userId}{query}", _cancellationToken);
    }

    public static async Task<UserTimesheetsWithBlockedDaysDto> GetTimesheetByUserIdAsPostMethodAsync(this HttpClient client,
        int userId,
        string filter,
        int top = 100,
        int skip = 0)
    {
        using var content = new StringContent(filter, Encoding.UTF8, "text/plain");
        var response = await client.PostAsync($"api/timesheet/{userId}/$query?top={top}&skip={skip}", content, _cancellationToken);
        return await response.Content.ReadFromJsonAsync<UserTimesheetsWithBlockedDaysDto>(cancellationToken: _cancellationToken);
    }

    public static Task<StatusOutput> UpdateTimesheetStatusAsync(this HttpClient client, int userId,
        int month,
        int year,
        TimesheetSubmitStatusType status)
    {
        var input = new UpdateStatusInput(status);

        return client.PutAsJsonAsync<UpdateStatusInput, StatusOutput>(
            $"api/users/{userId}/timesheets/status?month={month}&year={year}",
            input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateTimesheetStatusAsHttpMessageAsync(this HttpClient client, int userId,
        int month,
        int year,
        TimesheetSubmitStatusType status)
    {
        var input = new UpdateStatusInput(status);

        return client.PutAsJsonAsync(
            $"api/users/{userId}/timesheets/status?month={month}&year={year}",
            input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateTaskNameAsHttpMessageAsync(this HttpClient client,
        UpdateTaskNameDto input)
    {
        return client.PatchAsJsonAsync("api/timesheet/taskName", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<List<UserLookupWithFiltersOutput>> GetLookupUsersReportsWithFiltersAsync(this HttpClient client,
        bool? onlyManagedUsers = null,
        int? clientId = null,
        int? projectId = null,
        string? keyWord = null)
    {
        var clientIdSection = clientId is null ? "" : $"clientId={clientId}&";
        var onlyManagedUsersSection = onlyManagedUsers is null ? "" : $"onlyManagedUsers={onlyManagedUsers}&";
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetFromJsonAsync<List<UserLookupWithFiltersOutput>>(
            $"api/users/lookup/filters/reports?{clientIdSection}{onlyManagedUsersSection}{keyWordSection}{projectIdSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimesheetLookupUsersAsHttpMessageAsync(this HttpClient client,
        bool? onlyManagedUsers = null,
        int? clientId = null,
        int? projectId = null,
        string? keyWord = null)
    {
        var clientIdSection = clientId is null ? "" : $"clientId={clientId}&";
        var onlyManagedUsersSection = onlyManagedUsers is null ? "" : $"onlyManagedUsers={onlyManagedUsers}&";
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetAsync(
            $"api/users/lookup/filters/reports?{clientIdSection}{onlyManagedUsersSection}{keyWordSection}{projectIdSection}",
            _cancellationToken);
    }

    public static Task<List<UserLookupWithFiltersOutput>> GetTimesheetLookupUsersWithFiltersAsync(this HttpClient client,
        bool? onlyManagedUsers = null,
        int? clientId = null,
        int? projectId = null,
        string? keyWord = null)
    {
        var clientIdSection = clientId is null ? "" : $"clientId={clientId}&";
        var onlyManagedUsersSection = onlyManagedUsers is null ? "" : $"onlyManagedUsers={onlyManagedUsers}&";
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var projectIdSection = projectId is null ? "" : $"projectId={projectId}&";

        return client.GetFromJsonAsync<List<UserLookupWithFiltersOutput>>(
            $"api/users/lookup/filters?{clientIdSection}{onlyManagedUsersSection}{keyWordSection}{projectIdSection}",
            _cancellationToken);
    }
}
