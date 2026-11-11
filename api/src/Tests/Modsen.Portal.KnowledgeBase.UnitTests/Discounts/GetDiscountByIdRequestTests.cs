using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class GetDiscountByIdRequestTests
{
    private readonly Mock<IDiscountLoader> _discountLoader = new();
    private readonly GetDiscountByIdRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetDiscountByIdRequestTests()
    {
        _handler = new GetDiscountByIdRequest(_discountLoader.Object);
    }

    [Fact]
    public async Task HandleWithExistingIdShouldBeSuccess()
    {
        // Arrange
        const int discountId = 1;
        var query = new GetDiscountByIdRequest.Query(discountId);

        var discount = new DiscountCoreDto
        {
            Id = discountId,
            Name = "Test Discount",
            DiscountCategoryName = "Category",
            DiscountPercent = 10,
            Text = "Text",
            DiscountTags = [],
            DiscountCities = [],
            DiscountReferences = []
        };

        _discountLoader.Setup(x => x.Load(discountId, _cancellationToken))
            .ReturnsAsync(discount);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(discountId);
    }

    [Fact]
    public async Task HandleWithNonExistingIdShouldBeFailed()
    {
        // Arrange
        const int discountId = 1;
        var query = new GetDiscountByIdRequest.Query(discountId);

        _discountLoader.Setup(x => x.Load(discountId, _cancellationToken))
            .ReturnsAsync((DiscountCoreDto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _handler.Handle(query, _cancellationToken));
    }
}
