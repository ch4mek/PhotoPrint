using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Api.Modules;
using Modsen.Portal.Core;
using Modsen.Portal.Core.HostBuilder;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Contract;

namespace Modsen.Portal.Tools.PeopleForceDataComposer;

public class Program
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
    };
    private const string DocumentsDirectory = "C:\\pf-backup\\files";
    private const string RootDirectory = "C:\\pf-backup\\";
    private const string FilePathUrlMappingFilePath = "C:\\pf-backup\\PeopleForceMap.txt";
    private static IPeopleForceApiService _peopleForceApiService;

    public static async Task Main(string[] args)
    {
        var pfApiKey = "*****1231*****";
        //var env = "Development";
        var env = "Production";

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);
        Environment.SetEnvironmentVariable("PF_AppSettings__PeopleForceSettings__ApiKeyPeopleForce", pfApiKey);
        Environment.SetEnvironmentVariable("PF_Logging__LogLevel__Default", "Error");

        ConsoleFormatter.WriteLine($"ASPNETCORE_ENVIRONMENT = {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"):Red}");

        var webApplication = Init();
        using var scope = webApplication.Services.CreateScope();

        _peopleForceApiService = scope.ServiceProvider.GetRequiredService<IPeopleForceApiService>();
        var cancellationToken = CancellationToken.None;

        Directory.CreateDirectory(RootDirectory);
        Directory.CreateDirectory(DocumentsDirectory);

        await SaveAllPeopleForceData(cancellationToken);

        Console.ReadKey();
    }

    private static WebApplication Init()
    {
        var overridenSettings = new Dictionary<string, string?>
        {
            {"Logging:LogLevel:Default", "Warning"},
            {"Logging:LogLevel:Program", "Information"}
        };

        var webApp = ServiceHostBuilder
            .Create([], "Portal API", version: "pf-data-composer", PortalSharedConstants.EnvVariablesPfPrefix, overridenSettings)
            .WithModule((_, root) => new ApiServiceModule(root))
            .WithSignalR()
            .Build();

        Console.Clear();

        return webApp;
    }

    private static async Task SaveAllPeopleForceData(CancellationToken cancellationToken)
    {
        await SaveGeneralInfo(cancellationToken);
        await SaveEmployeesAllData(cancellationToken);
        await SaveLeaveRequestsData(cancellationToken);
        await SaveVacanciesData(cancellationToken);
        var candidateIds = await SaveCandidatesData(cancellationToken);
        await SaveCandidateDetails(null, cancellationToken);
    }

    private static async Task SaveGeneralInfo(CancellationToken cancellationToken)
    {
        foreach (var endpoint in GeneralUris)
        {
            try
            {
                ConsoleFormatter.WriteLine($"\nStart receiving data from {endpoint.Uri:White}");

                var response = await _peopleForceApiService.GetDataAsJsonArrayByUri(endpoint.Uri, cancellationToken, endpoint.NeedPagination);
                ConsoleFormatter.WriteLine($"{response.Count:Yellow} items {"received":Green}");
                if (response.Count > 0)
                {
                    await SaveData(response, endpoint.Uri, endpoint.Uri.Split("/").Last() + ".json");
                }
            }
            catch (Exception ex)
            {
                ConsoleFormatter.WriteLine($"{"Error":Red} occurred while saving data from {endpoint:White}: {ex.Message}");
            }

            await Task.Delay(250);
        }
    }

    private static async Task SaveEmployeesData(CancellationToken cancellationToken)
    {
        var employees = await _peopleForceApiService.GetAllEmployeesAsync(cancellationToken);
        var employeesJson = JsonSerializer.Serialize(employees, _jsonSerializerOptions);

        await File.WriteAllTextAsync(Path.Combine(RootDirectory, "employees.json"), employeesJson, Encoding.UTF8);
        ConsoleFormatter.WriteLine($"{employees.Count:Yellow} employees received and saved");

        foreach (var employee in employees)
        {
            ConsoleFormatter.WriteLine($"\nStart receiving data for employee {employee.Email:Yellow} with PF id {employee.Id}");


            foreach (var endpoint in EmployeeUris)
            {
                var requestUri = endpoint.Uri.Replace("{employee_id}", employee.Id.ToString(),
                    StringComparison.CurrentCulture);

                try
                {
                    ConsoleFormatter.WriteLine($"Start receiving data from {requestUri:White}");

                    var response = await _peopleForceApiService.GetDataAsJsonArrayByUri(requestUri, cancellationToken, endpoint.NeedPagination);
                    ConsoleFormatter.WriteLine($"{response.Count:Yellow} items received");
                    if (response.Count > 0)
                    {
                        await SaveData(response, endpoint.Uri,
                            $"employee_{employee.Id}_" + endpoint.Uri.Split("/").Last() + ".json");
                    }

                }
                catch (Exception ex)
                {
                    ConsoleFormatter.WriteLine(
                        $"{"Error":Red} occurred while saving data from {requestUri:Red} for {employee.Email:Yellow}: {ex.Message}");
                }

                await Task.Delay(250);
            }

            ConsoleFormatter.WriteLine($"{"All data was saved":Green} for employee {employee.Email:Yellow}");
        }
    }

    private static async Task SaveEmployeesAllData(CancellationToken cancellationToken)
    {
        const string employeesUrl = "/api/public/v2/employees?status=all";

        List<int> employeesIds = [];

        try
        {
            var employeesJson = await _peopleForceApiService
                .GetDataAsJsonArrayByUri(employeesUrl, cancellationToken, true);
            ConsoleFormatter.WriteLine($"{employeesJson.Count:Yellow} employees received");
            await SaveData(employeesJson, employeesUrl, "employees.json");

            employeesIds = employeesJson
                .Select(json =>
                    json.TryGetProperty("id", out var idString) &&
                    idString.TryGetInt32(out var employeeId)
                        ? employeeId
                        : default(int?))
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();
        }
        catch(Exception ex)
        {
            ConsoleFormatter.WriteLine(
                $"{"Error":Red} occurred while saving data from {employeesUrl:Red}: {ex.Message}");
        }

        var totalEmployees = employeesIds.Count;

        for (var index = 0; index < totalEmployees; index++)
        {
            var employeeId = employeesIds[index];
            ConsoleFormatter.WriteLine($"\nStart receiving data for employee {employeeId}");

            var employeeRequestUri = $"/api/public/v2/employees/{employeeId}";
            try
            {
                ConsoleFormatter.WriteLine($"Start receiving data from {employeeRequestUri:White}");

                var response = await _peopleForceApiService.GetDataAsJsonElementByUri(employeeRequestUri, cancellationToken);
                ConsoleFormatter.WriteLine($"Employee details received");

                await SaveData(response, employeeRequestUri,
                    $"employee_{employeeId}.json");

                await SaveEmployeeDetails(employeeId, cancellationToken);

                await Task.Delay(250);
            }
            catch (Exception ex)
            {
                ConsoleFormatter.WriteLine(
                    $"{"Error":Red} occurred while saving data from {employeeRequestUri:Red}: {ex.Message}");
            }
            ConsoleFormatter.WriteLine(
                $"{"All data was saved":Green} for employee {employeeId:Yellow}. Imported: {index}/{totalEmployees}");
        }
    }

    private static async Task SaveEmployeeDetails(int employeeId, CancellationToken cancellationToken)
    {
        foreach (var endpoint in EmployeeUris)
        {
            var requestUri = endpoint.Uri.Replace("{employee_id}", employeeId.ToString(),
                StringComparison.CurrentCulture);

            try
            {
                ConsoleFormatter.WriteLine($"Start receiving data from {requestUri:White}");

                var response = await _peopleForceApiService.GetDataAsJsonArrayByUri(requestUri, cancellationToken, endpoint.NeedPagination);
                ConsoleFormatter.WriteLine($"{response.Count:Yellow} items received");
                if (response.Count > 0)
                {
                    await SaveData(response, endpoint.Uri,
                        $"employee_{employeeId}_" + endpoint.Uri.Split("/").Last() + ".json");
                }
            }
            catch (Exception ex)
            {
                ConsoleFormatter.WriteLine(
                    $"{"Error":Red} occurred while saving data from {requestUri:Red} for employee {employeeId:Yellow}: {ex.Message}");
            }

            await Task.Delay(250);
        }
    }

    private static async Task SaveVacanciesData(CancellationToken cancellationToken)
    {
        ConsoleFormatter.WriteLine($"\nStart receiving data from {VacancyUri:Yellow}");

        try
        {
            var vacanciesJson = await _peopleForceApiService.GetDataAsJsonArrayByUri(VacancyUri, cancellationToken, true);
            ConsoleFormatter.WriteLine($"{vacanciesJson.Count:Yellow} items received");

            if (vacanciesJson.Count > 0)
            {
                await SaveData(vacanciesJson, VacancyUri, "vacancies.json");
            }

            var totalVacancies = vacanciesJson.Count;
            for (var index = 0; index < totalVacancies; index++)
            {
                var vacancyJson = vacanciesJson[index];
                if (!(vacancyJson.TryGetProperty("id", out var idString) &&
                        idString.TryGetInt32(out var vacancyId)))
                {
                    throw new JsonException("Can't parse field id for vacancy");
                }

                ConsoleFormatter.WriteLine($"\nStart receiving data for vacancy with PF id {vacancyId}");

                foreach (var requestInfo in VacancyUris)
                {
                    var requestUri = requestInfo.Uri.Replace("{vacancy_id}", vacancyId.ToString(),
                        StringComparison.CurrentCulture);
                    try
                    {
                        ConsoleFormatter.WriteLine($"Start receiving data from {requestUri:White}");

                        var response = await _peopleForceApiService
                            .GetDataAsJsonArrayByUri(requestUri, cancellationToken, requestInfo.NeedPagination);
                        ConsoleFormatter.WriteLine($"{response.Count:Yellow} items received");

                        if (response.Count > 0)
                        {
                            await SaveData(response, requestUri,
                                $"vacancy_{vacancyId}_" + requestInfo.Uri.Split("/").Last() + ".json");
                        }
                    }
                    catch (Exception ex)
                    {
                        ConsoleFormatter.WriteLine(
                            $"{"Error":Red} occurred while saving data from {requestUri:Red}: {ex.Message}");
                    }

                    await Task.Delay(250);
                }

                // vacancy pipelines                
                var vacancyPipelineRequestUri = VacancyPipelineUri.Replace("{vacancy_id}", vacancyId.ToString(),
                    StringComparison.CurrentCulture);
                try
                {
                    ConsoleFormatter.WriteLine($"Start receiving data from {vacancyPipelineRequestUri:White}");

                    var response = await _peopleForceApiService
                        .GetDataAsJsonElementByUri(vacancyPipelineRequestUri, cancellationToken);
                    ConsoleFormatter.WriteLine($"Vacancy pipeline was received");

                    await SaveData(response, vacancyPipelineRequestUri,
                        $"vacancy_{vacancyId}_" + vacancyPipelineRequestUri.Split("/").Last() + ".json");                    
                }
                catch (Exception ex)
                {
                    ConsoleFormatter.WriteLine(
                        $"{"Error":Red} occurred while saving data from {vacancyPipelineRequestUri:Red}: {ex.Message}");
                }

                await Task.Delay(250);

                ConsoleFormatter.WriteLine($"All data was saved");
                ConsoleFormatter.WriteLine($"{"All data was saved":Green} for vacancy {vacancyId:Yellow}. Imported: {index}/{totalVacancies}");
            }
        }
        catch (Exception ex)
        {
            ConsoleFormatter.WriteLine($"{"Error":Red} occurred while saving data for vacancies: {ex.Message}");
        }
    }

    private static async Task<List<int>?> SaveCandidatesData(CancellationToken cancellationToken)
    {
        try
        {
            var candidatesJson = await _peopleForceApiService.GetDataAsJsonArrayByUri(CandidateUri, cancellationToken, true);
            ConsoleFormatter.WriteLine($"{candidatesJson.Count:Yellow} items received");

            if (candidatesJson.Count > 0)
            {
                await SaveData(candidatesJson, CandidateUri, "candidates.json");
            }

            var totalCandidates = candidatesJson.Count;
            var candidateIds = new List<int>(totalCandidates);
            for (var index = 0; index < totalCandidates; index++)
            {
                var candidateJson = candidatesJson[index];
                if (!(candidateJson.TryGetProperty("id", out var idString) &&
                        idString.TryGetInt32(out var candidateId)))
                {
                    throw new JsonException("Can't parse field id for candidate");
                }

                candidateIds.Add(candidateId);

                ConsoleFormatter.WriteLine($"\nStart receiving data for candidate with PF id {candidateId}");

                foreach (var requestInfo in CandidateUris)
                {
                    var requestUri = requestInfo.Uri.Replace("{candidate_id}", candidateId.ToString(),
                        StringComparison.CurrentCulture);

                    try
                    {
                        ConsoleFormatter.WriteLine($"Start receiving data from {requestUri:White}");

                        var response = await _peopleForceApiService.GetDataAsJsonArrayByUri(requestUri, cancellationToken,
                            requestInfo.NeedPagination);
                        ConsoleFormatter.WriteLine($"{response.Count:Yellow} items received");

                        if (response.Count > 0)
                        {
                            await SaveData(response, requestUri,
                                $"candidate_{candidateId}_" + requestInfo.Uri.Split("/").Last() + ".json");
                        }

                        await Task.Delay(250);
                    }
                    catch (Exception ex)
                    {
                        ConsoleFormatter.WriteLine(
                            $"{"Error":Red} occurred while saving data from {requestUri:Red}: {ex.Message}");
                    }
                }

                ConsoleFormatter.WriteLine($"{"All data was saved":Green} for candidate {candidateId:Yellow}. Imported: {index}/{totalCandidates}");
            }

            return candidateIds;
        }
        catch (Exception ex)
        {
            ConsoleFormatter.WriteLine($"{"Error":Red} occurred while saving data for candidates: {ex.Message}");
            return null;
        }
    }

    private static async Task SaveCandidateDetails(List<int>? candidateIds, CancellationToken cancellationToken)
    {
        if (candidateIds == null)
        {
            var candidatesJson = await _peopleForceApiService.GetDataAsJsonArrayByUri(CandidateUri, cancellationToken, true);
            candidateIds = candidatesJson
                .Select(json =>
                    json.TryGetProperty("id", out var idString) &&
                    idString.TryGetInt32(out var candidateId)
                        ? candidateId
                        : default(int?))
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();
        }

        candidateIds ??= [];
        var totalCandidates = candidateIds.Count;

        for (var index = 0; index < candidateIds.Count; index++)
        {
            var candidateId = candidateIds[index];
            ConsoleFormatter.WriteLine($"\nStart receiving data for candidate {candidateId}");

            var requestUri = $"/api/public/v3/recruitment/candidates/{candidateId}";
            try
            {
                ConsoleFormatter.WriteLine($"Start receiving data from {requestUri:White}");

                var response = await _peopleForceApiService.GetDataAsJsonElementByUri(requestUri, cancellationToken);
                ConsoleFormatter.WriteLine($"Candidate details received");

                await SaveData(response, requestUri,
                    $"candidate_details_{candidateId}.json");

                // candidate's resume
                if (!response.TryGetProperty("resume", out var hasResume))
                {
                    throw new JsonException("Can't parse field resume field");
                }

                if (hasResume.GetBoolean())
                {
                    if (!response.TryGetProperty("resume_url", out var resumeUrl))
                    {
                        throw new JsonException("Can't parse field resume_url field");
                    }

                    var fileName = $"{Guid.CreateVersion7()}.pdf";
                    await SaveFileAsync(resumeUrl.GetString()!, fileName, cancellationToken);
                }

                await Task.Delay(250);
            }
            catch (Exception ex)
            {
                ConsoleFormatter.WriteLine(
                    $"{"Error":Red} occurred while saving data from {requestUri:Red}: {ex.Message}");
            }

            ConsoleFormatter.WriteLine(
                $"{"All data was saved":Green} for candidate {candidateId:Yellow}. Imported: {index}/{totalCandidates}");
        }
    }

    private static async Task SaveLeaveRequestsData(CancellationToken cancellationToken)
    {
        ConsoleFormatter.WriteLine($"\nStart receiving data from {LeaveRequestsUri.Uri:White}");

        try
        {
            var leavesJson = await _peopleForceApiService.GetDataAsJsonArrayByUri(LeaveRequestsUri.Uri, cancellationToken, true);
            ConsoleFormatter.WriteLine($"{leavesJson.Count:Yellow} items received");

            await SaveData(leavesJson, LeaveRequestsUri.Uri, "leave_requests.json");

            foreach (var leaveJson in leavesJson)
            {
                if (!leaveJson.TryGetProperty("attachments", out var attachments))
                {
                    throw new JsonException("Can't parse field attachments for leave");
                }

                foreach (var attachment in attachments.EnumerateArray())
                {
                    var attachmentUrl = new JsonElement();
                    try
                    {
                        if (!attachment.TryGetProperty("url", out attachmentUrl) ||
                            !attachment.TryGetProperty("filename", out var attachmentFileName))
                        {
                            throw new JsonException("Can't parse attachment url of filename");
                        }

                        var fileName = attachmentFileName.ToString().IsNullOrEmpty()
                            ? $"{Guid.CreateVersion7()}_{attachmentFileName}"
                            : $"{Guid.CreateVersion7()}.jpg";

                        await SaveFileAsync(attachmentUrl.GetString()!, fileName, cancellationToken);
                        await Task.Delay(250, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        ConsoleFormatter.WriteLine($"{"Error":Red} occurred while saving data from {attachmentUrl}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ConsoleFormatter.WriteLine($"{"Error":Red} occurred while saving data for requests: {ex.Message}");
        }
    }

    private static async Task SaveFileAsync(
        string attachmentUrl, string fileName,
        CancellationToken cancellationToken)
    {
        var fileContent = await _peopleForceApiService.GetFileContentById(new Uri(attachmentUrl), cancellationToken);
        var filePath = Path.Combine(DocumentsDirectory, fileName);

        await File.WriteAllBytesAsync(filePath, fileContent);

        var line = $"{attachmentUrl},{filePath}";
        using (var streamWriter = new StreamWriter(FilePathUrlMappingFilePath, append: true, encoding: Encoding.UTF8))
        {
            await streamWriter.WriteLineAsync(line);
        }

        ConsoleFormatter.WriteLine($"File {fileName} was {"saved":Green}");
    }

    private static async Task SaveData(List<JsonElement> response, string requestUri, string fileName)
    {
        var data = JsonSerializer.Serialize(response, _jsonSerializerOptions);
        await File.WriteAllTextAsync(Path.Combine(RootDirectory, fileName), data, Encoding.UTF8);
        ConsoleFormatter.WriteLine($"All data was {"saved":Green} to {fileName:Red}");
    }

    private static async Task SaveData(JsonElement response, string requestUri, string fileName)
    {
        var data = JsonSerializer.Serialize(response, _jsonSerializerOptions);
        await File.WriteAllTextAsync(Path.Combine(RootDirectory, fileName), data, Encoding.UTF8);
        ConsoleFormatter.WriteLine($"All data was {"saved":Green} to {fileName:Red}");
    }

    // Uri
    private static readonly RequestInfoDto[] GeneralUris =
    [
        new("/api/public/v2/employees/birthdays", true),
        new("/api/public/v2/employee_fields", true),
        new("/api/public/v2/employee_tables", true),
        new("/api/public/v2/leave_types", true),
        new("/api/public/v2/leave_policies", true),
        new("/api/public/v2/tasks", true),
        new("/api/public/v2/locations", true),
        new("/api/public/v2/divisions", true),
        new("/api/public/v2/employment_types", true),
        new("/api/public/v2/compensation_types", true),
        new("/api/public/v2/departments", true),
        new("/api/public/v2/holiday_policies", true),
        new("/api/public/v2/holidays", true),
        new("/api/public/v2/positions", true),
        new("/api/public/v2/skills", true),
        new("/api/public/v2/recruitment/vacancy_fields", false),
        new("/api/public/v2/recruitment/pipelines", true),
        new("/api/public/v2/recruitment/sources", true),
        new("/api/public/v2/recruitment/candidate_fields", true),
        new("/api/public/v2/recruitment/disqualify_reasons", true),
        new("/api/public/v2/assets", true),
        new("/api/public/v2/asset_categories", true),
        new("/api/public/v2/termination_types", true),
        new("/api/public/v2/termination_reasons", true),
        new("/api/public/v2/probation_policies", true),
        new("/api/public/v2/teams", true),
        new("/api/public/v2/time/timesheets", true),
        new("/api/public/v2/time/projects", true),
        new("/api/public/v2/time/timesheet_entries", false),
        new("/api/public/v2/document_folders", false),
        new("/api/public/v2/job_levels", false),
        new("/api/public/v2/pay_schedules", false),
        new("/api/public/v2/working_patterns", false),
        new("/api/public/v2/performance/key_performance_indicators", true),
        new("/api/public/v2/performance/objectives", true),
        new("/api/public/v2/competencies", true),
        new("/api/public/v2/external_users", true),
    ];

    private const string VacancyUri = "/api/public/v2/recruitment/vacancies"; // pagination
    private const string CandidateUri = "/api/public/v2/recruitment/candidates"; // pagination

    private static readonly string DocumentsUri = "/api/public/v2/employees/{employee_id}/documents";

    private static readonly RequestInfoDto[] EmployeeUris =
    [
        new("/api/public/v2/employees/{employee_id}/leave_balances", false),
        //new("/api/public/v2/employees/{employee_id}/dependents", false),
        new("/api/public/v2/employees/{employee_id}/leave_types", false),
        //new("/api/public/v2/employees/{employee_id}/educations", false),
        //new("/api/public/v2/employees/{employee_id}/emergency_contacts", false),
        //new("/api/public/v2/employees/{employee_id}/field_histories", true),
        //new("/api/public/v2/employees/{employee_id}/assets", true),
        //new("/api/public/v2/employees/{employee_id}/notes", false),
        //new("/api/public/v2/employees/{employee_id}/tasks", false),
        //new("/api/public/v2/employees/{employee_id}/skills", false),
        new("/api/public/v2/employees/{employee_id}/positions", false),
        new("/api/public/v2/employees/{employee_id}/employment_statuses", true),
    ];

    private static readonly RequestInfoDto[] VacancyUris =
    [
        new("/api/public/v2/recruitment/vacancies/{vacancy_id}/applications", false),
    ];

    private static readonly string VacancyPipelineUri = "/api/public/v2/recruitment/vacancies/{vacancy_id}/pipeline";

    private static readonly RequestInfoDto[] CandidateUris =
    [
        new("/api/public/v2/recruitment/candidates/{candidate_id}/notes", false),
        new("/api/public/v2/recruitment/candidates/{candidate_id}/educations", false),
        new("/api/public/v2/recruitment/candidates/{candidate_id}/experiences", false),
    ];

    private static readonly RequestInfoDto LeaveRequestsUri = new("/api/public/v2/leave_requests", true);

    private sealed record RequestInfoDto(string Uri, bool NeedPagination);
}
