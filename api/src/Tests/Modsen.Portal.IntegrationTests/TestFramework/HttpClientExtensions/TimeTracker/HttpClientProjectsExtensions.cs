using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

internal static class HttpClientProjectsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<ProjectOutput?> CreateProject(this HttpClient client,
        int clientId,
        string name = "",
        bool isPublic = false,
        bool isBillable = true,
        int? managerId = null,
        string? color = null)
    {
        var projectWithUserInput = new CreateProjectInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            ClientId = clientId,
            IsPublic = isPublic,
            IsBillable = isBillable,
            ManagerId = managerId,
            Color = color
        };

        return await client.PostAsJsonAsync<CreateProjectInput, ProjectOutput>(
            "api/projects",
            projectWithUserInput,
            _cancellationToken);
    }

    public static async Task<ProjectOutput?> CreateProject(this HttpClient client,
        CreateProjectInput input)
    {
        return await client.PostAsJsonAsync<CreateProjectInput, ProjectOutput>(
            "api/projects",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateProjectAsHttpMessageAsync(this HttpClient client,
        CreateProjectInput input)
    {
        return client.PostAsJsonAsync(
            "api/projects",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateProjectAsHttpMessageAsync(this HttpClient client,
        int clientId,
        string name = "",
        bool isPublic = false,
        bool isBillable = true,
        int? managerId = null,
        string? color = null)
    {
        var projectWithUserInput = new CreateProjectInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            ClientId = clientId,
            IsPublic = isPublic,
            IsBillable = isBillable,
            ManagerId = managerId,
            Color = color
        };

        return client.PostAsJsonAsync(
            "api/projects",
            projectWithUserInput,
            _cancellationToken);
    }

    public static Task<UpdateProjectOutput> UpdateProjectAsync(this HttpClient client, UpdateProjectInput input)
    {
        return client.PutAsJsonAsync<UpdateProjectInput, UpdateProjectOutput>(
            "api/projects", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateProjectAsHttpMessageAsync(this HttpClient client,
        UpdateProjectInput input)
    {
        return client.PutAsJsonAsync("api/projects", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<UserProjectAssignmentOutput?> CreateProjectAssignment(this HttpClient client,
        int userId,
        int projectId,
        DateOnly? onProjectFrom = null,
        DateOnly? onProjectTo = null,
        decimal fte = 1,
        bool isTimeBillable = true,
        string seniorityLevelId = "SeniorL3")
    {
        var userProjectAssignmentToCreate = new CreateUserProjectAssignmentInput(
            UserOnProjectFrom: onProjectFrom ?? DateOnly.FromDateTime(DateTime.UtcNow),
            UserOnProjectTo: onProjectTo,
            IsTimeOnProjectBillable: isTimeBillable,
            Fte: fte,
            UserOnProjectSeniorityLevelId: seniorityLevelId);

        return await client.PostAsJsonAsync<CreateUserProjectAssignmentInput, UserProjectAssignmentOutput>(
            $"api/users/{userId}/projects/{projectId}",
            userProjectAssignmentToCreate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateProjectAssignmentAsHttpMessageAsync(this HttpClient client,
        int userId,
        int projectId,
        DateOnly? onProjectFrom = null,
        DateOnly? onProjectTo = null,
        decimal fte = 1,
        bool isTimeBillable = true,
        string seniorityLevelId = "SeniorL3")
    {
        var userProjectAssignmentToCreate = new CreateUserProjectAssignmentInput(
            UserOnProjectFrom: onProjectFrom ?? DateOnly.FromDateTime(DateTime.UtcNow),
            UserOnProjectTo: onProjectTo,
            IsTimeOnProjectBillable: isTimeBillable,
            Fte: fte,
            UserOnProjectSeniorityLevelId: seniorityLevelId);

        return await client.PostAsJsonAsync(
            $"api/users/{userId}/projects/{projectId}",
            userProjectAssignmentToCreate,
            _cancellationToken);
    }

    public static Task<UserProjectWithNamesDto> GetProjectAssignmentAsync(
        this HttpClient client,
        int id)
    {
        return client.GetFromJsonAsync<UserProjectWithNamesDto>(
            $"api/user-projects/{id}", IntegrationTestsConstants.DefaultSerializerOptions);
    }

    public static Task<UserProjectWithNamesDto> GetUserProjectsAsync(
        this HttpClient client,
        int id)
    {
        return client.GetFromJsonAsync<UserProjectWithNamesDto>(
            $"api/user-projects/{id}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<OdpProjectUser>> GetProjectUserWithFilterAsync(this HttpClient client,
        string? filter = null,
        int top = 100,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"filter={filter}&";
        return client.GetFromJsonAsync<PageableListOfItems<OdpProjectUser>>(
            $"api/user-projects?top={top}&skip={skip}&{filterSection}",
            IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateProjectAssignmentAsHttpMessageAsync(this HttpClient client,
        UpdateUserProjectAssignmentInput input)
    {
        return client.PutAsJsonAsync("api/user-projects", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateProjectAssignmentAsHttpMessageAsync(this HttpClient client,
        int assignmentId,
        int userId,
        int projectId,
        DateOnly? onProjectFrom = null,
        DateOnly? onProjectTo = null,
        decimal fte = 1,
        bool isTimeBillable = true,
        string seniorityLevelId = "SeniorL3")
    {
        var input = new UpdateUserProjectAssignmentInput(
            Id: assignmentId,
            UserId: userId,
            ProjectId: projectId,
            IsTimeOnProjectBillable: isTimeBillable,
            Fte: fte,
            UserOnProjectSeniorityLevelId: seniorityLevelId,
            UserOnProjectFrom: onProjectFrom ?? DateOnly.FromDateTime(DateTime.UtcNow),
            UserOnProjectTo: onProjectTo);

        return client.PutAsJsonAsync("api/user-projects", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetProjectsWithRawSearchAsHttpMessageAsync(this HttpClient client,
        string query)
    {
        return client.GetAsync(
            new Uri($"api/Projects/search{query}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<PageableListOfItems<ProjectWithUserAndTimeInfoOutput>> GetProjectsWithSearchAsync(this HttpClient client,
        string? filter = null,
        string? orderBy = null,
        int top = 25,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"&filter={filter}";
        var orderbySection = orderBy.IsNullOrEmpty() ? "" : $"&orderby={orderBy}";

        return client.GetFromJsonAsync<PageableListOfItems<ProjectWithUserAndTimeInfoOutput>>(
            new Uri($"api/Projects/search?top={top}&skip={skip}&{filterSection}{orderbySection}", UriKind.Relative),
            _cancellationToken);
    }

    public static async Task<PageableListOfItems<ProjectWithUserAndTimeInfoOutput>> GetProjectsWithSearchAsPostMethodAsync(
        this HttpClient client,
        string filter,
        int top = 25,
        int skip = 0)
    {
        using var content = new StringContent(filter, Encoding.UTF8, "text/plain");
        var response = await client.PostAsync(new Uri($"api/Projects/search/$query?top={top}&skip={skip}",
            UriKind.Relative), content, _cancellationToken);
        return await response.Content.ReadFromJsonAsync<PageableListOfItems<ProjectWithUserAndTimeInfoOutput>>(_cancellationToken);
    }

    public static Task<List<ProjectManagersLookupOutput>> GetProjectManagersLookupAsync(this HttpClient client)
    {
        return client.GetFromJsonAsync<List<ProjectManagersLookupOutput>>(
            "api/projects/managers/lookup",
            _cancellationToken);
    }

    public static Task<ProjectWithManagerOutput> GetProjectByIdAsync(this HttpClient client, int id)
    {
        return client.GetFromJsonAsync<ProjectWithManagerOutput>(
            new Uri($"api/projects/{id}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<List<ClientProjectOutput>> GetProjectsByClientIdAsync(this HttpClient client, int id,
        bool? onlyManagedProjects = null)
    {
        var onlyManagedProjectsSection = onlyManagedProjects switch
        {
            null => "",
            _ => $"onlyManagedProjects={onlyManagedProjects.ToString()}"
        };

        return client.GetFromJsonAsync<List<ClientProjectOutput>>(
            $"/api/Clients/{id}/projects?{onlyManagedProjectsSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetProjectByIdAsHttpMessageAsync(this HttpClient client, int id)
    {
        return client.GetAsync(
            new Uri($"api/projects/{id}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<List<ProjectLookupOutput>> GetProjectsByUserIdAsync(this HttpClient client,
        int userId,
        bool? onlyManaged = null,
        bool? isPublic = null,
        DateOnly? trackIntervalFrom = null,
        DateOnly? trackIntervalTo = null)
    {

        var isPublicSection = isPublic is null ? "" : $"isPublic={isPublic.ToString()}&";
        var onlyManagedSection = onlyManaged is null ? ""
            : $"onlyManagedProjects={onlyManaged.ToString()}&";

        var intervalFromSection = trackIntervalFrom switch
        {
            null => "",
            _ => $"trackIntervalFrom={trackIntervalFrom.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&"
        };

        var intervalToSection = trackIntervalTo switch
        {
            null => "",
            _ => $"trackIntervalTo={trackIntervalTo.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&"
        };

        return client.GetFromJsonAsync<List<ProjectLookupOutput>>(
            new Uri($"api/users/{userId}/projects?{onlyManagedSection}{isPublicSection}{intervalFromSection}{intervalToSection}",
                UriKind.Relative),
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetProjectsByUserIdAsHttpMessageAsync(this HttpClient client,
        int userId,
        bool? onlyManaged = null,
        DateOnly? trackIntervalFrom = null,
        DateOnly? trackIntervalTo = null)
    {
        var onlyManagedSection = onlyManaged switch
        {
            null => "",
            _ => $"onlyManagedProjects={onlyManaged.ToString()}&"
        };

        var intervalFromSection = trackIntervalFrom switch
        {
            null => "",
            _ => $"trackIntervalFrom={trackIntervalFrom.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&"
        };

        var intervalToSection = trackIntervalTo switch
        {
            null => "",
            _ => $"trackIntervalTo={trackIntervalTo.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&"
        };

        return client.GetAsync(
            new Uri($"api/users/{userId}/projects?{onlyManagedSection}{intervalFromSection}{intervalToSection}",
                UriKind.Relative),
            _cancellationToken);
    }

    public static Task<List<ProjectWithDisabledDaysDto>> GetProjectsWithDisabledDaysByUserIdAsync(this HttpClient client,
        int userId,
        bool? onlyManaged = null,
        DateOnly? trackIntervalFrom = null,
        DateOnly? trackIntervalTo = null)
    {
        var onlyManagedSection = onlyManaged switch
        {
            null => "",
            _ => $"onlyManagedProjects={onlyManaged.ToString()}&"
        };

        var intervalFromSection = trackIntervalFrom switch
        {
            null => "",
            _ => $"trackIntervalFrom={trackIntervalFrom.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&"
        };

        var intervalToSection = trackIntervalTo switch
        {
            null => "",
            _ => $"trackIntervalTo={trackIntervalTo.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&"
        };

        return client.GetFromJsonAsync<List<ProjectWithDisabledDaysDto>>(
            $"api/users/{userId}/projects/with-disabled-days?{onlyManagedSection}{intervalFromSection}{intervalToSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetProjectsWithDisabledDaysByUserIdAsHttpMessageAsync(
        this HttpClient client,
        int userId)
    {
        return client.GetAsync(new Uri($"api/users/{userId}/projects/with-disabled-days", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<List<ProjectLookupOutput>> GetProjectsLookupAsync(this HttpClient client,
        bool? onlyManagedProjects = null,
        bool? isPublic = null,
        bool? isActive = null,
        string? keyWord = null,
        string? roles = null)
    {
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var isPublicSection = isPublic is null ? "" : $"isPublic={isPublic.ToString()}&";
        var isActiveSection = isActive is null ? "" : $"isActive={isActive.ToString()}&";
        var rolesSection = roles is null ? "" : $"roles={roles}&";

        var onlyManagedProjectsSection = onlyManagedProjects is null ? ""
            : $"onlyManagedProjects={onlyManagedProjects.ToString()}&";

        return client.GetFromJsonAsync<List<ProjectLookupOutput>>(
            $"api/Projects/lookup?{keyWordSection}{rolesSection}" +
            $"{isPublicSection}{onlyManagedProjectsSection}{isActiveSection}",
            _cancellationToken);
    }

    public static Task<List<ProjectSortedLookupOutput>> GetProjectsSortedLookupAsync(this HttpClient client,
        int? userId = null,
        string? keyWord = null)
    {
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";

        return client.GetFromJsonAsync<List<ProjectSortedLookupOutput>>(
            $"api/Projects/sorted-lookup?{keyWordSection}{userIdSection}",
            _cancellationToken);
    }

    public static Task<List<ProjectSortedLookupOutput>> GetPersonalProjectsSortedLookupAsync(this HttpClient client,
        int? userId = null,
        string? keyWord = null)
    {
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";

        return client.GetFromJsonAsync<List<ProjectSortedLookupOutput>>(
            $"api/Projects/sorted-lookup/personal?{keyWordSection}{userIdSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetPersonalProjectsSortedLookupAsHttpMessageAsync(
        this HttpClient client,
        int? userId = null,
        string? keyWord = null)
    {
        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}&";
        var userIdSection = userId is null ? "" : $"userId={userId}&";

        return client.GetAsync(
            $"api/Projects/sorted-lookup/personal?{keyWordSection}{userIdSection}",
            _cancellationToken);
    }

    public static Task<List<UserProjectLookupOutput>>GetUserProjectsLookupAsync(this HttpClient client,
        int userId)
    {
        return client.GetFromJsonAsync<List<UserProjectLookupOutput>>(
            $"api/user-projects/lookup?userId={userId}", IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<bool> SetProjectInactiveAsync(this HttpClient client,
        int id)
    {
        return client.PutAsJsonAsync<ProjectStatusInput, bool>(
            $"api/projects/{id}/isActive",
            new ProjectStatusInput { IsActive = false },
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> SetProjectInactiveAsHttpMessageAsync(this HttpClient client,
        int id)
    {
        return client.PutAsJsonAsync(
            $"api/projects/{id}/isActive",
            new ProjectStatusInput { IsActive = false },
            _cancellationToken);
    }

    public static Task<bool> SetProjectActiveAsync(this HttpClient client,
        int id)
    {
        return client.PutAsJsonAsync<ProjectStatusInput, bool>(
            $"api/projects/{id}/isActive",
            new ProjectStatusInput { IsActive = true },
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> SetProjectActiveAsHttpMessageAsync(this HttpClient client,
        int id)
    {
        return client.PutAsJsonAsync(
            $"api/projects/{id}/isActive",
            new ProjectStatusInput { IsActive = true },
            _cancellationToken);
    }
}
