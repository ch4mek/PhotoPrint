using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Generators;

namespace Modsen.Portal.Equipment.UnitTests.Generators;

public class EquipmentInventoryNumberGeneratorTests
{
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock;
    private readonly IEquipmentInventoryNumberGenerator _equipmentInventoryNumberGenerator;
    private readonly CancellationToken _cancellationToken;

    public EquipmentInventoryNumberGeneratorTests()
    {
        _equipmentDbContextMock = new Mock<IEquipmentDbContext>();
        _cancellationToken = CancellationToken.None;

        _equipmentInventoryNumberGenerator = new EquipmentInventoryNumberGenerator(_equipmentDbContextMock.Object);
    }

    private EquipmentEntity GetEntity(DateTime purchaseDate, string? inventoryNumber = null)
    {
        return new EquipmentEntity
        {
            Name = "TestEquipment",
            BrokenStatus = EquipmentBrokenStatus.None,
            Location = EquipmentLocationType.Belarus,
            SerialNumber = Guid.CreateVersion7().ToString(),
            InventoryNumber = inventoryNumber,
            PurchasePrice = 3999.99m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 1999.99m,
            PurchaseDate = purchaseDate,
            PurchasePlace = "TestPlace",
            GuaranteeDate = purchaseDate.AddYears(2),
            Characteristics = "TestCharacteristics",
            Comment = "TestComment",
            InvoiceNumber = Guid.CreateVersion7().ToString(),
            Storage = LocationType.Vitebsk,
            RepairPlace = null,
            ApproverId = 1,
            JiraTask = "TEST-123"
        };
    }

    [Fact]
    public async Task GenerateWhenNoEquipmentExistsReturnsFirstInventoryNumber()
    {
        // Arrange
        var purchaseDate = new DateTime(2025, 10, 06);
        var expectedNumber = "202510061D";
        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _equipmentInventoryNumberGenerator.Generate(purchaseDate, _cancellationToken);

        // Assert
        result.Should().Be(expectedNumber);
    }

    [Fact]
    public async Task GenerateWhenEquipmentWithNullInventoryNumbersReturnsFirstInventoryNumber()
    {
        // Arrange
        var purchaseDate = new DateTime(2025, 10, 06);
        var expectedNumber = "202510061D";
        var equipments = new List<EquipmentEntity>
        {
            GetEntity(purchaseDate),
            GetEntity(purchaseDate)
        };
        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);

        // Act
        var result = await _equipmentInventoryNumberGenerator.Generate(purchaseDate, _cancellationToken);

        // Assert
        result.Should().Be(expectedNumber);
    }

    [Fact]
    public async Task GenerateWhenEquipmentWithSameDateExistsReturnsIncrementedInventoryNumber()
    {
        // Arrange
        var purchaseDate = new DateTime(2025, 10, 06);
        var expectedNumber = "202510063D";
        var equipments = new List<EquipmentEntity>
        {
            GetEntity(purchaseDate, "202510061D"),
            GetEntity(purchaseDate, "202510062D")
        };
        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);

        // Act
        var result = await _equipmentInventoryNumberGenerator.Generate(purchaseDate, _cancellationToken);

        // Assert
        result.Should().Be(expectedNumber);
    }

    [Fact]
    public async Task GenerateWhenEquipmentWithDifferentDateExistsReturnsFirstInventoryNumberForNewDate()
    {
        // Arrange
        var purchaseDate = new DateTime(2025, 10, 06);
        var expectedNumber = "202510061D";
        var equipments = new List<EquipmentEntity>
        {
            GetEntity(new DateTime(2025, 10, 05), "202510051D"),
            GetEntity(new DateTime(2025, 10, 05), "202510052D")
        };
        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);

        // Act
        var result = await _equipmentInventoryNumberGenerator.Generate(purchaseDate, _cancellationToken);

        // Assert
        result.Should().Be(expectedNumber);
    }

    [Fact]
    public async Task GenerateWhenInvalidInventoryNumberInDatabaseSkipsInvalidAndGeneratesNextNumber()
    {
        // Arrange
        var purchaseDate = new DateTime(2025, 10, 06);
        var expectedNumber = "202510062D";
        var equipments = new List<EquipmentEntity>
        {
            GetEntity(purchaseDate, "202510061D"),
            GetEntity(purchaseDate, "InvalidNumber"),
            GetEntity(purchaseDate, "20251006X")
        };
        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);

        // Act
        var result = await _equipmentInventoryNumberGenerator.Generate(purchaseDate, _cancellationToken);

        // Assert
        result.Should().Be(expectedNumber);
    }

    [Fact]
    public async Task GenerateWhenMultipleEquipmentsWithMixedInventoryNumbersGeneratesCorrectNumber()
    {
        // Arrange
        var purchaseDate = new DateTime(2025, 10, 06);
        var expectedNumber = "202510064D";
        var equipments = new List<EquipmentEntity>
        {
            GetEntity(purchaseDate, "202510061D"),
            GetEntity(purchaseDate, "202510063D"),
            GetEntity(purchaseDate),
            GetEntity(new DateTime(2025, 10, 05), "202510052D")
        };
        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);

        // Act
        var result = await _equipmentInventoryNumberGenerator.Generate(purchaseDate, _cancellationToken);

        // Assert
        result.Should().Be(expectedNumber);
    }
}
