using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class GetItemsLookupRequestTests
{
    private readonly Mock<IGetAvailableItemsDbQuery> _dbQuery = new();

    private readonly GetItemsLookupRequest _request;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetItemsLookupRequestTests()
    {
        _request = new GetItemsLookupRequest(_dbQuery.Object);
    }

    [Fact]
    public async Task HandleWhenNoItemsFoundShouldReturnEmptyList()
    {
        //Arrange
        _dbQuery.Setup(query => query.ExecuteAsync(_token))
            .ReturnsAsync([]);

        var expectedResult = new List<ItemLookupDto>();

        //Act
        var result = await _request.Handle(new GetItemsLookupRequest.Query(), _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }

    [Fact]
    public async Task HandleShouldReturnValidListOfItems()
    {
        //Arrange
        _dbQuery.Setup(query => query.ExecuteAsync(_token))
            .ReturnsAsync([
                new() { Id = 1, Name = "Hoodie", IsClothing = true, Status = MerchItemStatus.Available },
                new() { Id = 2, Name = "Cup", IsClothing = false, Status = MerchItemStatus.Available },
                new() { Id = 4, Name = "Pen", IsClothing = false, Status = MerchItemStatus.Available },
                new() { Id = 6, Name = "Gingerbread", IsClothing = false, Status = MerchItemStatus.Available },
                new() { Id = 7, Name = "Notes book", IsClothing = false, Status = MerchItemStatus.Available }
            ]);

        var expectedResult = new List<ItemLookupDto>
        {
            new(1, "Hoodie", true),
            new(2, "Cup", false),
            new(4, "Pen", false),
            new(6, "Gingerbread", false),
            new(7, "Notes book", false),
        };

        //Act
        var result = await _request.Handle(new GetItemsLookupRequest.Query(), _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }

}
