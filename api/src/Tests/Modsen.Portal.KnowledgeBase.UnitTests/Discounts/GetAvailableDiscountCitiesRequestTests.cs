using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class GetAvailableDiscountCitiesRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();
    private readonly GetAvailableDiscountCitiesRequest _handler;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GetAvailableDiscountCitiesRequestTests()
    {
        _handler = new GetAvailableDiscountCitiesRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenCitiesExistShouldReturnCitiesList()
    {
        // Arrange
        var cities = new List<CityEntity> { new() { Id = 10, Name = "Minsk" }, new() { Id = 20, Name = "Brest" } };

        MockDbContext(cities);

        var query = new GetAvailableDiscountCitiesRequest.Query();

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(new DiscountCityCoreDto { CityId = 10, Name = "Minsk" });
        result.Should().ContainEquivalentOf(new DiscountCityCoreDto { CityId = 20, Name = "Brest" });
    }

    [Fact]
    public async Task HandleWhenNoCitiesExistShouldReturnEmptyList()
    {
        // Arrange
        MockDbContext([]);

        var query = new GetAvailableDiscountCitiesRequest.Query();

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        result.Should().BeEmpty();
    }

    private void MockDbContext(IEnumerable<CityEntity> cities)
    {
        _dbContextMock.Setup(x => x.Cities).Returns(cities.ToArray().BuildMockDbSet().Object);
    }
}
