using MockQueryable.Moq;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.DbQueries;

public class GetAvailableItemsDbQueryTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();

    private readonly GetAvailableItemsDbQuery _dbQuery;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetAvailableItemsDbQueryTests()
    {
        _dbQuery = new GetAvailableItemsDbQuery(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task ExecuteAsyncShouldReturnAvailableItems()
    {
        //Arrange
        _merchDbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new() { Id = 1, Name = "Hoodie", IsClothing = true, Status = MerchItemStatus.Available},
                new() { Id = 2, Name = "Cup", IsClothing = false, Status = MerchItemStatus.Available},
                new() { Id = 3, Name = "T-shirt", IsClothing = true, Status = MerchItemStatus.Archived},
                new() { Id = 4, Name = "Pen", IsClothing = false, Status = MerchItemStatus.Available},
                new() { Id = 5, Name = "Postcard", IsClothing = false, Status = MerchItemStatus.Archived},
                new() { Id = 6, Name = "Gingerbread", IsClothing = false, Status = MerchItemStatus.Available},
                new() { Id = 7, Name = "Notes book", IsClothing = false, Status = MerchItemStatus.Available},
            }.BuildMockDbSet().Object);

        //Act
        var items = await _dbQuery.ExecuteAsync(_token);

        //Assert
        items.Count.Should().Be(5);
        items.Select(item => item.Id).Should().BeEquivalentTo([1, 2, 4, 6, 7]);

    }
}
