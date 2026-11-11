using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;
using Modsen.Portal.Merch.Requests.Kits.Requests;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests;

public class GetKitCountByIdRequestTests
{
    private readonly GetKitCountsByIdRequest _handler;

    private readonly Mock<IItemsCountLoader> _itemsCountLoader = new();
    private readonly Mock<IKitPossibleAmountCalculator> _kitPossibleAmountCalculator = new();
    private readonly Mock<IKitsWithCompoundsLoader> _kitWithCompoundsLoader = new();
    private readonly Mock<ICompoundSizesInStockResolver> _compoundSizesInStockResolver = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetKitCountByIdRequestTests()
    {
        _handler = new GetKitCountsByIdRequest(_itemsCountLoader.Object, _kitPossibleAmountCalculator.Object,
            _kitWithCompoundsLoader.Object, _compoundSizesInStockResolver.Object);
    }

    [Fact]
    public async Task GetKitCountByIdWhenKitNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        var kitId = 1;
        var storageId = 1;

        var kits = new List<KitWithCompoundsCoreDto>();

        _kitWithCompoundsLoader
            .Setup(x => x.LoadAsync(new List<int> { kitId }, new List<int> { storageId },
                _cancellationToken))
            .ReturnsAsync(kits);

        var request = new GetKitCountsByIdRequest.Query(kitId, storageId);

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task GetKitCountByIdWhenShould()
    {
        // Arrange
        var kitId = 1;
        var storageId = 1;

        var size = new MerchSizeEntity { OrderWeight = 0, Id = "One", Name = "test" };
        var sizes = new List<MerchSizeEntity> { size };
        var minskStorage = new WarehouseEntity { Location = "Minsk", Id = 1 };
        var vitebskStorage = new WarehouseEntity { Location = "Vitebsk", Id = 2 };

        var kits = new List<KitWithCompoundsCoreDto>
        {
            new()
            {
                Id = 1,
                Name = "kit1",
                Description = "1",
                PhotoFileId = "1",
                ContainsClothing = true,
                AllStorages = [minskStorage.Location, vitebskStorage.Location],
                Compounds =
                [
                    new()
                    {
                        Name = "item1",
                        AmountInKit = 2,
                        Id = 1,
                        IsClothing = true
                    },

                    new()
                    {
                        Name = "item2",
                        AmountInKit = 1,
                        Id = 2,
                        IsClothing = false
                    }
                ]
            },
        };

        _kitWithCompoundsLoader
            .Setup(x => x.LoadAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<IReadOnlyCollection<int>>(),
                _cancellationToken))
            .ReturnsAsync(kits);

        var kitCompoundsInStock = new List<CompoundSizeAmountInStockCoreDto> { };

        var total = new KitTotalWithPossibleInStorageCoreDto
        {
            PossibleKits = new List<KitSizeInStoragePossibleAmountCoreDto>
            {
                new KitSizeInStoragePossibleAmountCoreDto
                {
                    StorageName = minskStorage.Location,
                    SizeId = size.Id,
                    PossibleKitAmount = 2,
                    Status = StorageAvailabilityStatus.Available,
                }
            },
            TotalPossibleKits = 2
        };

        _compoundSizesInStockResolver
            .Setup(x => x.ResolveAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<IReadOnlyCollection<int>>(),
                _cancellationToken))
            .ReturnsAsync(kitCompoundsInStock);

        _kitPossibleAmountCalculator
            .Setup(x => x.Calculate(kits.First(), new List<KitMinAmountEntity>(), kitCompoundsInStock))
            .Returns(total);

        var items = new List<ItemSizeCountDto>
        {
            new ItemSizeCountDto
            {
                ItemId = 1,
                CountInKit = 2,
                SizeCounts = [new SizeCountDto {SizeId = size.Id, Count = 1}]
            }
        };

        _itemsCountLoader
            .Setup(x => x.LoadAsync(It.IsAny<int[]>(), storageId, _cancellationToken))
            .ReturnsAsync(items);

        var request = new GetKitCountsByIdRequest.Query(kitId, storageId);

        var expected = new KitCountDto
        {
            AvailableAmountInStorage = 2,
            ItemSizes = [
                new ItemSizeCountDto{CountInKit = 2, ItemId = 1, SizeCounts = [new SizeCountDto { SizeId = size.Id, Count = 1}]}
            ]
        };

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
