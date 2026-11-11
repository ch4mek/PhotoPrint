using MockQueryable.Moq;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.Loaders;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Core.Loaders;

public class ItemLookupLoaderTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();

    private readonly ItemLookupLoader _loader;
    private readonly CancellationToken _token = CancellationToken.None;

    public ItemLookupLoaderTests()
    {
        _merchDbContextMock
            .Setup(context => context.Items)
            .Returns(new List<ItemEntity>
            {
                new() { Id = 1, Name = "Hoodie", IsClothing = true},
                new() { Id = 2, Name = "Cup", IsClothing = false},
                new() { Id = 3, Name = "T-shirt", IsClothing = true},
                new() { Id = 4, Name = "Pen", IsClothing = false},
                new() { Id = 5, Name = "Postcard", IsClothing = false},
                new() { Id = 6, Name = "Gingerbread", IsClothing = false},
                new() { Id = 7, Name = "Notes book", IsClothing = false},
            }.BuildMockDbSet().Object);

        _loader = new ItemLookupLoader(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task LoadAsyncWhenIdsNotSpecifiedReturnsEmptyCollection()
    {
        //Arrange
        var itemsIds = Array.Empty<int>();

        //Act
        var result = await _loader.LoadAsync(itemsIds, _token);

        //Assert
        result.Should().BeEmpty();

    }

    [Fact]
    public async Task LoadAsyncWhenIdOfOneItemSpecifiedReturnsCollectionWithOneItem()
    {
        //Arrange
        var itemsIds = new[] { 1 };

        //Act
        var result = await _loader.LoadAsync(itemsIds, _token);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.IsClothing.Should().BeTrue();
        result.FirstOrDefault()!.Id.Should().Be(1);
        result.FirstOrDefault()!.Name.Should().Be("Hoodie");

    }

    [Fact]
    public async Task LoadAsyncWhenIdsOfAllItemsSpecifiedReturnsCollectionWithAllItems()
    {
        //Arrange
        var itemsIds = new[] { 1, 2, 3, 4, 5, 6, 7, 8};

        //Act
        var result = await _loader.LoadAsync(itemsIds, _token);

        //Assert
        result.Count.Should().Be(7);

    }
}
