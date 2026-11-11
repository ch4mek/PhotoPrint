using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Calculators;

public class NonClothingItemSizesPossibleAmountsCalculatorTests
{
    private readonly NonClothingItemSizesPossibleAmountsCalculator _calculator = new();

    [Fact]
    public void CalculateWhenKitStatusRunningOutShouldReturnValidPossibleKitsAndStatus()
    {
        //Arrange
        var storageName = "Minsk";

        var compounds = new List<KitCompoundCoreDto>
        {
            new() { AmountInKit = 2, IsClothing = false, Name = "Postcard", Id = 1 },
            new() { AmountInKit = 1, IsClothing = false, Name = "Note book", Id = 2 },
            new() { AmountInKit = 1, IsClothing = true, Name = "Hoodie", Id = 3 },
        };

        var kitMinAmounts = new List<KitMinAmountEntity>
        {
            new() { KitId = 1, WarehouseId = 1, MinAmount = 3 },
        };

        var stockAmounts = new List<CompoundSizeAmountInStockCoreDto>
        {
            new() { ItemId = 3, SizeId = "XS", StorageName = storageName, StorageId = 1, SizeOrderWeight = 100, Amount = 1 },
            new() { ItemId = 3, SizeId = "S", StorageName = storageName, StorageId = 1, SizeOrderWeight = 200, Amount = 2 },
            new() { ItemId = 3, SizeId = "M", StorageName = storageName, StorageId = 1, SizeOrderWeight = 300, Amount = 5 },
            new() { ItemId = 3, SizeId = "L", StorageName = storageName, StorageId = 1, SizeOrderWeight = 400, Amount = 6 },
            new() { ItemId = 3, SizeId = "XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 500, Amount = 3 },
            new() { ItemId = 3, SizeId = "2XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 600, Amount = 1 },
            new() { ItemId = 3, SizeId = "3XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 700, Amount = 2 },
            new() { ItemId = 3, SizeId = "4XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 800, Amount = 1 },

            new() { ItemId = 1, SizeId = "One", StorageName = storageName, StorageId = 1, SizeOrderWeight = 0, Amount = 4 },

            new() { ItemId = 2, SizeId = "One", StorageName = storageName, StorageId = 1, SizeOrderWeight = 0, Amount = 10 },
        };

        var expectedResult = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new() { StorageName = storageName, SizeId = "One", PossibleKitAmount = 2, Status = StorageAvailabilityStatus.RunningOut },
        };

        //Act
        var result = _calculator.Calculate(kitMinAmounts, stockAmounts, compounds);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }

    [Fact]
    public void CalculateWhenKitStatusAvailableShouldReturnValidPossibleKitsAndStatus()
    {
        //Arrange
        var storageName = "Minsk";

        var compounds = new List<KitCompoundCoreDto>
        {
            new() { AmountInKit = 2, IsClothing = false, Name = "Postcard", Id = 1 },
            new() { AmountInKit = 1, IsClothing = false, Name = "Note book", Id = 2 },
            new() { AmountInKit = 1, IsClothing = true, Name = "Hoodie", Id = 3 },
        };

        var kitMinAmounts = new List<KitMinAmountEntity>
        {
            new() { KitId = 1, WarehouseId = 1, MinAmount = 3 },
        };

        var stockAmounts = new List<CompoundSizeAmountInStockCoreDto>
        {
            new() { ItemId = 3, SizeId = "XS", StorageName = storageName, StorageId = 1, SizeOrderWeight = 100, Amount = 1 },
            new() { ItemId = 3, SizeId = "S", StorageName = storageName, StorageId = 1, SizeOrderWeight = 200, Amount = 2 },
            new() { ItemId = 3, SizeId = "M", StorageName = storageName, StorageId = 1, SizeOrderWeight = 300, Amount = 5 },
            new() { ItemId = 3, SizeId = "L", StorageName = storageName, StorageId = 1, SizeOrderWeight = 400, Amount = 6 },
            new() { ItemId = 3, SizeId = "XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 500, Amount = 3 },
            new() { ItemId = 3, SizeId = "2XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 600, Amount = 1 },
            new() { ItemId = 3, SizeId = "3XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 700, Amount = 2 },
            new() { ItemId = 3, SizeId = "4XL", StorageName = storageName, StorageId = 1, SizeOrderWeight = 800, Amount = 1 },

            new() { ItemId = 1, SizeId = "One", StorageName = storageName, StorageId = 1, SizeOrderWeight = 0, Amount = 10 },

            new() { ItemId = 2, SizeId = "One", StorageName = storageName, StorageId = 1, SizeOrderWeight = 0, Amount = 10 },
        };

        var expectedResult = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new() { StorageName = storageName, SizeId = "One", PossibleKitAmount = 5, Status = StorageAvailabilityStatus.Available },
        };

        //Act
        var result = _calculator.Calculate(kitMinAmounts, stockAmounts, compounds);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }

}
