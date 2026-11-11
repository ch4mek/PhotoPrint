using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Finance.ExpenseCategories;

[Collection("GeneralCollection")]
public class ExpenseCategoriesIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ExpenseCategoriesIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateExpenseCategoryWhenCannotManageExpensesResponseWith403()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.CreateExpenseCategoryAsResponseMessageAsync(new CreateExpenseCategoryInput(""));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateExpenseCategoryWhenNameOnlyPunctuationMarksResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateExpenseCategoryInput(",/.;-| ");

        // Act
        var response = await client.CreateExpenseCategoryAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateExpenseCategoryWhenRequestOkResponseWithExpenseCategoryDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateExpenseCategoryInput("CreateExpenseCategoryWhenRequestOkResponseWithExpenseCategoryDto test category");

        // Act
        var response = await client.CreateExpenseCategoryAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var category = await response.Content.ReadFromJsonAsync<ExpenseCategoryCoreDto>(_jsonOptions, _cancellationToken);
        category.Should().NotBeNull();
        category.Name.Should().Be(input.CategoryName);
        category.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateExpenseCategoryNameExistsResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateExpenseCategoryInput("CreateExpenseCategoryNameExistsResponseWith400 test category");
        await client.CreateExpenseCategoryAsResponseMessageAsync(input);

        // Act
        var response = await client.CreateExpenseCategoryAsResponseMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseCategoryExists);
    }


    [Fact]
    public async Task EditExpenseCategoryWhenNameOnlyPunctuationMarksResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new EditExpenseCategoryInput(",/.;-| ");

        // Act
        var response = await client.EditExpenseCategoryAsResponseMessageAsync(100, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditExpenseCategoryWhenNotFoundResponseWith404()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new EditExpenseCategoryInput("EditExpenseCategoryWhenNotFoundResponseWith404");
        // Act
        var response = await client.EditExpenseCategoryAsResponseMessageAsync(100, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task EditExpenseCategoryNameExistsResponseWith400()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var existingInput1 = new CreateExpenseCategoryInput("EditExpenseCategoryNameExistsResponseWith400 category name");
        await client.CreateExpenseCategoryAsResponseMessageAsync(existingInput1);

        var updatingCategory = await client.CreateExpenseCategory("EditExpenseCategoryNameExistsResponseWith400 updated category name");

        var input = new EditExpenseCategoryInput(existingInput1.CategoryName);
        // Act
        var response = await client.EditExpenseCategoryAsResponseMessageAsync(updatingCategory!.Id, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseCategoryExists);
    }

    [Fact]
    public async Task EditExpenseCategoryWhenRequestValidResponseWithDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var updatingCategory = await client.CreateExpenseCategory("EditExpenseCategoryWhenRequestValidResponseWithDto category name");

        var input = new EditExpenseCategoryInput("EditExpenseCategoryWhenRequestValidResponseWithDto category updated");

        // Act
        var response = await client.EditExpenseCategoryAsResponseMessageAsync(updatingCategory!.Id, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCategory = await response.Content.ReadFromJsonAsync<ExpenseCategoryCoreDto>(_jsonOptions, _cancellationToken);

        updatedCategory.Should().NotBeNull();
        updatedCategory.Name.Should().Be(input.CategoryName);
        updatedCategory.OrderWeight.Should().Be(updatingCategory.OrderWeight);
        updatedCategory.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteExpenseCategoryWhenNotFoundResponseWith404()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.DeleteExpenseCategoryAsResponseMessageAsync(100);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task DeleteExpenseCategoryWhenRequestValidResponseWithDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var deletingCategory = await client.CreateExpenseCategory("DeleteExpenseCategoryWhenRequestValidResponseWithDto category name");

        // Act
        var response = await client.DeleteExpenseCategoryAsResponseMessageAsync(deletingCategory!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var deletedCategory = await response.Content.ReadFromJsonAsync<ExpenseCategoryCoreDto>(_jsonOptions, _cancellationToken);

        deletedCategory.Should().NotBeNull();
        deletedCategory.Name.Should().Be(deletingCategory.Name);
        deletedCategory.OrderWeight.Should().Be(deletingCategory.OrderWeight);
        deletedCategory.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetExpenseCategoryLookupWhenRequestValidResponseWithNotDeletedCategories()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var activeCategory = await client.CreateExpenseCategory("active ExpenseCategoryLookup category name");
        var deletedCategory = await client.CreateExpenseCategory("deleted ExpenseCategoryLookup category name");
        await client.DeleteExpenseCategoryAsResponseMessageAsync(deletedCategory!.Id);

        // Act
        var response = await client.ExpenseCategoryLookupAsResponseMessageAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var lookup = await response.Content.ReadFromJsonAsync<List<string>>(_jsonOptions, _cancellationToken);

        lookup.Should().Contain(activeCategory!.Name);
        lookup.Should().NotContain(deletedCategory!.Name);
    }
}
