using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Validators;

public class CreateEquipmentRequestDtoValidatorTests
{
    private readonly CreateEquipmentRequestDto.Validator _validator = new();

    private readonly CreateEquipmentRequestDto _validCommand = new()
    {
        Name = "Valid Name",
        EquipmentTypeId = "Laptop",
        BrokenStatus = EquipmentBrokenStatus.None,
        Location = EquipmentLocationType.Belarus,
        SerialNumber = "Valid SerialNumber",
        PurchasePrice = 5999.99m,
        PurchaseCurrency = MoneyCurrencyType.Byn,
        PurchasePriceUsd = 1999.99m,
        PurchaseDate = new DateTime(2023, 05, 22, 0, 0, 0, DateTimeKind.Utc),
        PurchasePlace = "Valid PurchasePlace",
        GuaranteeDate = new DateTime(2026, 05, 22, 0, 0, 0, DateTimeKind.Utc),
        Characteristics = "Valid Characteristics",
        Comment = "Valid Comment",
        InvoiceNumber = "DSAMVAS123ASKD123",
        Storage = LocationType.Vitebsk,
        RepairPlace = null,
        ApproverId = 1,
        JiraTask = "qwe"
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-22)]
    public void ValidateWhenApproverIdIsNotValidShouldReturnInvalidResult(int approverId)
    {
        // Arrange
        var command = _validCommand with { ApproverId = approverId };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenCharacteristicsIsNotValidShouldReturnInvalidResult()
    {
        // Arrange
        var command = _validCommand with { Characteristics = string.Empty };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidComments))]
    public void ValidateWhenCommentIsNotValidShouldReturnInvalidResult(string? comment)
    {
        // Arrange
        var command = _validCommand with { Comment = comment };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidGuaranteeDates))]
    public void ValidateWhenGuaranteeDateIsNotValidShouldReturnInvalidResult(DateTime guaranteeDate, string message)
    {
        // Arrange
        var command = _validCommand with { GuaranteeDate = guaranteeDate };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(e => e.ErrorMessage == message).Should().BeTrue();
    }

    [Theory]
    [InlineData((EquipmentLocationType)(-1))]
    [InlineData((EquipmentLocationType)3)]
    [InlineData((EquipmentLocationType)4)]
    public void ValidateWhenLocationIsNotValidShouldReturnInvalidResult(EquipmentLocationType locationType)
    {
        // Arrange
        var command = _validCommand with { Location = locationType };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidNames))]
    public void ValidateWhenNameIsNotValidShouldReturnInvalidResult(string name)
    {
        // Arrange
        var command = _validCommand with { Name = name };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidPurchaseDates))]
    public void ValidateWhenPurchaseDateIsNotValidShouldReturnInvalidResult(DateTime purchaseDate, string message)
    {
        // Arrange
        var command = _validCommand with { PurchaseDate = purchaseDate };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Any(e => e.ErrorMessage == message).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidPurchasePlaces))]
    public void ValidateWhenPurchasePlaceIsNotValidShouldReturnInvalidResult(string purchasePlace)
    {
        // Arrange
        var command = _validCommand with { PurchasePlace = purchasePlace };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-55.99)]
    [InlineData(-0.01)]
    [InlineData(0.001)]
    [InlineData(0.000064)]
    [InlineData(1E-13)]
    [InlineData(1E+10)]
    [InlineData(15E-3)]
    public void ValidateWhenPurchasePriceIsNotValidShouldReturnInvalidResult(decimal purchasePrice)
    {
        // Arrange
        var command = _validCommand with { PurchasePrice = purchasePrice };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-55.99)]
    [InlineData(-0.01)]
    [InlineData(0.001)]
    [InlineData(0.000064)]
    [InlineData(1E-13)]
    [InlineData(1E+10)]
    [InlineData(15E-3)]
    public void ValidateWhenPurchasePriceUsdIsNotValidShouldReturnInvalidResult(decimal purchasePriceUsd)
    {
        // Arrange
        var command = _validCommand with { PurchasePriceUsd = purchasePriceUsd };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(GetInvalidSerialNumbers))]
    public void ValidateWhenSerialNumberIsNotValidShouldReturnInvalidResult(string serialNumber)
    {
        // Arrange
        var command = _validCommand with { SerialNumber = serialNumber };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenCommentIsValidShouldReturnValidResult()
    {
        // Arrange
        var command = _validCommand with { Comment = null };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidRepairPlaces))]
    public void ValidateWhenRepairPlaceIsNotValidShouldReturnInvalidResult(string repairPlace)
    {
        // Arrange
        var command = _validCommand with { RepairPlace = repairPlace };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    public static IEnumerable<object[]> GetInvalidComments()
    {
        yield return [new string('a', 1001)];
        yield return [new string('b', 1054)];
        yield return ["b"];
        yield return ["a"];
    }

    public static IEnumerable<object[]> GetInvalidGuaranteeDates()
    {
        yield return [new DateTime(2026, 07, 07, 11, 29, 55, DateTimeKind.Utc), "Time must be zero"];
        yield return [new DateTime(2026, 07, 07, 0, 0, 1, DateTimeKind.Utc), "Time must be zero"];
    }

    public static IEnumerable<object[]> GetInvalidNames()
    {
        yield return [new string('a', 251)];
        yield return [new string('n', 274)];
        yield return ["a"];
    }

    public static IEnumerable<object[]> GetInvalidPurchaseDates()
    {
        yield return [new DateTime(2026, 07, 07, 11, 29, 55, DateTimeKind.Utc), "Time must be zero"];
        yield return [new DateTime(2026, 07, 07, 0, 0, 1, DateTimeKind.Utc), "Time must be zero"];
    }

    public static IEnumerable<object[]> GetInvalidPurchasePlaces()
    {
        yield return [string.Empty];
        yield return [new string('Y', 251)];
        yield return ["*TEST*"];
        yield return ["----------"];
        yield return ["          "];
    }

    public static IEnumerable<object[]> GetInvalidSerialNumbers()
    {
        yield return [new string('a', 251)];
        yield return [new string('n', 274)];
        yield return ["a"];
    }

    public static IEnumerable<object[]> GetInvalidRepairPlaces()
    {
        yield return [string.Empty];
        yield return [new string('Y', 251)];
        yield return ["*TEST*"];
        yield return ["----------"];
        yield return ["          "];
    }
}
