using System.Net.Http.Json;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;

internal static class HttpClientExpensesExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<HttpResponseMessage> CreateExpenseAsResponseMessageAsync(this HttpClient httpClient,
        CreateExpenseInput input)
    {
        return httpClient.PostAsJsonAsync(
            $"api/expenses",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static async Task<ExpenseCoreDto?> CreateExpenseAsync(this HttpClient httpClient,
        int expenseCategoryId, int year, int monthOfYear, decimal expenseAmount)
    {
        var input = new CreateExpenseInput(expenseCategoryId, year, monthOfYear, expenseAmount);

        var createdExpense = await httpClient.PostAsJsonAsync<CreateExpenseInput, ExpenseCoreDto>(
            $"api/expenses",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);

        return createdExpense;
    }

    public static Task<HttpResponseMessage> UpdateExpenseAsResponseMessageAsync(this HttpClient httpClient,
        int id, UpdateExpenseInput input)
    {
        return httpClient.PatchAsJsonAsync(
            $"api/expenses/{id}",
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetExpensesAsResponseMessageAsync(this HttpClient httpClient,
        int year, GetExpensesInput input, int? pageSize = null, int? pageNumber = null)
    {
        var url = $"api/expenses/{year}";

        var query = new List<string>();
        if (pageSize.HasValue)
        {
            query.Add($"pageSize={pageSize.Value}");
        }
        if (pageNumber.HasValue)
        {
            query.Add($"pageNumber={pageNumber.Value}");
        }
        if (query.Count > 0)
        {
            url += "?" + string.Join("&", query);
        }

        return httpClient.PostAsJsonAsync(
            url,
            input,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
