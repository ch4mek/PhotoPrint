using MediatR;
using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class UpdateDiscountOrderRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContext = new();
    private readonly UpdateDiscountOrderRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateDiscountOrderRequestTests()
    {
        _handler = new UpdateDiscountOrderRequest(_dbContext.Object);
    }

    [Fact]
    public async Task HandleWhenUpdatesOrderWeightShouldBeSuccess()
    {
        // Arrange
        var discounts = new List<DiscountEntity>
        {
            new()
            {
                Id = 1,
                OrderWeight = 1,
                Name = null,
                Text = null
            },
            new()
            {
                Id = 2,
                OrderWeight = 5,
                Name = null,
                Text = null
            }
        };

        var updateOrders = new List<DiscountOrderDto>
        {
            new() { DiscountId = 1, OrderWeight = 10 },
            new() { DiscountId = 2, OrderWeight = 20 }
        };

        MockDbContext(discounts);

        var command = new UpdateDiscountOrderRequest.Command
        {
            Orders = updateOrders
        };

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        discounts.First(x => x.Id == 1).OrderWeight.Should().Be(10);
        discounts.First(x => x.Id == 2).OrderWeight.Should().Be(20);
        _dbContext.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleWhenNoMatchingDiscountsShouldDoNothingButSaveChangesCalled()
    {
        // Arrange
        var discounts = new List<DiscountEntity>
        {
            new()
            {
                Id = 99,
                OrderWeight = 3,
                Name = null,
                Text = null
            }
        };

        var updateOrders = new List<DiscountOrderDto>
        {
            new() { DiscountId = 1, OrderWeight = 10 }
        };

        MockDbContext(discounts);

        var command = new UpdateDiscountOrderRequest.Command
        {
            Orders = updateOrders
        };

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        discounts.First().OrderWeight.Should().Be(3);
        _dbContext.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }

    private void MockDbContext(IEnumerable<DiscountEntity> discounts)
    {
        _dbContext.Setup(x => x.Discounts)
            .Returns(discounts.ToArray().BuildMockDbSet().Object);
    }
}
