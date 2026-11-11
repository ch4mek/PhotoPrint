using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class DeleteDiscountRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();
    private readonly Mock<ICommunicationBus> _iCommunicationBusMock = new();

    private readonly DeleteDiscountRequest _handler;

    public DeleteDiscountRequestTests()
    {
        _handler = new DeleteDiscountRequest(_dbContextMock.Object, _iCommunicationBusMock.Object);
    }

    private void MockTestContext()
    {
        var discounts = new List<DiscountEntity> { new() { Id = 1, Name = "Discount", Text = "test" } };
        _dbContextMock.Setup(db => db.Discounts)
            .Returns(discounts.BuildMockDbSet().Object);
        _dbContextMock.Setup(db => db.Discounts.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((object[] discount) =>
                new ValueTask<DiscountEntity?>(discounts.Find(it => it.Id == (int)discount[0])));
    }

    [Fact]
    public async Task HandleWhenRequestShouldCreate()
    {
        // Arrange
        MockTestContext();

        // Act
        var id = await _handler.Handle(new DeleteDiscountRequest.Command(1), It.IsAny<CancellationToken>());

        // Assert
        id.Should().Be(1);

        _dbContextMock.Verify(db => db.Discounts.Remove(It.IsAny<DiscountEntity>()), Times.Once);
    }

    [Fact]
    public async Task HandleWhenRequest404ShouldNotFound()
    {
        // Arrange
        MockTestContext();

        // Act
        var test = () => _handler.Handle(new DeleteDiscountRequest.Command(2), It.IsAny<CancellationToken>());

        // Assert
        await test.Should().ThrowAsync<EntityNotFoundException>();

        _dbContextMock.Verify(db => db.Discounts.Remove(It.IsAny<DiscountEntity>()), Times.Never);
    }
}
