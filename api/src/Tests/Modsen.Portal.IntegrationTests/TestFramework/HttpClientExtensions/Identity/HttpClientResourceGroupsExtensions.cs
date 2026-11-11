using System.Net.Http.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Identity.DataAccess.Npgsql.OData;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;

internal static class HttpClientResourceGroupsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<List<DepartmentOutput>> GetDepartments(this HttpClient client)
    {
        return await client.GetFromJsonAsync<List<DepartmentOutput>>("api/Departments", IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetDepartmentsAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync("api/Departments", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetResourceGroupAsHttpMessageAsync(this HttpClient client, int id)
    {
        return client.GetAsync($"api/resource-groups/{id}", _cancellationToken);
    }

    public static Task<ResourceGroupOutput?> GetResourceGroupAsync(this HttpClient client, int id)
    {
        return client.GetFromJsonAsync<ResourceGroupOutput>(
            $"api/resource-groups/{id}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<CreateResourceGroupOutput> CreateResourceGroup(this HttpClient client,
        string name = "",
        string departmentId = "Net",
        int? managerId = null)
    {
        var createGroupModel = new CreateResourceGroupInput
        {
            GroupName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            GroupManagerId = managerId
        };

        return client.PostAsJsonAsync<CreateResourceGroupInput, CreateResourceGroupOutput>(
            $"api/departments/{departmentId}/resource-groups",
            createGroupModel,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateResourceGroupAsHttpMessageAsync(this HttpClient client,
        string name = "",
        string departmentId = "Net",
        int? managerId = null)
    {
        var createGroupModel = new CreateResourceGroupInput
        {
            GroupName = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            GroupManagerId = managerId
        };

        return client.PostAsJsonAsync(
            $"api/departments/{departmentId}/resource-groups",
            createGroupModel,
            _cancellationToken);
    }

    public static Task<ResourceGroupOutput> UpdateResourceGroupAsync(this HttpClient client,
        int id,
        string departmentId = "Net",
        string name = "",
        int? managerId = null)
    {
        var input = new UpdateResourceGroupInput
        {
            DepartmentId = departmentId,
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            GroupManagerId = managerId
        };

        return client.PutAsJsonAsync<UpdateResourceGroupInput, ResourceGroupOutput>(
            $"api/resource-groups/{id}",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateResourceGroupAsHttpMessageAsync(this HttpClient client,
        int id,
        string departmentId = "Net",
        string name = "",
        int? managerId = null)
    {
        var input = new UpdateResourceGroupInput
        {
            DepartmentId = departmentId,
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            GroupManagerId = managerId
        };

        return client.PutAsJsonAsync(
            $"api/resource-groups/{id}",
            input,
            _cancellationToken);
    }

    public static Task<PageableListOfItems<OdpGroupMember>> GetResourceGroupMembersAsync(this HttpClient client,
        int id,
        int top = 25,
        int skip = 0)
    {
        return client.GetFromJsonAsync<PageableListOfItems<OdpGroupMember>>(
            $"api/resource-groups/{id}/users?top={top}&skip={skip}",
            _cancellationToken);
    }

    public static Task<UserDto> AddUserToResourceGroupAsync(this HttpClient client,
        int groupId,
        int userId)
    {
        return client.PostAsJsonAsync<int, UserDto>(
            $"api/resource-groups/{groupId}/users",
            userId,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> AddUserToResourceGroupAsHttpMessageAsync(this HttpClient client,
        int groupId,
        int userId)
    {
        return client.PostAsJsonAsync(
            $"api/resource-groups/{groupId}/users",
            userId,
            _cancellationToken);
    }

    public static Task<UserDto> DeleteUserFromResourceGroupAsync(this HttpClient client,
        int groupId,
        int userId)
    {
        return client.DeleteAsync<UserDto>(
            $"api/resource-groups/{groupId}/users/{userId}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteUserFromResourceGroupAsHttpMessageAsync(this HttpClient client,
        int groupId,
        int userId)
    {
        return client.DeleteAsync(
            $"api/resource-groups/{groupId}/users/{userId}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> DeleteResourceGroup(this HttpClient client,
        int id)
    {
        return await client.DeleteAsync($"api/resource-groups/{id}", _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetResourceGroupsLookup(this HttpClient client,
        string query)
    {
        return client.GetAsync($"api/resource-groups/lookup{query}", _cancellationToken);
    }
}
