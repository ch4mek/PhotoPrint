using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Finance.Expenses;

[Collection("GeneralCollection")]
public class ExpensesIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ExpensesIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateExpenseWhenCannotManageExpensesResponseWith403()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.CreateExpenseAsResponseMessageAsync(new CreateExpenseInput(1000, 2026, 6, 1000));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateExpenseWhenExpenseCategoryNotFoundResponseWith404()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateExpenseInput(1000, 2026, 6, 1000);

        // Act
        var response = await client.CreateExpenseAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task CreateExpenseWhenExpenseCategoryIsDeletedResponseWith404()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var category = await client.CreateExpenseCategory("CreateExpenseWhenExpenseCategoryIsDeletedResponseWith404 test category");
        await client.DeleteExpenseCategoryAsResponseMessageAsync(category!.Id);

        var input = new CreateExpenseInput(category.Id, 2026, 6, 1000);

        // Act
        var response = await client.CreateExpenseAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task CreateExpenseWhenRequestOkResponseWithExpenseDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var category = await client.CreateExpenseCategory("CreateExpenseWhenRequestOkResponseWithExpenseDto test category");

        var input = new CreateExpenseInput(category!.Id, 2026, 6, 1000);

        // Act
        var response = await client.CreateExpenseAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expense = await response.Content.ReadFromJsonAsync<ExpenseCoreDto>(_jsonOptions, _cancellationToken);
        expense.Should().NotBeNull();
        expense.ExpenseCategoryId.Should().Be(input.ExpenseCategoryId);
        expense.Year.Should().Be(input.Year);
        expense.MonthOfYear.Should().Be(input.MonthOfYear);
        expense.ExpenseAmount.Should().Be(input.ExpenseAmount);
    }

    [Fact]
    public async Task UpdateExpenseWhenExpenseAmountIsNegativeResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.UpdateExpenseAsResponseMessageAsync(1, new UpdateExpenseInput(-10));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var validationFailures = document.RootElement.GetProperty("ValidationFailures");
        validationFailures.EnumerateArray()
            .Should()
            .Contain(x => x.GetProperty("PropertyName").GetString() == nameof(UpdateExpenseInput.ExpenseAmount));        
    }

    [Fact]
    public async Task UpdateExpenseWhenExpenseAmountIsTooLargeResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.UpdateExpenseAsResponseMessageAsync(1, new UpdateExpenseInput(1000000));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var validationFailures = document.RootElement.GetProperty("ValidationFailures");
        validationFailures.EnumerateArray()
            .Should()
            .Contain(x => x.GetProperty("PropertyName").GetString() == nameof(UpdateExpenseInput.ExpenseAmount));
    }

    [Fact]
    public async Task UpdateExpenseWhenExpenseNotFoundResponseWith404()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.UpdateExpenseAsResponseMessageAsync(9999, new UpdateExpenseInput(2000));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseNotFound);
    }

    [Fact]
    public async Task UpdateExpenseWhenRequestOkResponseWithExpenseCoreDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var category = await client.CreateExpenseCategory("UpdateExpenseWhenRequestOkResponseWithExpenseCoreDto test category");
        var existingExpense = await client.CreateExpenseAsync(category!.Id, 2026, 6, 1000);

        var input = new UpdateExpenseInput(10);

        // Act
        var response = await client.UpdateExpenseAsResponseMessageAsync(existingExpense!.Id, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedExpense = await response.Content.ReadFromJsonAsync<ExpenseCoreDto>(_jsonOptions, _cancellationToken);
        updatedExpense!.ExpenseAmount.Should().Be(input.ExpenseAmount);
    }

    [Fact]
    public async Task GetExpensesWhenYearIsLessThan2020ResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetExpensesAsResponseMessageAsync(2019, new GetExpensesInput());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var validationFailures = document.RootElement.GetProperty("ValidationFailures");
        validationFailures.EnumerateArray()
            .Should()
            .Contain(x => x.GetProperty("PropertyName").GetString() == "Year");
    }

    [Fact]
    public async Task GetExpensesWhenExpenseExistsReturnExpense()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var year = 2026;
        var expenseAmount = 100;
        var categoryName = "GetExpensesWhenExpenseExistsReturnExpense";

        var category = await client.CreateExpenseCategory(categoryName);
        await client.CreateExpenseAsync(category!.Id, year, 5, expenseAmount);

        // Act
        var response = await client.GetExpensesAsResponseMessageAsync(year, new GetExpensesInput());

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expensesTab = await response.Content.ReadFromJsonAsync<ExpensesTabPageCoreDto>(_jsonOptions, _cancellationToken);

        var categoryDto = expensesTab!.CategoriesExpenses.Items.Should()
            .ContainSingle(x => x.Category.Name == categoryName).Subject;

        categoryDto.ExpensesByMonth.Should().ContainSingle();
        categoryDto.ExpensesByMonth.Single()!.ExpenseAmount.Should().Be(expenseAmount);
    }

    [Fact]
    public async Task GetExpensesWhenFilterByNameReturnFilteredExpenses()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var year = 2026;
        var category1Name = "Office GetExpensesWhenFilterByNameReturnFilteredExpenses";
        var category2Name = "Marketing GetExpensesWhenFilterByNameReturnFilteredExpenses";

        var category1 = await client.CreateExpenseCategory(category1Name);
        var category2 = await client.CreateExpenseCategory(category2Name);
        await client.CreateExpenseAsync(category1!.Id, year, 5, 10);
        await client.CreateExpenseAsync(category2!.Id, year, 5, 20);

        // Act
        var response = await client.GetExpensesAsResponseMessageAsync(year, new GetExpensesInput{ CategoryName = "Market" });

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expensesTab = await response.Content.ReadFromJsonAsync<ExpensesTabPageCoreDto>(_jsonOptions, _cancellationToken);
        expensesTab!.CategoriesExpenses.Items.Should().ContainSingle();
        expensesTab.CategoriesExpenses.Items.Single()
            .Category.Name.Should().Be(category2Name);
    }

    [Fact]
    public async Task GetExpensesWhenPaginationReturnPaginatedExpenses()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var year = 2024;
        var categoryNames = new List<string>();
        for(var i = 1; i <= 5; i++)
        {
            categoryNames.Add($"category {i} GetExpensesWhenPaginationReturnPaginatedExpenses");
        }

        var categories = new List<ExpenseCategoryCoreDto?>();
        foreach (var name in categoryNames)
        {
            categories.Add(await client.CreateExpenseCategory(name));
        }

        var expenses = new List<ExpenseCoreDto?>();
        foreach (var category in categories)
        {
            expenses.Add(await client.CreateExpenseAsync(category!.Id, year, 5, 10));
        }

        var pageSize = 2;
        var pageNumber = 1;
        // Act
        var response = await client.GetExpensesAsResponseMessageAsync(year, new GetExpensesInput(),
            pageSize: pageSize, pageNumber: pageNumber);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var expensesTab = await response.Content.ReadFromJsonAsync<ExpensesTabPageCoreDto>(_jsonOptions, _cancellationToken);
        expensesTab!.CategoriesExpenses.Items.Should().HaveCount(2);
        expensesTab!.CategoriesExpenses.PageSize.Should().Be(pageSize);
        expensesTab!.CategoriesExpenses.Page.Should().Be(pageNumber);
        expensesTab!.CategoriesExpenses.Total.Should().BeGreaterThanOrEqualTo(categoryNames.Count());
    }
}
