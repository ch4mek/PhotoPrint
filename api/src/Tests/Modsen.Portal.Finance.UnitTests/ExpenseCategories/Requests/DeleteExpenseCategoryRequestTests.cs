using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExpenseCategories.Requests;

public class DeleteExpenseCategoryRequestTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly DeleteExpenseCategoryRequest _request;

    public DeleteExpenseCategoryRequestTests()
    {
        _request = new DeleteExpenseCategoryRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenExpenceCategoryNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var command = new DeleteExpenseCategoryRequest.Command(CategoryId: 1);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ExpenseCategoryNotFound);
    }

    [Fact]
    public async Task HandleWhenNoCorrespondingExpensesDeletesCategory()
    {
        // Arrange
        var deleting = TestDataGenerator.GetExpenseCategoryEntity();

        var command = new DeleteExpenseCategoryRequest.Command(CategoryId: deleting.Id);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { deleting}.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(deleting.Name);
        result.OrderWeight.Should().Be(deleting.OrderWeight);
        result.IsDeleted.Should().Be(true);

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleWhenAreCorrespondingExpensesDeletesCategoryAndExpenses()
    {
        // Arrange
        var categoryId = 1;
        var expense = TestDataGenerator.GetExpenseEntity(categoryId);
        var deleting = TestDataGenerator.GetExpenseCategoryEntity(id: categoryId, expenses: new List<ExpenseEntity> { expense});

        var command = new DeleteExpenseCategoryRequest.Command(CategoryId: deleting.Id);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { deleting }.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(deleting.Name);
        result.OrderWeight.Should().Be(deleting.OrderWeight);
        result.IsDeleted.Should().BeTrue();

        deleting.IsDeleted.Should().BeTrue();
        deleting.Expenses.All(x => x.IsDeleted).Should().BeTrue();

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
