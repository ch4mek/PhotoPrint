using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExpenseCategories.Requests;

public class EditExpenseCategoryRequestTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly EditExpenseCategoryRequest _request;

    public EditExpenseCategoryRequestTests()
    {
        _request = new EditExpenseCategoryRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenExpenceCategoryNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        var command = new EditExpenseCategoryRequest.Command(CategoryId: 11, "new category name");

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
    public async Task HandleWhenCategoryNameUsedThrowInvalidDataAppException()
    {
        // Arrange
        var existing1 = TestDataGenerator.GetExpenseCategoryEntity(id: 1, name: "category 1");
        var updating = TestDataGenerator.GetExpenseCategoryEntity(id: 2, name: "category 2");

        var command = new EditExpenseCategoryRequest.Command(CategoryId: updating.Id, "category 1");

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { existing1, updating }.BuildMockDbSet().Object);

        // Act
        var action = () => _request.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.ExpenseCategoryExists);
    }

    [Fact]
    public async Task HandleWhenCategoryNameUniqueReturnsDto()
    {
        // Arrange
        var existing1 = TestDataGenerator.GetExpenseCategoryEntity(id: 1, name: "category 1");
        var updating = TestDataGenerator.GetExpenseCategoryEntity(id: 2, name: "category 2");

        var command = new EditExpenseCategoryRequest.Command(CategoryId: updating.Id, "category 2 updated");

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { existing1, updating }.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.CategoryName);
        result.OrderWeight.Should().Be(updating.OrderWeight);

        updating.Name.Should().Be(command.CategoryName);

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
