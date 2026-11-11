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

public class UpdateExpenseRequestTests
{
    private readonly Mock<IExpenseEncryptor> _expenseEncryptorMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateExpenseRequest _request;

    public UpdateExpenseRequestTests()
    {
        _request = new UpdateExpenseRequest(
            _expenseEncryptorMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenExpenseNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        _dbContextMock.SetupGet(x => x.Expenses)
            .Returns(new List<ExpenseEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(new UpdateExpenseRequest.Command(100, 50000), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ExpenseNotFound);
    }

    [Fact]
    public async Task HandleWhenExpenseIsDeletedThrowEntityNotFoundException()
    {
        // Arrange
        var existing = TestDataGenerator.GetExpenseEntity(isDeleted: true);

        _dbContextMock.SetupGet(x => x.Expenses)
            .Returns(new List<ExpenseEntity> { existing }.BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(new UpdateExpenseRequest.Command(existing.Id, 50000), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ExpenseNotFound);
    }

    [Fact]
    public async Task HandleWhenRequestValidReturnsDto()
    {
        // Arrange
        var existingCategory = TestDataGenerator.GetExpenseCategoryEntity();
        var existing = TestDataGenerator.GetExpenseEntity(expenseCategory: existingCategory);

        var command = new UpdateExpenseRequest.Command(existing.Id, 50000);

        _dbContextMock.SetupGet(x => x.Expenses)
            .Returns(new List<ExpenseEntity> { existing }.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existing.Id);
        result.ExpenseCategoryId.Should().Be(existing.ExpenseCategoryId);
        result.Year.Should().Be(existing.Year);
        result.MonthOfYear.Should().Be(existing.MonthOfYear);
        result.ExpenseAmount.Should().Be(command.ExpenseAmount);

        _expenseEncryptorMock.Verify(x => x.Encrypt(It.Is<decimal>(n => n == command.ExpenseAmount)), Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
