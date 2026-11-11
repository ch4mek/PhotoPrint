using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
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

public class UpdateKitRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ICreateOrUpdateKitValidator> _updateKitValidatorMock = new();
    private readonly Mock<IItemLookupLoader> _itemLookupLoaderMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly UpdateKitRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public UpdateKitRequestTests()
    {
        _handler = new UpdateKitRequest(
            _authenticatedUserContextMock.Object,
            _updateKitValidatorMock.Object,
            _itemLookupLoaderMock.Object,
            _clockServiceMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenKitNotFoundThrowsNotFoundException()
    {
        //Arrange
        _updateKitValidatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<CreateOrUpdateKitValidatorCoreDto>(), _token));

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() {Id = 2, Name = "test"}
            }.BuildMockDbSet().Object);

        var requestDto = GetUpdateKitDto();

        //Act
        var action = () => _handler.Handle(new UpdateKitRequest.Command(requestDto), _token);

        //Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.KitNotFound);
    }


    [Fact]
    public async Task HandleWhenKitArchivedThrowsInvalidDataAppException()
    {
        //Arrange
        _updateKitValidatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<CreateOrUpdateKitValidatorCoreDto>(), _token));

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() {Id = 1, Name = "test", Status = MerchItemStatus.Archived}
            }.BuildMockDbSet().Object);

        var requestDto = GetUpdateKitDto();

        //Act
        var action = () => _handler.Handle(new UpdateKitRequest.Command(requestDto), _token);

        //Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.KitIsNotAvailable);
    }

    [Fact]
    public async Task HandleShouldUpdateKit()
    {
        //Arrange
        _updateKitValidatorMock
            .Setup(validator => validator.ValidateAsync(It.IsAny<CreateOrUpdateKitValidatorCoreDto>(), _token));

        _clockServiceMock
            .Setup(service => service.UtcNow)
            .Returns(DateTime.UtcNow);

        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(1);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new() {Id = 1, Name = "Kit 1", Description = "Kit 1"}
            }.BuildMockDbSet().Object);

        _itemLookupLoaderMock
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), _token))
            .ReturnsAsync([
                new(1, "Cup", false),
                new(2, "Pen", false)
            ]);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>().BuildMockDbSet().Object);
        _dbContextMock
            .Setup(context => context.KitItems)
            .Returns(new List<KitItemEntity>().BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts.RemoveRange(It.IsAny<IEnumerable<KitMinAmountEntity>>()));
        _dbContextMock
            .Setup(context => context.KitItems.RemoveRange(It.IsAny<IEnumerable<KitItemEntity>>()));

        _dbContextMock.Setup(context => context.SaveChangesAsync(_token));

        var requestDto = GetUpdateKitDto(
            kitCompounds: new List<AddKitCompoundDto> { new(1, 1), new(2, 1) },
            minAmounts: new List<KitMinAmountDto> { new(1, 5), new(2, 5) });

        var expectedCompounds = new List<KitCompoundDto> { new(1, false, "Cup", 1), new(2, false, "Pen", 1), };
        var expectedMinAmounts = new List<KitMinAmountDto> { new(1, 5), new(2, 5) };

        //Act
        var kitDto = await _handler.Handle(new UpdateKitRequest.Command(requestDto), _token);

        //Assert
        kitDto.Id.Should().Be(1);
        kitDto.Name.Should().Be(requestDto.Name);
        kitDto.PhotoFileId.Should().Be(requestDto.PhotoFileId);
        kitDto.Description.Should().Be(requestDto.Description);

        kitDto.Compounds.Should().BeEquivalentTo(expectedCompounds);
        kitDto.MinAmounts.Should().BeEquivalentTo(expectedMinAmounts);
    }

    private UpdateKitDto GetUpdateKitDto(int id = 1, string name = "test kit name",
        List<AddKitCompoundDto>? kitCompounds = null, List<KitMinAmountDto>? minAmounts = null)
    {
        return new UpdateKitDto(
            1,
            name,
            null,
            null,
            kitCompounds ?? new List<AddKitCompoundDto>(),
            minAmounts ?? new List<KitMinAmountDto>());
    }
}
