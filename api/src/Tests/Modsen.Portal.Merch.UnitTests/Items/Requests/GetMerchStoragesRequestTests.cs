using MockQueryable.Moq;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class GetMerchStoragesRequestTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetMerchStoragesRequest _handler;

    public GetMerchStoragesRequestTests()
    {
        _merchDbContextMock.Setup(x => x.Warehouses)
            .Returns(
                new List<WarehouseEntity>
                    {
                        new() { Id = 1, Location = "Minsk" },
                        new() { Id = 2, Location = "Vitebsk" },
                        new() { Id = 3, Location = "Warsaw" }
                    }.BuildMockDbSet()
                    .Object);

        _handler = new GetMerchStoragesRequest(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenStoragesExistReturnsValidMerchStorageDtosArray()
    {
        //Arrange
        var expected = new MerchStorageDto[]
        {
            new() { Id = 1, Name = "Minsk" },
            new() { Id = 2, Name = "Vitebsk" },
            new() { Id = 3, Name = "Warsaw" }
        };

        //Act
        var result = await _handler.Handle(new GetMerchStoragesRequest.Query(), _token);

        //Assert
        result.Should().BeEquivalentTo(expected);

    }
}
