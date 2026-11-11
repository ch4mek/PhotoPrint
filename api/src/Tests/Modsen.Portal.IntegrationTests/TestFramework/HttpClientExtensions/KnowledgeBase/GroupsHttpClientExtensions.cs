using System.Net.Http.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;

internal static class GroupsHttpClientExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<ArticleOutput?> CreateArticle(this HttpClient client,
        int groupId,
        string name = "",
        string icon = "",
        string text = "",
        string keywords = "",
        bool isActive = true,
        bool isAvailableOnMainScreen = true,
        List<string>? tags = null)
    {
        var createInput = new CreateArticleInput(
            groupId,
            name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            icon.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : icon,
            text.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : text,
            keywords.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : keywords,
            isActive,
            isAvailableOnMainScreen,
            tags ?? []);

        return await client.PostAsJsonAsync<CreateArticleInput, ArticleOutput>(
            "api/articles",
            createInput,
            _cancellationToken);
    }

    public static async Task<GroupOutput?> CreateGroup(this HttpClient client,
        string name = "",
        string icon = "",
        int? parentGroupId = null,
        bool isActive = true,
        bool isAvailableOnMainScreen = true)
    {
        var createInput = new CreateGroupInput
        (
            Name : name.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : name,
            Icon : icon.IsNullOrEmpty() ? Guid.CreateVersion7().ToAlphabetic() : icon,
            IsActive : isActive,
            IsAvailableOnMainScreen : isAvailableOnMainScreen,
            ParentGroupId : parentGroupId
        );

        return await client.PostAsJsonAsync<CreateGroupInput, GroupOutput>(
            "api/knowledge-groups",
            createInput,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> SetGroupOrderAsHttpMessage(this HttpClient client,
        GroupOrderInput input)
    {
        return await client.PutAsJsonAsync("api/knowledge-groups/order", input, _cancellationToken);
    }
}
