using System.Net.Http.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;

internal static class TagsHttpClientExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<HttpResponseMessage> GetTagsAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync(
            "api/tags",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetArticleTagsAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync(
            "api/articles/tags",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetDiscountTagsAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync(
            "api/discounts/tags",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetTagAsHttpMessage(this HttpClient client, int id)
    {
        return await client.GetAsync(
            $"api/tags/{id}",
            _cancellationToken);
    }

    public static async Task<TagOutput?> CreateTag(this HttpClient client,
        string name = "")
    {
        var tagToCreate = new TagInput(name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name);

        return await client.PostAsJsonAsync<TagInput, TagOutput>(
            "api/tags",
            tagToCreate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateTagAsHttpMessage(this HttpClient client,
        string name = "")
    {
        var tagToCreate = new TagInput(name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name);

        return await client.PostAsJsonAsync(
            "api/tags",
            tagToCreate,
            _cancellationToken);
    }

    public static async Task<TagOutput?> UpdateTag(this HttpClient client,
        int id,
        string name)
    {
        var tagToUpdate = new TagInput(name);

        return await client.PutAsJsonAsync<TagInput, TagOutput>(
            $"api/tags/{id}",
            tagToUpdate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage?> UpdateTagAsHttpMessage(this HttpClient client,
        int id,
        string name)
    {
        var tagToUpdate = new TagInput(name);

        return await client.PutAsJsonAsync(
            $"api/tags/{id}",
            tagToUpdate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage?> DeleteTagAsHttpMessage(this HttpClient client,
        int id)
    {
        return await client.DeleteAsync(
            $"api/tags/{id}",
            _cancellationToken);
    }
}
