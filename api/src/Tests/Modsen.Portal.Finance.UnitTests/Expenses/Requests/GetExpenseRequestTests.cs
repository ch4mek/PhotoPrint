using AwesomeAssertions;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Expenses.Core.Builders;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;
using Modsen.Portal.Finance.Requests.Expenses.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Expenses.Requests;

public class GetExpenseRequestTests
{
    private readonly Mock<IExpenseEncryptor> _expenseEncryptorMock = new();
    private readonly Mock<IExpensesQueryBuilder> _expensesQueryBuilderMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetExpenseRequest _request;

    public GetExpenseRequestTests()
    {
        _request = new GetExpenseRequest(
            _expenseEncryptorMock.Object,
            _expensesQueryBuilderMock.Object);

        SetupBuilder();
    }

    private void SetupBuilder()
    {
        _expensesQueryBuilderMock
            .Setup(x => x.ForYear(It.IsAny<int>()))
            .Returns(_expensesQueryBuilderMock.Object);

        _expensesQueryBuilderMock
            .Setup(x => x.WithPagination(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(_expensesQueryBuilderMock.Object);

        _expensesQueryBuilderMock
            .Setup(x => x.WithFilter(It.IsAny<ExpenseFilterCoreDto>()))
            .Returns(_expensesQueryBuilderMock.Object);

        _expensesQueryBuilderMock
            .Setup(x => x.WithOrdering(It.IsAny<ExpenseOrderingCoreDto>()))
            .Returns(_expensesQueryBuilderMock.Object);
    }

    [Fact]
    public async Task HandleWhenNoExpensesReturnsEmptyExpensesList()
    {
        // Arrange
        var categorizedExpenses = new ExpensesQueryBuilderDto
        {
            Categories = [TestDataGenerator.GetExpenseCategoryEntity()],
            TotalCount = 1
        };

        _expensesQueryBuilderMock.Setup(x => x.BuildAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorizedExpenses);       

        // Act
        var result = await _request.Handle(GetQuery(), _cancellationToken);

        // Assert
        result.CategoriesExpenses.Items.Should().HaveCount(1);

        var item = result.CategoriesExpenses.Items.Single();
        item.Category.Id.Should().Be(1);
        item.ExpensesByMonth.Should().BeEmpty();
        result.ExpensesSumsByMonth.Should().OnlyContain(x => x == 0);

        _expenseEncryptorMock.Verify(x => x.Decrypt(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleWhenCategoryHasExpensesReturnsExpenses()
    {
        // Arrange
        var categoryId = 3;
        var expenses = new List<ExpenseEntity>
        {
            TestDataGenerator.GetExpenseEntity(categoryId, 1, "enc1", monthOfYear: 5),
            TestDataGenerator.GetExpenseEntity(categoryId, 2, "enc2", monthOfYear: 7),
        };
        var category = TestDataGenerator.GetExpenseCategoryEntity(id: categoryId, expenses: expenses);

        var categorizedExpenses = new ExpensesQueryBuilderDto
        {
            Categories = [category],
            TotalCount = 1
        };

        _expensesQueryBuilderMock.Setup(x => x.BuildAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorizedExpenses);

        _expenseEncryptorMock.Setup(x => x.Decrypt(expenses[0].ExpenseAmount)).Returns(100);
        _expenseEncryptorMock.Setup(x => x.Decrypt(expenses[1].ExpenseAmount)).Returns(200);

        // Act
        var result = await _request.Handle(GetQuery(), _cancellationToken);

        // Assert
        var resultExpenses = result.CategoriesExpenses.Items.Single().ExpensesByMonth;

        resultExpenses.Should().HaveCount(2);

        resultExpenses[0]!.MonthOfYear.Should().Be(expenses[0].MonthOfYear);
        resultExpenses[0]!.ExpenseAmount.Should().Be(100);

        resultExpenses[1]!.MonthOfYear.Should().Be(expenses[1].MonthOfYear);
        resultExpenses[1]!.ExpenseAmount.Should().Be(200);

        _expenseEncryptorMock.Verify(x => x.Decrypt(expenses[0].ExpenseAmount), Times.Once);
        _expenseEncryptorMock.Verify(x => x.Decrypt(expenses[1].ExpenseAmount), Times.Once);
    }

    [Fact]
    public async Task HandleWhenSeveralExpensesInSameMont_SumsThemCorrectly()
    {
        var category1Id = 3;
        var category2Id = 4;
        var expenses = new List<ExpenseEntity>
        {
            TestDataGenerator.GetExpenseEntity(category1Id, 1, "enc1", monthOfYear: 1),
            TestDataGenerator.GetExpenseEntity(category2Id, 2, "enc2", monthOfYear: 1),
            TestDataGenerator.GetExpenseEntity(category1Id, 3, "enc3", monthOfYear: 2),
        };
        var category1 = TestDataGenerator.GetExpenseCategoryEntity(id: category1Id,
            expenses: expenses.Where(e => e.ExpenseCategoryId == category1Id).ToList());
        var category2 = TestDataGenerator.GetExpenseCategoryEntity(id: category2Id,
            expenses: expenses.Where(e => e.ExpenseCategoryId == category2Id).ToList());

        var categorizedExpenses = new ExpensesQueryBuilderDto
        {
            Categories = [category1, category2],
            TotalCount = 2
        };

        _expensesQueryBuilderMock.Setup(x => x.BuildAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorizedExpenses);

        var decrypted1 = 100;
        var decrypted2 = 50;
        var decrypted3 = 30;

        _expenseEncryptorMock.Setup(x => x.Decrypt(expenses[0].ExpenseAmount)).Returns(decrypted1);
        _expenseEncryptorMock.Setup(x => x.Decrypt(expenses[1].ExpenseAmount)).Returns(decrypted2);
        _expenseEncryptorMock.Setup(x => x.Decrypt(expenses[2].ExpenseAmount)).Returns(decrypted3);

        // Act
        var result = await _request.Handle(GetQuery(), _cancellationToken);

        // Assert
        result.ExpensesSumsByMonth[0].Should().Be(decrypted1 + decrypted2);
        result.ExpensesSumsByMonth[1].Should().Be(30);

        for (var i = 2; i < 12; i++)
        {
            result.ExpensesSumsByMonth[i].Should().Be(0);
        }
    }

    private GetExpenseRequest.Query GetQuery()
    {
        return new GetExpenseRequest.Query
        {
            Year = 2026,
            PageNumber = 0,
            PageSize = 25
        };
    }
}
