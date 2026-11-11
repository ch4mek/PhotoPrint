using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;

internal static class DiscountsHttpClientExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<HttpResponseMessage> DeleteDiscountAsHttpMessage(this HttpClient client, int id)
    {
        return await client.DeleteAsync(
            $"api/discounts/{id}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetDiscountsAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync(
            "api/discounts/search?keywords=key",
            _cancellationToken);
    }

    public static async Task<List<DiscountOutput>?> GetDiscountsByGroup(this HttpClient client, int groupId)
    {
        return await client.GetFromJsonAsync<List<DiscountOutput>>(
            $"api/discount-categories/{groupId}/discounts",
            _cancellationToken);
    }

    public static async Task<DiscountOutput?> CreateDiscount(this HttpClient client,
        int discountCategoryId,
        int discountPercent = 10,
        string name = "",
        string imageUrl = "",
        bool isActive = true,
        string text = "",
        List<int>? discountCityIds = null,
        List<int>? discountTagIds = null,
        List<string>? discountSocialMediaUrls = null,
        JsonSerializerOptions jsonOptions = null!)
    {
        var createDiscountModel = new CreateDiscountInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            DiscountCategoryId = discountCategoryId,
            DiscountPercent = discountPercent,
            ImageUrl = imageUrl.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : imageUrl,
            IsActive = isActive,
            Text = text.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : text,
            DiscountCityIds = discountCityIds ?? [],
            DiscountTagIds = discountTagIds ?? [],
            DiscountSocialMediaUrls = discountSocialMediaUrls ?? []
        };

        return await client.PostAsJsonAsync<CreateDiscountInput, DiscountOutput>(
            "api/discounts",
            createDiscountModel,
            jsonOptions,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateDiscountAsHttpMessage(this HttpClient client,
        int discountCategoryId,
        int discountPercent = 10,
        string name = "",
        string imageUrl = "",
        bool isActive = true,
        string text = "",
        List<int>? discountCityIds = null,
        List<int>? discountTagIds = null,
        List<string>? discountSocialMediaUrls = null)
    {
        var createDiscountModel = new CreateDiscountInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            DiscountCategoryId = discountCategoryId,
            DiscountPercent = discountPercent,
            ImageUrl = imageUrl.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : imageUrl,
            IsActive = isActive,
            Text = text.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : text,
            DiscountCityIds = discountCityIds ?? [],
            DiscountTagIds = discountTagIds ?? [],
            DiscountSocialMediaUrls = discountSocialMediaUrls ?? []
        };

        return await client.PostAsJsonAsync(
            "api/discounts",
            createDiscountModel,
            _cancellationToken);
    }

    public static async Task<DiscountOutput?> UpdateDiscount(this HttpClient client,
        int id,
        int discountCategoryId,
        int discountPercent = 10,
        string name = "",
        string imageUrl = "",
        bool isActive = true,
        string text = "",
        List<int>? discountCityIds = null,
        List<int>? discountTagIds = null,
        List<string>? discountSocialMediaUrls = null)
    {
        var updateDiscountModel = new UpdateDiscountInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            DiscountCategoryId = discountCategoryId,
            DiscountPercent = discountPercent,
            ImageUrl = imageUrl.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : imageUrl,
            IsActive = isActive,
            Text = text.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : text,
            DiscountCityIds = discountCityIds ?? [],
            DiscountTagIds = discountTagIds ?? [],
            DiscountSocialMediaUrls = discountSocialMediaUrls ?? []
        };

        return await client.PutAsJsonAsync<UpdateDiscountInput, DiscountOutput>(
            $"api/discounts/{id}", updateDiscountModel, _cancellationToken);
    }

    public static async Task<HttpResponseMessage> UpdateDiscountAsHttpMessage(this HttpClient client,
        int id,
        int discountCategoryId,
        int discountPercent = 10,
        string name = "",
        string imageUrl = "",
        bool isActive = true,
        string text = "",
        List<int>? discountCityIds = null,
        List<int>? discountTagIds = null,
        List<string>? discountSocialMediaUrls = null)
    {
        var updateDiscountModel = new UpdateDiscountInput
        {
            Name = name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            DiscountCategoryId = discountCategoryId,
            DiscountPercent = discountPercent,
            ImageUrl = imageUrl.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : imageUrl,
            IsActive = isActive,
            Text = text.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : text,
            DiscountCityIds = discountCityIds ?? [],
            DiscountTagIds = discountTagIds ?? [],
            DiscountSocialMediaUrls = discountSocialMediaUrls ?? []
        };

        return await client.PutAsJsonAsync($"api/discounts/{id}", updateDiscountModel, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }
}
