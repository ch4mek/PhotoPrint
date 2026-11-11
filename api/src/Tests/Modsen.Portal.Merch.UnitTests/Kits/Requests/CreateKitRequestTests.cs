using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Validators;
using Modsen.Portal.Merch.Requests.Kits.Requests;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests;

public class CreateKitRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ICreateOrUpdateKitValidator> _createKitValidatorMock = new();
    private readonly Mock<IItemLookupLoader> _itemLookupLoaderMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly CreateKitRequest _handler;

    public CreateKitRequestTests()
    {
        _handler = new CreateKitRequest(
            _authenticatedUserContextMock.Object,
            _createKitValidatorMock.Object,
            _itemLookupLoaderMock.Object,
            _clockServiceMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestValidCreatesKitAndReturnsKitDto()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(1);

        _clockServiceMock
            .Setup(service => service.UtcNow)
            .Returns(DateTime.UtcNow);

        _createKitValidatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<CreateOrUpdateKitValidatorCoreDto>(), _token));

        _itemLookupLoaderMock
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new(1, "Cup", false),
                new(2, "Pen", false)
            ]);

        _dbContextMock.Setup(context => context.Kits.AddAsync(It.IsAny<KitEntity>(), _token));
        _dbContextMock.Setup(context => context.SaveChangesAsync( _token));

        var requestDto = new CreateKitDto(
            Name: "Kit 1",
            Description: "Kit 1 description",
            PhotoFileId: "fileId",
            Compounds: new List<AddKitCompoundDto> { new(1, 2), new(2, 1) },
            MinAmounts: new List<KitMinAmountDto> { new(1, 2) });

        var expectedCompounds = new List<KitCompoundDto>
        {
            new(1, false, "Cup", 2),
            new(2, false, "Pen", 1),
        };

        var expectedMinAmounts = new List<KitMinAmountDto> { new(1, 2) };

        //Act
        var result = await _handler.Handle(new CreateKitRequest.Command(requestDto), _token);

        //Assert
        result.Name.Should().Be(requestDto.Name);
        result.Description.Should().Be(requestDto.Description);
        result.PhotoFileId.Should().Be(requestDto.PhotoFileId);

        result.Compounds.Count.Should().Be(2);
        result.Compounds.Should().BeEquivalentTo(expectedCompounds);

        result.MinAmounts.Count.Should().Be(1);
        result.MinAmounts.Should().BeEquivalentTo(expectedMinAmounts);

    }
}
