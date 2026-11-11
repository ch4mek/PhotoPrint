using MockQueryable.Moq;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class GetMerchSizesRequestTests
{
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetMerchSizesRequest _handler;

    public GetMerchSizesRequestTests()
    {
        _merchDbContextMock.Setup(x => x.MerchSizes)
            .Returns(new List<MerchSizeEntity>
            {
                new() { Id = "XS", Name = "Extra small", OrderWeight = 100 },
                new() { Id = "S", Name = "Small", OrderWeight = 200 },
                new() { Id = "M", Name = "Medium", OrderWeight = 300 },
                new() { Id = "L", Name = "Large", OrderWeight = 400 },
                new() { Id = "XL", Name = "Extra large", OrderWeight = 500 },
                new() { Id = "2XL", Name = "Double extra large", OrderWeight = 600 },
                new() { Id = "3XL", Name = "Triple extra large", OrderWeight = 700 },
                new() { Id = "4XL", Name = "Quadruple extra large", OrderWeight = 800 },
                new() { Id = "One", Name = "One size", OrderWeight = 0 }
            }.BuildMockDbSet().Object);

        _handler = new GetMerchSizesRequest(_merchDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenSizesExistReturnsValidMerchSizeDtosArray()
    {
        //Arrange
        var expected = new MerchSizeDto[]
        {
            new() { Id = "XS", Name = "Extra small" },
            new() { Id = "S", Name = "Small" },
            new() { Id = "M", Name = "Medium" },
            new() { Id = "L", Name = "Large" },
            new() { Id = "XL", Name = "Extra large" },
            new() { Id = "2XL", Name = "Double extra large" },
            new() { Id = "3XL", Name = "Triple extra large" },
            new() { Id = "4XL", Name = "Quadruple extra large" },
            new() { Id = "One", Name = "One size" }
        };

        //Act
        var result = await _handler.Handle(new GetMerchSizesRequest.Query(), _token);

        //Assert
        result.Should().BeEquivalentTo(expected);

    }
}
