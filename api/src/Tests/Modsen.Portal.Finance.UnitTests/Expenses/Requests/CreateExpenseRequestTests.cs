using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;
using Modsen.Portal.Finance.Requests.Expenses.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Expenses.Requests;

public class CreateExpenseRequestTests
{
    private readonly Mock<IExpenseEncryptor> _expenseEncryptorMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateExpenseRequest _request;

    public CreateExpenseRequestTests()
    {
        _request = new CreateExpenseRequest(
            _expenseEncryptorMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenExpenseCategoryNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(GetCommand(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task HandleWhenExpenseCategoryIsDeletedThrowEntityNotFoundException()
    {
        // Arrange
        var existingCategory = TestDataGenerator.GetExpenseCategoryEntity(isDeleted: true);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { existingCategory }.BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(GetCommand(expenseCategoryId: existingCategory.Id), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task HandleWhenExpenseAlredyexistsThrowInvalidDataAppException()
    {
        // Arrange
        var existingCategory = TestDataGenerator.GetExpenseCategoryEntity();
        var command = GetCommand(expenseCategoryId: existingCategory.Id);
        var existingExpense = TestDataGenerator.GetExpenseEntity(categoryId: existingCategory.Id,
            year: command.Year, monthOfYear: command.MonthOfYear);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { existingCategory }.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.Expenses)
            .Returns(new List<ExpenseEntity> { existingExpense }.BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.ExpenseAlreadyExists);
    }

    [Fact]
    public async Task HandleWhenRequestValidReturnsDto()
    {
        // Arrange
        var existingCategory = TestDataGenerator.GetExpenseCategoryEntity(id: 12);

        var command = GetCommand(expenseCategoryId: existingCategory.Id);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { existingCategory }.BuildMockDbSet().Object);

        var expensesMockDbSet = new List<ExpenseEntity>().BuildMockDbSet();

        _dbContextMock.SetupGet(x => x.Expenses).Returns(expensesMockDbSet.Object);

        // Act
        var result = await _request.Handle(command, _cancellationToken); 

        // Assert
        result.Should().NotBeNull();
        result.ExpenseCategoryId.Should().Be(existingCategory.Id);
        result.Year.Should().Be(command.Year);
        result.MonthOfYear.Should().Be(command.MonthOfYear);
        result.ExpenseAmount.Should().Be(command.ExpenseAmount);

        _expenseEncryptorMock.Verify(x => x.Encrypt(It.Is<decimal>(n => n == command.ExpenseAmount)), Times.Once);
        expensesMockDbSet.Verify(
            x => x.AddAsync(It.Is<ExpenseEntity>(e => e.ExpenseCategoryId == existingCategory.Id && e.Year == command.Year
                && e.MonthOfYear == command.MonthOfYear), It.IsAny<CancellationToken>()),
            Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateExpenseRequest.Command GetCommand(int expenseCategoryId = 1)
    {
        return new CreateExpenseRequest.Command
        {
            ExpenseCategoryId = expenseCategoryId,
            Year = 2026,
            MonthOfYear = 6,
            ExpenseAmount = 10000
        };
    }
}
