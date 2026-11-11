using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Core.Calculators;

public class KitPossibleAmountCalculatorTests
{
    private readonly Mock<INonClothingItemSizesPossibleAmountsCalculator> _possibleNonClothingAmountsCalculatorMock = new();
    private readonly Mock<ITotalPossibleKitsWithClothingCalculator> _totalPossibleKitsWithClothingCalculatorMock = new();
    private readonly Mock<IClothingItemSizesPossibleAmountsCalculator> _possibleClothingAmountsCalculatorMock = new();

    private readonly KitPossibleAmountCalculator _calculator;

    public KitPossibleAmountCalculatorTests()
    {
        _calculator = new KitPossibleAmountCalculator(
            _possibleNonClothingAmountsCalculatorMock.Object,
            _totalPossibleKitsWithClothingCalculatorMock.Object,
            _possibleClothingAmountsCalculatorMock.Object);
    }

    [Fact]
    public void CalculateWhenKitNotContainClothingReturnsResultOfNonClothingItemSizesPossibleAmountsCalculator()
    {
        //Arrange
        var kitCoreDto = new KitWithCompoundsCoreDto()
        {
            Id = 1,
            ContainsClothing = false,
            Name = "Test kit",
            Compounds = new List<KitCompoundCoreDto>()
        };

        var kitMinAmounts = new List<KitMinAmountEntity>();
        var compoundsInStock = new List<CompoundSizeAmountInStockCoreDto>();

        var possibleAmountDtos = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new()
            {
                StorageName = "Minsk",
                SizeId = "S",
                Status = StorageAvailabilityStatus.Available,
                PossibleKitAmount = 10
            },
            new()
            {
                StorageName = "Minsk",
                SizeId = "M",
                Status = StorageAvailabilityStatus.Available,
                PossibleKitAmount = 15
            }
        };

        var expectedTotalKits = possibleAmountDtos[0].PossibleKitAmount + possibleAmountDtos[1].PossibleKitAmount;

        _possibleNonClothingAmountsCalculatorMock
            .Setup(calculator => calculator.Calculate(kitMinAmounts, compoundsInStock, kitCoreDto.Compounds))
            .Returns(possibleAmountDtos);

        //Act
        var result = _calculator.Calculate(kitCoreDto, kitMinAmounts, compoundsInStock);

        //Assert
        result.PossibleKits.Should().BeEquivalentTo(possibleAmountDtos);
        result.TotalPossibleKits.Should().Be(expectedTotalKits);
    }

    [Fact]
    public void CalculateWhenKitContainsClothingAndAmountOfNonClothingSizeIsLessReturnsStatusAndAmountAccordingToNonClothing()
    {
        //Arrange
        var kitCoreDto = new KitWithCompoundsCoreDto()
        {
            Id = 1,
            ContainsClothing = true,
            Name = "Test kit",
            Compounds = new List<KitCompoundCoreDto>()
        };

        var kitMinAmounts = new List<KitMinAmountEntity>();
        var compoundsInStock = new List<CompoundSizeAmountInStockCoreDto>();

        var possibleAmountOfNonClothingDtos = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new()
            {
                StorageName = "Minsk",
                SizeId = "One",
                Status = StorageAvailabilityStatus.RunningOut,
                PossibleKitAmount = 10
            }
        };

        var possibleAmountOfClothingDtos = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new()
            {
                StorageName = "Minsk",
                SizeId = "S",
                Status = StorageAvailabilityStatus.Available,
                PossibleKitAmount = 100
            },
            new()
            {
                StorageName = "Minsk",
                SizeId = "M",
                Status = StorageAvailabilityStatus.Available,
                PossibleKitAmount = 100
            }
        };

        var totalPossibleAmountDtos = new List<KitSizeInStoragePossibleAmountCoreDto>
        {
            new()
            {
                StorageName = "Minsk",
                SizeId = "S",
                Status = StorageAvailabilityStatus.RunningOut,
                PossibleKitAmount = 10
            },
            new()
            {
                StorageName = "Minsk",
                SizeId = "M",
                Status = StorageAvailabilityStatus.RunningOut,
                PossibleKitAmount = 10
            }
        };

        _possibleNonClothingAmountsCalculatorMock
            .Setup(calculator => calculator.Calculate(kitMinAmounts, compoundsInStock, kitCoreDto.Compounds))
            .Returns(possibleAmountOfNonClothingDtos);

        _possibleClothingAmountsCalculatorMock
            .Setup(calculator => calculator.Calculate(kitMinAmounts, compoundsInStock, kitCoreDto.Compounds))
            .Returns(possibleAmountOfClothingDtos);

        _totalPossibleKitsWithClothingCalculatorMock
            .Setup(calculator => calculator.Calculate(compoundsInStock, kitCoreDto.Compounds))
            .Returns(10);

        //Act
        var result = _calculator.Calculate(kitCoreDto, kitMinAmounts, compoundsInStock);

        //Assert
        result.PossibleKits.Should().BeEquivalentTo(totalPossibleAmountDtos);
        result.TotalPossibleKits.Should().Be(10);
    }

}
