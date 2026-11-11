using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExpenseCategories.Requests;

public class CreateExpenseCategoryRequestTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateExpenseCategoryRequest _request;

    public CreateExpenseCategoryRequestTests()
    {
        _request = new CreateExpenseCategoryRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenCategoryNameUsedThrowInvalidDataAppException()
    {
        // Arrange
        var existing = TestDataGenerator.GetExpenseCategoryEntity(); 

        var command = new CreateExpenseCategoryRequest.Command(existing.Name);

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(new List<ExpenseCategoryEntity> { existing }.BuildMockDbSet().Object);

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
        var existing = TestDataGenerator.GetExpenseCategoryEntity();

        var command = new CreateExpenseCategoryRequest.Command("second category");

        var dbSetMock = new List<ExpenseCategoryEntity> { existing }
            .BuildMockDbSet();

        _dbContextMock.SetupGet(x => x.ExpenseCategories)
            .Returns(dbSetMock.Object);

        // Act
        var result = await _request.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.CategoryName);
        result.OrderWeight.Should().Be(existing.OrderWeight + 1);
        result.IsDeleted.Should().Be(false);

        dbSetMock.Verify(
            x => x.AddAsync(
                It.Is<ExpenseCategoryEntity>(e =>
                    e.Name == command.CategoryName &&
                    e.OrderWeight == existing.OrderWeight + 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
