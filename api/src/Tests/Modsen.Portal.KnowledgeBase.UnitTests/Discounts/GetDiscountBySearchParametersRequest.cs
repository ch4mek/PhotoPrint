using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class GetDiscountBySearchParametersRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();

    private readonly GetDiscountBySearchParametersRequest _handler;

    public GetDiscountBySearchParametersRequestTests()
    {
        _handler = new GetDiscountBySearchParametersRequest(
            _dbContextMock.Object,
            new DiscountLoader(_dbContextMock.Object));
    }

    private static void AssertDiscounts(int[] ids, List<DiscountCoreDto> discountDtos)
    {
        var discounts = discountDtos.Select(b => b.Id).Order().ToArray();
        discounts.Should().BeEquivalentTo(ids.Order());
    }

    [Theory]
    [InlineData(new[] { 2, 3 }, 2, null, null, null, null)]
    [InlineData(new[] { 1, 2 }, null, 1, null, null, null)]
    [InlineData(new[] { 1 }, null, 2, null, null, null)]
    [InlineData(new[] { 2 }, null, null, null, null, 1)]
    [InlineData(new[] { 1, 2, 3, 4 }, null, null, null, null, null)]
    [InlineData(new[] { 1, 2, 3, 4 }, null, null, "Name", null, null)]
    public async Task HandleWhenSearchShouldReturnIds(int[]? ids, int? group, int? tag, string? name,
        string? keywords, int? city)
    {
        // Arrange
        Shared.MockDbContext(_dbContextMock);

        // Act
        var discounts = await _handler.Handle(
            new GetDiscountBySearchParametersRequest.Query(group, tag, name, city),
            It.IsAny<CancellationToken>());

        // Assert
        AssertDiscounts(ids ?? [], discounts);
    }
}
