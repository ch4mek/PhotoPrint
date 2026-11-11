using System.Net.Http.Json;
using System.Text;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

public static class HttpClientClientsExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<ClientOutput?> GetClientAsync(this HttpClient client, int id)
    {
        return client.GetFromJsonAsync<ClientOutput>(
            $"api/Clients/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetClientAsHttpMessageAsync(this HttpClient client, int id)
    {
        return client.GetAsync(
            $"api/Clients/{id}",
            _cancellationToken);
    }

    public static Task<PageableListOfItems<ClientOutput>> GetClientsWithSearchAsync(this HttpClient client,
        string? filter = null,
        string? orderBy = null,
        int top = 25,
        int skip = 0)
    {
        var orderBySection = orderBy.IsNullOrEmpty() ? "" : $"orderBy={orderBy}&";
        var filterSection = filter.IsNullOrEmpty() ? "" : $"filter={filter}&";

        return client.GetFromJsonAsync<PageableListOfItems<ClientOutput>>(
            $"api/Clients/search?top={top}&skip={skip}&{orderBySection}{filterSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetClientsWithSearchAsHttpMessageAsync(this HttpClient client,
        string? filter = null,
        string? orderBy = null,
        int top = 25,
        int skip = 0)
    {
        var orderBySection = orderBy.IsNullOrEmpty() ? "" : $"orderBy={orderBy}&";
        var filterSection = filter.IsNullOrEmpty() ? "" : $"filter={filter}&";

        return client.GetAsync(
            $"api/Clients/search?top={top}&skip={skip}&{orderBySection}{filterSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetClientsWithRawSearchAsHttpMessageAsync(this HttpClient client,
        string query)
    {
        return client.GetAsync(
            $"api/Clients/search{query}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetClientsWithSearchAsPostMethodAsync(this HttpClient client,
        string query)
    {
        using var content = new StringContent(query, Encoding.UTF8, "text/plain");

        return await client.PostAsync("api/Clients/search/$query", content, _cancellationToken);
    }

    public static Task<List<ClientOutput>?> GetClientsLookupAsync(this HttpClient client,
        string? keyWord = null,
        bool? isActive = null,
        bool? onlyManagedClients = null)
    {
        var onlyManagedClientsSection = onlyManagedClients switch
        {
            null => string.Empty,
            _ => $"onlyManagedClients={onlyManagedClients}&"
        };

        var keyWordSection = keyWord is null ? string.Empty : $"keyWord={keyWord}&";

        return client.GetFromJsonAsync<List<ClientOutput>>(
            $"api/clients/lookup?{onlyManagedClientsSection}{keyWordSection}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetClientsLookupAsHttpMessageAsync(this HttpClient client,
        string? keyWord = null,
        bool? onlyManagedClients = null)
    {
        var onlyManagedClientsSection = onlyManagedClients switch
        {
            null => string.Empty,
            _ => $"onlyManagedClients={onlyManagedClients}&"
        };

        var keyWordSection = keyWord is null ? string.Empty : $"keyWord={keyWord}&";

        return client.GetAsync(
            $"api/clients/lookup?{onlyManagedClientsSection}{keyWordSection}",
            _cancellationToken);
    }

    public static Task<CreateClientOutput?> CreateClientAsync(this HttpClient client,
        string? name = null,
        string? color = "#000000",
        string? comments = "Comment",
        int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
        string? legalEntityName = null)
    {
        name = name ?? Guid.CreateVersion7().ToAlphabetic();

        var input = new CreateClientInput
        {
            Name = name,
            Color = color,
            Comments = comments,
            RegisteredLocation = registeredLocationId,
            LegalEntityName = legalEntityName ?? name
        };

        return client.PostAsJsonAsync<CreateClientInput, CreateClientOutput>(
            "api/clients",
            input,
            _cancellationToken);
    }

    public static Task<CreateClientOutput?> CreateClientAsync(this HttpClient client,
        CreateClientInput input)
    {
        return client.PostAsJsonAsync<CreateClientInput, CreateClientOutput>(
            "api/clients",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateClientAsHttpMessageAsync(this HttpClient client,
        string? name = null,
        string? color = "#000000",
        string? comments = "Comment",
        int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId)
    {
        var input = new CreateClientInput
        {
            Name = name ?? Guid.CreateVersion7().ToAlphabetic(),
            Color = color,
            Comments = comments,
            RegisteredLocation = registeredLocationId
        };

        return client.PostAsJsonAsync(
            "api/clients",
            input,
            _cancellationToken);
    }

    public static Task<ClientOutput> UpdateClientAsync(this HttpClient client,
        int id,
        string? name = null,
        string? color = "#000000",
        string? comments = "Comment",
        int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId)
    {
        var input = new UpdateClientInput
        {
            Id = id,
            Name = name ?? Guid.CreateVersion7().ToAlphabetic(),
            Color = color,
            Comments = comments,
            RegisteredLocation = registeredLocationId
        };

        return client.PutAsJsonAsync<UpdateClientInput, ClientOutput>(
            "api/clients",
            input,
            _cancellationToken);
    }

    public static Task<ClientOutput> UpdateClientAsync(this HttpClient client, UpdateClientInput input)
    {
        return client.PutAsJsonAsync<UpdateClientInput, ClientOutput>(
            "api/clients",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateClientAsHttpMessageAsync(this HttpClient client,
        int id,
        string? name = null,
        string? color = "#000000",
        string? comments = "Comment",
        int registeredLocationId = PortalSharedConstants.DefaultRegisteredLocationId)
    {
        var input = new UpdateClientInput
        {
            Id = id,
            Name = name ?? Guid.CreateVersion7().ToAlphabetic(),
            Color = color,
            Comments = comments,
            RegisteredLocation = registeredLocationId
        };

        return client.PutAsJsonAsync(
            "api/clients",
            input,
            _cancellationToken);
    }

    public static async Task<ClientStatusOutput> SetClientInactiveAsync(this HttpClient client, int id)
    {
        var input = new ClientStatusInput
        {
            IsActive = false
        };

        var response = await client.PatchAsJsonAsync(
            $"api/clients/{id}/IsActive",
            input,
            _cancellationToken);

        return await response.Content.ReadFromJsonAsync<ClientStatusOutput>(cancellationToken: _cancellationToken);
    }

    public static Task<HttpResponseMessage> SetClientInactiveAsHttpMessageAsync(this HttpClient client, int id)
    {
        var input = new ClientStatusInput
        {
            IsActive = false
        };

        return client.PatchAsJsonAsync(
            $"api/clients/{id}/IsActive",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> SetClientActiveAsHttpMessageAsync(this HttpClient client, int id)
    {
        var input = new ClientStatusInput
        {
            IsActive = true
        };

        return client.PatchAsJsonAsync(
            $"api/clients/{id}/IsActive",
            input,
            _cancellationToken);
    }
}
