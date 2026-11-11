using System.Net.Http.Json;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;

internal static class HttpClientExpenseCategoresExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> CreateExpenseCategoryAsResponseMessageAsync(this HttpClient httpClient,
        CreateExpenseCategoryInput input)
    {
        return httpClient.PostAsJsonAsync(
            $"api/expense-categories",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static async Task<ExpenseCategoryCoreDto?> CreateExpenseCategory(this HttpClient httpClient,
        string categoryName)
    {
        var input = new CreateExpenseCategoryInput(categoryName);

        var createdCategory = await httpClient.PostAsJsonAsync<CreateExpenseCategoryInput, ExpenseCategoryCoreDto>(
            "api/expense-categories",
            input,
            _cancellationToken);

        return createdCategory;
    }

    public static Task<HttpResponseMessage> EditExpenseCategoryAsResponseMessageAsync(this HttpClient httpClient,
        int id, EditExpenseCategoryInput input)
    {
        return httpClient.PatchAsJsonAsync(
            $"api/expense-categories/{id}",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteExpenseCategoryAsResponseMessageAsync(this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync(
            new Uri($"api/expense-categories/{id}", UriKind.Relative),
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> ExpenseCategoryLookupAsResponseMessageAsync(this HttpClient httpClient)
    {
        return httpClient.GetAsync(
            new Uri($"api/expense-categories/lookup", UriKind.Relative),            
            _cancellationToken);
    }
}
