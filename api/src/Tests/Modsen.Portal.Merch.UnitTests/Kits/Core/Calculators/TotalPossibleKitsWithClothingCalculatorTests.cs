using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Calculators;

public class TotalPossibleKitsWithClothingCalculatorTests
{
    private readonly TotalPossibleKitsWithClothingCalculator _calculator = new();

    [Fact]
    public void CalculateWhenNonClothingAmountLessShouldReturnValidTotalPossibleKits()
    {
        //Arrange
        var compounds = new List<KitCompoundCoreDto>
        {
            new() { AmountInKit = 1, IsClothing = true, Name = "Hoodie", Id = 1 },
            new() { AmountInKit = 2, IsClothing = false, Name = "Postcard", Id = 2 }
        };

        var stockAmounts = new List<CompoundSizeAmountInStockCoreDto>
        {
            new() { ItemId = 1, SizeId = "XS", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 100, Amount = 1 },
            new() { ItemId = 1, SizeId = "S", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 200, Amount = 2 },
            new() { ItemId = 1, SizeId = "M", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 300, Amount = 5 },
            new() { ItemId = 1, SizeId = "L", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 400, Amount = 6 },
            new() { ItemId = 1, SizeId = "XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 500, Amount = 3 },
            new() { ItemId = 1, SizeId = "2XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 600, Amount = 1 },
            new() { ItemId = 1, SizeId = "3XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 700, Amount = 2 },
            new() { ItemId = 1, SizeId = "4XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 800, Amount = 1 },

            new() { ItemId = 2, SizeId = "One", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 0, Amount = 5 },
        };

        //Act
        var result = _calculator.Calculate(stockAmounts, compounds);

        //Assert
        result.Should().Be(2); // because only 5 postcards and for each kit 2 postcards needed: 5/2 = 2 full kits

    }

    [Fact]
    public void CalculateWhenClothingAmountLessShouldReturnValidTotalPossibleKits()
    {
        //Arrange
        var compounds = new List<KitCompoundCoreDto>
        {
            new() { AmountInKit = 1, IsClothing = true, Name = "Hoodie", Id = 1 },
            new() { AmountInKit = 2, IsClothing = false, Name = "Postcard", Id = 2 }
        };

        var stockAmounts = new List<CompoundSizeAmountInStockCoreDto>
        {
            new() { ItemId = 1, SizeId = "XS", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 100, Amount = 1 },
            new() { ItemId = 1, SizeId = "S", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 200, Amount = 2 },
            new() { ItemId = 1, SizeId = "M", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 300, Amount = 5 },
            new() { ItemId = 1, SizeId = "L", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 400, Amount = 6 },
            new() { ItemId = 1, SizeId = "XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 500, Amount = 3 },
            new() { ItemId = 1, SizeId = "2XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 600, Amount = 1 },
            new() { ItemId = 1, SizeId = "3XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 700, Amount = 2 },
            new() { ItemId = 1, SizeId = "4XL", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 800, Amount = 1 },

            new() { ItemId = 2, SizeId = "One", StorageName = "Minsk", StorageId = 1, SizeOrderWeight = 0, Amount = 100 },
        };

        //Act
        var result = _calculator.Calculate(stockAmounts, compounds);

        //Assert
        result.Should().Be(21); // because only 21 hoodies and for each kit 1 hoodie needed: 21 full kits

    }
}
