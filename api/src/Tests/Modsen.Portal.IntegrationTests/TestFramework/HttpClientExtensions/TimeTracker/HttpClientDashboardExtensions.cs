using System.Net.Http.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

internal static class HttpClientDashboardExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> GetDashboardUserProjectAsHttpMessageAsync(this HttpClient client,
        int userId,
        int projectId)
    {
        return client.GetAsync($"api/dashboard/users/{userId}/{projectId}", _cancellationToken);
    }

    public static Task<List<DashboardTasksCoreDto>> GetDashboardUserProjectAsync(this HttpClient client,
        int userId,
        int projectId,
        string? departmentId = null,
        int? page = null,
        int? rowsCount = null)
    {
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"departmentId={departmentId}&";
        var pageSection = page is null ? "" : $"page={page}&";
        var rowsCountSection = rowsCount is null ? "" : $"rowsCount={rowsCount}";

        return client.GetFromJsonAsync<List<DashboardTasksCoreDto>>(
            $"api/dashboard/users/{userId}/{projectId}?{departmentSection}{pageSection}{rowsCountSection}",
            _cancellationToken);
    }

    public static Task<DashboardUsersCoreDto> GetDashboardUsersAsync(this HttpClient client,
        string? departmentId = null,
        int? page = null,
        int? rowsCount = null)
    {
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"departmentId={departmentId}&";
        var pageSection = page is null ? "" : $"page={page}&";
        var rowsCountSection = rowsCount is null ? "" : $"rowsCount={rowsCount}";

        return client.GetFromJsonAsync<DashboardUsersCoreDto>(
            $"api/dashboard/users?{departmentSection}{pageSection}{rowsCountSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetDashboardUsersAsHttpMessageAsync(this HttpClient client,
        string? departmentId = null,
        int? page = null,
        int? rowsCount = null)
    {
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"departmentId={departmentId}&";
        var pageSection = page is null ? "" : $"page={page}&";
        var rowsCountSection = rowsCount is null ? "" : $"rowsCount={rowsCount}";

        return client.GetAsync($"api/dashboard/users?{departmentSection}{pageSection}{rowsCountSection}",
            _cancellationToken);
    }

    public static Task<List<DashboardOfUserProjectsCoreDto>> GetUserDashboardAsync(this HttpClient client,
        int userId,
        string? departmentId = null,
        int? page = null,
        int? rowsCount = null)
    {
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"departmentId={departmentId}&";
        var pageSection = page is null ? "" : $"page={page}&";
        var rowsCountSection = rowsCount is null ? "" : $"rowsCount={rowsCount}";

        return client.GetFromJsonAsync<List<DashboardOfUserProjectsCoreDto>>(
            $"api/dashboard/users/{userId}?{departmentSection}{pageSection}{rowsCountSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetUserDashboardAsHttpMessageAsync(this HttpClient client, int userId)
    {
        return client.GetAsync($"api/dashboard/users/{userId}", _cancellationToken);
    }

    public static Task<DashboardCurrentUserCoreDto> GetDashboardAsync(this HttpClient client,
        string? filter = null,
        int top = 25,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"filter={filter}&";
        return client.GetFromJsonAsync<DashboardCurrentUserCoreDto>(
            $"api/dashboard?top={top}&skip={skip}&{filterSection}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetDashboardAsHttpMessageAsync(this HttpClient client)
    {
        return client.GetAsync("api/dashboard", _cancellationToken);
    }

    public static Task<DashboardCurrentUserAllTrackedHoursCoreDto> GetDashboardTrackedTimeAsync(
        this HttpClient client)
    {
        return client.GetFromJsonAsync<DashboardCurrentUserAllTrackedHoursCoreDto>
            ("api/dashboard/tracked-time", IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<DashboardOverviewCoreDto> GetDashboardOverviewAsync(this HttpClient client,
        string? departmentId = null)
    {
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"departmentId={departmentId}&";

        return client.GetFromJsonAsync<DashboardOverviewCoreDto>(
            $"api/dashboard/overview?{departmentSection}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetDashboardOverviewAsHttpMessageAsync(this HttpClient client,
        string? departmentId = null)
    {
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"departmentId={departmentId}&";

        return client.GetAsync(
            $"api/dashboard/overview?{departmentSection}", _cancellationToken);
    }

    public static Task<List<PieChartUsersCoreDto>> GetDashboardPieChartAsync(this HttpClient client,
        int? userId = null,
        int? year = null,
        int? month = null)
    {
        year ??= DateTime.UtcNow.Year;
        month ??= DateTime.UtcNow.Month;
        var userIdSection = userId is null ? "" : $"userId={userId}";

        return client.GetFromJsonAsync<List<PieChartUsersCoreDto>>(
            $"api/dashboard/pieChart?year={year}&monthOfYear[0]={month}&{userIdSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetDashboardPieChartAsHttpMessageAsync(this HttpClient client,
        int? userId = null,
        int? year = null,
        int? month = null)
    {
        year ??= DateTime.UtcNow.Year;
        month ??= DateTime.UtcNow.Month;
        var userIdSection = userId is null ? "" : $"userId={userId}";

        return client.GetAsync($"api/dashboard/pieChart?year={year}&monthOfYear[0]={month}&{userIdSection}",
            _cancellationToken);
    }

    public static Task<List<TimeStackCoreDto>> GetTimeStackAsync(this HttpClient client,
        string departmentId = "",
        int? year = null,
        int? month = null)
    {
        year ??= DateTime.UtcNow.Year;
        month ??= DateTime.UtcNow.Month;
        var departmentSection = departmentId.IsNullOrEmpty() ? "" : $"department={departmentId}";

        return client.GetFromJsonAsync<List<TimeStackCoreDto>>(
            $"api/dashboard/time-stack?year={year}&monthOfYear={month}&{departmentSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetTimeStackAsHttpMessageAsync(this HttpClient client)
    {
        return client.GetAsync("api/dashboard/time-stack", _cancellationToken);
    }

    public static Task<TimesheetCoreDto> TrackFactAsync(this HttpClient client,
        int projectId,
        int? userId = null,
        DateOnly? date = null,
        string? name = null,
        int trackedInMinutes = 100)
    {
        var task = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            FactTrackedInMinutes = trackedInMinutes,
            UserId = userId
        };

        return client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>(
            "/api/Timesheet/fact/upsert", task, _cancellationToken);
    }

    public static Task<TimesheetCoreDto> TrackFactAsync(this HttpClient client,
        UpsertFactDto input)
    {
        return client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>(
            "/api/Timesheet/fact/upsert", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> TrackFactAsHttpMessageAsync(this HttpClient client,
        int projectId,
        int? userId = null,
        DateOnly? date = null,
        string? name = null,
        int trackedInMinutes = 100)
    {
        var task = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            FactTrackedInMinutes = trackedInMinutes,
            UserId = userId
        };

        return client.PostAsJsonAsync("/api/Timesheet/fact/upsert", task, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> TrackFactAsHttpMessageAsync(this HttpClient client,
        UpsertFactDto input)
    {
        return client.PostAsJsonAsync("/api/Timesheet/fact/upsert", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> TrackFactToTimesheetManagerAsHttpMessageAsync(this HttpClient client,
        int projectId,
        int? userId = null,
        DateOnly? date = null,
        string? name = null,
        int trackedInMinutes = 100)
    {
        var task = new UpsertFactDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            FactTrackedInMinutes = trackedInMinutes,
            UserId = userId
        };

        return client.PostAsJsonAsync(
            "api/timesheetManager", task, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<TimesheetCoreDto> TrackBillableAsync(this HttpClient client,
        int projectId,
        int? userId = null,
        DateOnly? date = null,
        string? name = null,
        int trackedInMinutes = 100)
    {
        var billable = new UpsertBillableDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            BillableTrackedInMinutes = trackedInMinutes,
            UserId = userId
        };

        return client.PostAsJsonAsync<UpsertBillableDto, TimesheetCoreDto>(
            "/api/Timesheet/billable/upsert",
            billable,
            _cancellationToken);
    }

    public static Task<TimesheetCoreDto> TrackBillableAsync(this HttpClient client,
        UpsertBillableDto input)
    {
        return client.PostAsJsonAsync<UpsertBillableDto, TimesheetCoreDto>(
            "/api/Timesheet/billable/upsert",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> TrackBillableAsHttpMessageAsync(this HttpClient client,
        int projectId,
        int? userId = null,
        DateOnly? date = null,
        string? name = null,
        int trackedInMinutes = 100)
    {
        var billable = new UpsertBillableDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            BillableTrackedInMinutes = trackedInMinutes,
            UserId = userId
        };

        return client.PostAsJsonAsync(
            "/api/Timesheet/billable/upsert",
            billable,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> TrackBillableAsHttpMessageAsync(this HttpClient client,
        UpsertBillableDto input)
    {
        return client.PostAsJsonAsync(
            "/api/Timesheet/billable/upsert",
            input,
            _cancellationToken);
    }

    public static Task<TimesheetCoreDto> CreateFactTimesheetAsync(this HttpClient client, CreateTimesheetDto input)
    {
        return client.PostAsJsonAsync<CreateTimesheetDto, TimesheetCoreDto>("api/timesheet/fact/upsert", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateTask(this HttpClient client, UpdateTaskNameDto updateTaskNameDto)
    {
        return client
            .PatchAsJsonAsync("api/timesheet/taskName", updateTaskNameDto, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateFactTimesheetAsHttpMessageAsync(this HttpClient client,
        int projectId,
        DateOnly? date = null,
        string? name = null,
        int factTrackedInMinutes = 100,
        int billableTrackedInMinutes = 100)
    {
        var task = new CreateTimesheetDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            FactTrackedInMinutes = factTrackedInMinutes,
            BillableTrackedInMinutes = billableTrackedInMinutes
        };

        return client.PostAsJsonAsync("api/timesheet/fact/upsert", task, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateFactTimesheetAsHttpMessageAsync(this HttpClient client,
        UpsertFactDto input)
    {
        return client.PostAsJsonAsync("api/timesheet/fact/upsert", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateTimesheetToTimesheetManagerAsHttpMessageAsync(this HttpClient client,
        int projectId,
        int? userId = null,
        DateOnly? date = null,
        string? name = null,
        int factTrackedInMinutes = 100,
        int billableTrackedInMinutes = 100)
    {
        var task = new CreateTimesheetDto
        {
            ProjectId = projectId,
            ReportForDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name!,
            FactTrackedInMinutes = factTrackedInMinutes,
            BillableTrackedInMinutes = billableTrackedInMinutes,
            UserId = userId ?? 0
        };

        return client.PostAsJsonAsync("api/timesheetManager", task, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteFactAsHttpMessageAsync(this HttpClient client, int id)
    {
        return client.DeleteAsync($"api/timesheet/{id}/fact", _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteTimesheetAsHttpMessageAsync(this HttpClient client, int timesheetId)
    {
        return client.DeleteAsync(new Uri($"api/timesheet/{timesheetId}", UriKind.Relative), _cancellationToken);
    }

    public static async Task DeleteTimesheetsUnderCeoRoleAsync(this HttpClient client,
        List<TimesheetCoreDto> timesheets)
    {
        await client.AuthAsCeo();
        foreach (var timesheet in timesheets)
        {
            await client.DeleteTimesheetAsHttpMessageAsync(timesheet.Id);
        }
    }


    public static Task<HttpResponseMessage> GetTimesheetByUserIdAndQuery(this HttpClient client, int? userId, string query)
    {
        return client.GetAsync($"api/timesheet/{userId}?{query}", _cancellationToken);
    }
}
