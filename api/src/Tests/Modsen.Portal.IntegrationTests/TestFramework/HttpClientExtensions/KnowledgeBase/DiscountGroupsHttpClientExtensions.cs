using System.Net.Http.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;

internal static class DiscountGroupsHttpClientExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<HttpResponseMessage> GetDiscountGroupsAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync(
            "api/discount-categories",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetDiscountGroupAsHttpMessage(this HttpClient client, int id)
    {
        return await client.GetAsync(
            $"api/discount-categories/{id}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> DeleteDiscountGroupAsHttpMessage(this HttpClient client, int id)
    {
        return await client.DeleteAsync(
            $"api/discount-categories/{id}",
            _cancellationToken);
    }

    public static async Task<DiscountCategoryShortOutput?> CreateDiscountGroup(this HttpClient client,
        string name = "")
    {
        var discountGroupToCreate = new DiscountCategoryInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
        };

        return await client.PostAsJsonAsync<DiscountCategoryInput, DiscountCategoryShortOutput>(
            "api/discount-categories",
            discountGroupToCreate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateDiscountGroupAsHttpMessage(this HttpClient client,
        string name = "")
    {
        var discountGroupToCreate = new DiscountCategoryInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
        };

        return await client.PostAsJsonAsync(
            "api/discount-categories",
            discountGroupToCreate,
            _cancellationToken);
    }

    public static async Task<DiscountCategoryShortOutput?> UpdateDiscountGroup(this HttpClient client,
        int id,
        string name)
    {
        var discountGroupToUpdate = new DiscountCategoryInput
        {
            Name = name
        };

        return await client.PutAsJsonAsync<DiscountCategoryInput, DiscountCategoryShortOutput>(
            $"api/discount-categories/{id}",
            discountGroupToUpdate,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> UpdateDiscountGroupAsHttpMessage(this HttpClient client,
        int id,
        string name)
    {
        var discountGroupToUpdate = new DiscountCategoryInput
        {
            Name = name
        };

        return await client.PutAsJsonAsync(
            $"api/discount-categories/{id}",
            discountGroupToUpdate,
            _cancellationToken);
    }
}
