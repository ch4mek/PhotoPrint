using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Calculators;

public class ClothingItemSizesPossibleAmountsCalculatorTests
{
    private readonly ClothingItemSizesPossibleAmountsCalculator _calculator = new();

    [Fact]
    public void CalculateWhenOneItemInKitReturnsValidListOfPossibleKitsAndStatusForEachSize()
    {
        //Arrange
        var storageName = "Minsk";

        var compounds = new List<KitCompoundCoreDto>
        {
            new() { AmountInKit = 1, IsClothing = true, Name = "Hoodie", Id = 1 },
        };

        var kitMinAmounts = new List<KitMinAmountEntity>
        {
            new() { KitId = 1, WarehouseId = 1, MinAmount = 3 },
        };

        var stockAmounts = new List<CompoundSizeAmountInStockCoreDto>
        {
            new() { ItemId = 1, SizeId = "XS", StorageName = storageName, StorageId = 1, SizeOrderWeight = 100, Amount = 1 },
            new() { ItemId = 1, SizeId = "S", StorageName = storageName, StorageId = 1, SizeOrderWeight = 200, Amount = 2 },
            new() { ItemId = 1, SizeId = "M", StorageName = storageName, StorageId = 1, SizeOrderWeight = 300, Amount = 5 },
            new() { ItemId = 1, SizeId = "L", StorageName = storageName, StorageId = 1, SizeOrderWeight = 400, Amount = 6 },
            new() { ItemId = 1, SizeId = "XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 500, Amount = 3 },
            new() { ItemId = 1, SizeId = "2XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 600, Amount = 1 },
            new() { ItemId = 1, SizeId = "3XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 700, Amount = 2 },
            new() { ItemId = 1, SizeId = "4XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 800, Amount = 1 }
        };

        var expectedResult = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new() { StorageName = storageName, SizeId = "XS", PossibleKitAmount = 1, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "S", PossibleKitAmount = 2, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "M", PossibleKitAmount = 5, Status = StorageAvailabilityStatus.Available },
            new() { StorageName = storageName, SizeId = "L", PossibleKitAmount = 6, Status = StorageAvailabilityStatus.Available },
            new() { StorageName = storageName, SizeId = "XL", PossibleKitAmount = 3, Status = StorageAvailabilityStatus.Available },
            new() { StorageName = storageName, SizeId = "2XL", PossibleKitAmount = 1, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "3XL", PossibleKitAmount = 2, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "4XL", PossibleKitAmount = 1, Status = StorageAvailabilityStatus.RunningOut },
        };

        //Act
        var result = _calculator.Calculate(kitMinAmounts, stockAmounts, compounds);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }

    [Fact]
    public void CalculateWhenTwoItemsInKitReturnsValidListOfPossibleKitsAndStatusForEachSize()
    {
        //Arrange
        var storageName = "Minsk";

        var compounds = new List<KitCompoundCoreDto>
        {
            new() { AmountInKit = 2, IsClothing = true, Name = "Hoodie", Id = 1 },
        };

        var kitMinAmounts = new List<KitMinAmountEntity>
        {
            new() { KitId = 1, WarehouseId = 1, MinAmount = 3 },
        };

        var stockAmounts = new List<CompoundSizeAmountInStockCoreDto>
        {
            new() { ItemId = 1, SizeId = "XS", StorageName = storageName, StorageId = 1, SizeOrderWeight = 100, Amount = 1 },
            new() { ItemId = 1, SizeId = "S", StorageName = storageName, StorageId = 1, SizeOrderWeight = 200, Amount = 2 },
            new() { ItemId = 1, SizeId = "M", StorageName = storageName, StorageId = 1, SizeOrderWeight = 300, Amount = 5 },
            new() { ItemId = 1, SizeId = "L", StorageName = storageName, StorageId = 1, SizeOrderWeight = 400, Amount = 6 },
            new() { ItemId = 1, SizeId = "XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 500, Amount = 3 },
            new() { ItemId = 1, SizeId = "2XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 600, Amount = 1 },
            new() { ItemId = 1, SizeId = "3XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 700, Amount = 2 },
            new() { ItemId = 1, SizeId = "4XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 800, Amount = 1 }
        };

        var expectedResult = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new() { StorageName = storageName, SizeId = "XS", PossibleKitAmount = 0, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "S", PossibleKitAmount = 1, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "M", PossibleKitAmount = 2, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "L", PossibleKitAmount = 3, Status = StorageAvailabilityStatus.Available },
            new() { StorageName = storageName, SizeId = "XL", PossibleKitAmount = 1, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "2XL", PossibleKitAmount = 0, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "3XL", PossibleKitAmount = 1, Status = StorageAvailabilityStatus.RunningOut },
            new() { StorageName = storageName, SizeId = "4XL", PossibleKitAmount = 0, Status = StorageAvailabilityStatus.RunningOut },
        };

        //Act
        var result = _calculator.Calculate(kitMinAmounts, stockAmounts, compounds);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }
}
