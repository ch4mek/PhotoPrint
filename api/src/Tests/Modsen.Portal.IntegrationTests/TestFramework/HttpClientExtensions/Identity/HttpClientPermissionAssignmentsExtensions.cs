using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;

internal static class HttpClientPermissionAssignmentsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<List<PermissionAssignmentOutput>?> UpdateUserPermissionAssignments(this HttpClient client,
        int userId,
        Dictionary<string, string> resources)
    {
        var updateAssignmentsInput = new UpdateUserPermissionsInput
        {
            UserId = userId,
            PermissionType = PermissionType.TrackBillableWorkingHours,
            Resources = resources,
            ResourceType = PermissionResourceType.Project
        };

         return await client.PutAsJsonAsync<UpdateUserPermissionsInput, List<PermissionAssignmentOutput>>(
            "api/permissions/assignments",
            updateAssignmentsInput,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> UpdateUserPermissionAssignmentsAsHttpMessage(this HttpClient client,
        int userId,
        Dictionary<string, string> resources, JsonSerializerOptions jsonOptions)
    {
        var updateAssignmentsInput = new UpdateUserPermissionsInput
        {
            UserId = userId,
            PermissionType = PermissionType.TrackBillableWorkingHours,
            Resources = resources,
            ResourceType = PermissionResourceType.Project
        };

        return await client.PutAsJsonAsync(
            "api/permissions/assignments",
            updateAssignmentsInput,
            jsonOptions,
            _cancellationToken);
    }

    public static async Task<PageableListOfItems<ProjectPermissionAssignmentOutput>?> GetPermissionAssignments(
        this HttpClient client,
        JsonSerializerOptions jsonOptions,
        string? filter = null,
        int top = 25,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"$filter={filter}";

        return await client.GetFromJsonAsync<PageableListOfItems<ProjectPermissionAssignmentOutput>>(
            new Uri($"api/permissions/assignments?{filterSection}&top={top}&skip={skip}", UriKind.Relative),
            jsonOptions,
            _cancellationToken);
    }

    public static async Task<UserPermissionResourcesOutput?> GetPermissionResources(this HttpClient client,
        int? userId = null,
        PermissionType? permissionType = null,
        PermissionResourceType? resourceType = null)
    {
        var userIdSection = userId.ToString().IsNullOrEmpty() ? "" : $"userId={userId}&";
        var permissionTypeSection = permissionType.HasValue ? $"permissionType={permissionType.Value}&" : "";
        var resourceTypeSection = resourceType.HasValue ? $"resourceType={resourceType.Value}" : "";

        return await client.GetFromJsonAsync<UserPermissionResourcesOutput>(
                new Uri($"api/permissions/assignments/resources?{userIdSection}{permissionTypeSection}{resourceTypeSection}",
                    UriKind.Relative),
                _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetPermissionResourcesAsHttpMessage(this HttpClient client,
        int? userId = null,
        PermissionType? permissionType = null,
        PermissionResourceType? resourceType = null)
    {
        var userIdSection = userId.ToString().IsNullOrEmpty() ? "" : $"userId={userId}&";
        var permissionTypeSection = permissionType.HasValue ? $"permissionType={permissionType.Value}&" : "";
        var resourceTypeSection = resourceType.HasValue ? $"resourceType={resourceType.Value}" : "";

        return await client.GetAsync(
            new Uri($"api/permissions/assignments/resources?{userIdSection}{permissionTypeSection}{resourceTypeSection}",
                UriKind.Relative),
            _cancellationToken);
    }

    public static async Task<PermissionAssignmentOutput?> DeletePermissionAssignment(this HttpClient client,
        int id)
    {
        return await client.DeleteAsync<PermissionAssignmentOutput>(
            $"api/permissions/assignments/{id}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> DeletePermissionAssignmentAsHttpMessage(this HttpClient client,
        int id)
    {
        return await client.DeleteAsync(
            $"api/permissions/assignments/{id}",
            _cancellationToken);
    }
}
