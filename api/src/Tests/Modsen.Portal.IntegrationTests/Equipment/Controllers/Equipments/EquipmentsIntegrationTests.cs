using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Equipment.Controllers.Equipments;


[Collection("GeneralCollection")]
public class EquipmentsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly string _summaryReportWorksheet = "Summary Report";
    private readonly string _defaultDateFormat = "dd/MM/yyyy";

    public EquipmentsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenAuthAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var updateEquipmentInput = new UpdateEquipmentRequestDto()
        {
            Id = 100,
            SerialNumber = Guid.CreateVersion7().ToString(),
            InventoryNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "wwwwww"
        };

        // Act
        var response = await client.UpdateEquipmentAsResponseMessageAsync(updateEquipmentInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenEquipmentDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var updateEquipmentInput = new UpdateEquipmentRequestDto
        {
            Id = 101,
            SerialNumber = Guid.CreateVersion7().ToString(),
            InventoryNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "zzzzzz"
        };

        // Act
        var response = await client.UpdateEquipmentAsResponseMessageAsync(updateEquipmentInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenRequestIsInvalidResponseWithBadRequestResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var updateEquipmentInput = new UpdateEquipmentRequestDto
        {
            Id = 102,
            SerialNumber = Guid.CreateVersion7().ToString(),
            InventoryNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 2, 10, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 2, 10, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "zzzzzz"
        };

        // Act
        var response = await client.UpdateEquipmentAsResponseMessageAsync(updateEquipmentInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUpdateAndGetEquipmentWhenAuthAsCeoResponseWithUpdatedEquipment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwwwwwe"
        };

        var updateEquipmentInput = new UpdateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            InventoryNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CtoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (8Gbps), 6800 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = null,
            InvoiceNumber = "AKSFASC321",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Poland,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 150,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 55.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwwwwwe"
        };

        var expectedEquipment = new EquipmentOutput
        {
            SerialNumber = updateEquipmentInput.SerialNumber,
            InventoryNumber = updateEquipmentInput.InventoryNumber,
            Status = updateEquipmentInput.BrokenStatus.GetDescription(),
            Characteristics = updateEquipmentInput.Characteristics,
            Comment = updateEquipmentInput.Comment,
            InvoiceNumber = updateEquipmentInput.InvoiceNumber,
            EquipmentTypeId = updateEquipmentInput.EquipmentTypeId,
            GuaranteeDate = updateEquipmentInput.GuaranteeDate,
            Location = updateEquipmentInput.Location.GetDescription(),
            Name = updateEquipmentInput.Name,
            PurchaseDate = updateEquipmentInput.PurchaseDate,
            PurchasePlace = updateEquipmentInput.PurchasePlace,
            PurchasePrice = updateEquipmentInput.PurchasePrice,
            PurchaseCurrency = updateEquipmentInput.PurchaseCurrency.GetDescription(),
            PurchasePriceAndCurrency = $"{updateEquipmentInput.PurchasePrice} {updateEquipmentInput.PurchaseCurrency.GetDescription()}",
            PurchasePriceUsd = updateEquipmentInput.PurchasePriceUsd,
            Storage = updateEquipmentInput.Storage,
            RepairPlace = updateEquipmentInput.RepairPlace,
            JiraTask = updateEquipmentInput.JiraTask,
            Approver = new ApproverOutput
            {
                Id = updateEquipmentInput.ApproverId,
                FullName = "CTO Test"
            },
            ActiveHolder = null,
            Type = new EquipmentTypeOutput(
                createEquipmentInput.EquipmentTypeId,
                createEquipmentInput.EquipmentTypeId,
                "PC accessory: HDD"
            )
        };

        // Act
        var createEquipmentResponse = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        var createdEquipment = await createEquipmentResponse.Content.ReadFromJsonAsync<EquipmentOutput>(
            _jsonOptions,
            _cancellationToken);

        updateEquipmentInput.Id = createdEquipment!.Id;

        var updateEquipmentResponse = await client.UpdateEquipmentAsResponseMessageAsync(updateEquipmentInput);

        var equipment = await client.GetEquipmentAsync(createdEquipment.Id);

        // Assert
        createEquipmentResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updateEquipmentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        equipment.Should().NotBeNull();
        equipment.Status.Should().BeEquivalentTo(EquipmentAvailabilityType.Available.GetDescription());
        equipment.Should().BeEquivalentTo(
            expectedEquipment,
            config => config.Excluding(x => x.Id)
                            .Excluding(x => x.Status));
    }

    [Fact]
    public async Task CreateEquipmentWhenAuthenticatedAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        // Act
        var response = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateEquipmentWhenRequestIsInvalidResponseWithBadRequestResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.005m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        // Act
        var response = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateEquipmentWhenApproverIsNotInValidRoleResponseWithBadRequestResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.EmployeeUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.005m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        // Act
        var response = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateEquipmentWhenRequestIsValidResponseWith201AndCreatedEquipment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var expectedEquipmentOutput = new EquipmentOutput
        {
            SerialNumber = createEquipmentInput.SerialNumber,
            Status = createEquipmentInput.BrokenStatus.GetDescription(),
            Characteristics = createEquipmentInput.Characteristics,
            Comment = createEquipmentInput.Comment,
            InvoiceNumber = createEquipmentInput.InvoiceNumber,
            EquipmentTypeId = createEquipmentInput.EquipmentTypeId,
            GuaranteeDate = createEquipmentInput.GuaranteeDate,
            Location = createEquipmentInput.Location.GetDescription(),
            Name = createEquipmentInput.Name,
            PurchaseDate = createEquipmentInput.PurchaseDate,
            PurchasePlace = createEquipmentInput.PurchasePlace,
            PurchasePrice = createEquipmentInput.PurchasePrice,
            PurchaseCurrency = createEquipmentInput.PurchaseCurrency.GetDescription(),
            PurchasePriceAndCurrency = $"{createEquipmentInput.PurchasePrice} {createEquipmentInput.PurchaseCurrency.GetDescription()}",
            PurchasePriceUsd = createEquipmentInput.PurchasePriceUsd,
            JiraTask = createEquipmentInput.JiraTask,
            ActiveHolder = null,
            Storage = LocationType.Vitebsk,
            Approver = new ApproverOutput
            {
                Id = createEquipmentInput.ApproverId,
                FullName = "CTO Test"
            },
            Type = new EquipmentTypeOutput(
                createEquipmentInput.EquipmentTypeId,
                createEquipmentInput.EquipmentTypeId,
                "PC accessory: HDD"
                )
        };

        // Act
        var response = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        // Assert
        var body = await response.Content.ReadFromJsonAsync<EquipmentOutput>(
            _jsonOptions,
            _cancellationToken);

        body.Should().NotBeNull();
        body.Status.Should().BeEquivalentTo(EquipmentAvailabilityType.Available.GetDescription());
        body.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(expectedEquipmentOutput, config =>
                config.Excluding(x => x.Id)
                      .Excluding(x => x.Approver.FullName)
                      .Excluding(x => x.Status)
                      .Excluding(x => x.InventoryNumber));
        body.Id.Should().BeGreaterThan(0);
        body.Approver.FullName.Should().NotBeEmpty();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location?.ToString().Should().EndWithEquivalentOf($"api/equipment/{body.Id}");
    }

    [Fact]
    public async Task CreateEquipmentWhenAllTextPropertiesHaveMaxLengthResponseWith201AndCreatedEquipment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = new string('a', 1000),
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = new string('a', 250),
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = new string('a', 250),
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            InvoiceNumber = "LVKAMmqevma13dASF",
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var expectedEquipmentOutput = new EquipmentOutput
        {
            SerialNumber = createEquipmentInput.SerialNumber,
            Status = createEquipmentInput.BrokenStatus.GetDescription(),
            Characteristics = createEquipmentInput.Characteristics,
            Comment = createEquipmentInput.Comment,
            InvoiceNumber = createEquipmentInput.InvoiceNumber,
            EquipmentTypeId = createEquipmentInput.EquipmentTypeId,
            GuaranteeDate = createEquipmentInput.GuaranteeDate,
            Location = createEquipmentInput.Location.GetDescription(),
            Name = createEquipmentInput.Name,
            PurchaseDate = createEquipmentInput.PurchaseDate,
            PurchasePlace = createEquipmentInput.PurchasePlace,
            PurchasePrice = createEquipmentInput.PurchasePrice,
            PurchaseCurrency = createEquipmentInput.PurchaseCurrency.GetDescription(),
            PurchasePriceAndCurrency = $"{createEquipmentInput.PurchasePrice} {createEquipmentInput.PurchaseCurrency.GetDescription()}",
            PurchasePriceUsd = createEquipmentInput.PurchasePriceUsd,
            JiraTask = createEquipmentInput.JiraTask,
            ActiveHolder = null,
            Storage = LocationType.Vitebsk,
            Approver = new ApproverOutput
            {
                Id = createEquipmentInput.ApproverId,
                FullName = "CTO Test"
            },
            Type = new EquipmentTypeOutput("PcAccessoriesHdd", "PcAccessoriesHdd", "PC accessory: HDD")
        };

        // Act
        var response = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        // Assert
        var body = await response.Content.ReadFromJsonAsync<EquipmentOutput>(
            _jsonOptions,
            _cancellationToken);

        body.Should().NotBeNull();
        body.Status.Should().BeEquivalentTo(EquipmentAvailabilityType.Available.GetDescription());
        body.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(expectedEquipmentOutput, config =>
                config.Excluding(x => x.Id)
                      .Excluding(x => x.Approver.FullName)
                      .Excluding(x => x.Status)
                      .Excluding(x => x.InventoryNumber));
        body.Id.Should().BeGreaterThan(0);
        body?.Approver?.FullName.Should().NotBeEmpty();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location?.ToString().Should().EndWithEquivalentOf($"api/equipment/{body!.Id}");
    }

    [Fact]
    public async Task CreateEquipmentWithAssignWhenRequestIsValidResponseWith201AndCreatedEquipment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var expectedEquipmentOutput = new EquipmentOutput
        {
            SerialNumber = createEquipmentInput.SerialNumber,
            Characteristics = createEquipmentInput.Characteristics,
            Status = EquipmentBrokenStatus.None.GetDescription(),
            Comment = createEquipmentInput.Comment,
            InvoiceNumber = createEquipmentInput.InvoiceNumber,
            EquipmentTypeId = createEquipmentInput.EquipmentTypeId,
            GuaranteeDate = createEquipmentInput.GuaranteeDate,
            Location = createEquipmentInput.Location.GetDescription(),
            Name = createEquipmentInput.Name,
            PurchaseDate = createEquipmentInput.PurchaseDate,
            PurchasePlace = createEquipmentInput.PurchasePlace,
            PurchasePrice = createEquipmentInput.PurchasePrice,
            PurchaseCurrency = createEquipmentInput.PurchaseCurrency.GetDescription(),
            PurchasePriceAndCurrency = $"{createEquipmentInput.PurchasePrice} {createEquipmentInput.PurchaseCurrency.GetDescription()}",
            PurchasePriceUsd = createEquipmentInput.PurchasePriceUsd,
            JiraTask = createEquipmentInput.JiraTask,
            Storage = LocationType.Vitebsk,
            Approver = new ApproverOutput
            {
                Id = createEquipmentInput.ApproverId,
                FullName = "CTO Test"
            },
            Type = new EquipmentTypeOutput(
                createEquipmentInput.EquipmentTypeId,
                createEquipmentInput.EquipmentTypeId,
                "PC accessory: HDD"
            )
        };

        // Act
        var response = await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);

        // Assert
        var body = await response.Content.ReadFromJsonAsync<EquipmentOutput>(
            _jsonOptions,
            _cancellationToken);

        body.Should().NotBeNull();
        body.Status.Should().BeEquivalentTo(EquipmentAvailabilityType.Available.GetDescription());
        body.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(expectedEquipmentOutput, config =>
                config.Excluding(x => x.Id)
                      .Excluding(x => x.Approver.FullName)
                      .Excluding(x => x.ActiveHolder!.FullName)
                      .Excluding(x => x.ActiveHolder!.DepartmentId)
                      .Excluding(x => x.Status)
                      .Excluding(x => x.InventoryNumber));
        body.Id.Should().BeGreaterThan(0);
        body.Approver.FullName.Should().NotBeEmpty();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location?.ToString().Should().EndWithEquivalentOf($"api/equipment/{body.Id}");
    }

    [Fact]
    public async Task GetEquipmentsWhenAuthedAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetPageableEquipmentAsResponseMessageAsync(0, 1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEquipmentsWhenRequestIsValidResponseWithExpectedEquipmentCountResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        for (var i = 0; i < 10; i++)
        {
            await client.CreateEquipmentAsResponseMessageAsync(createEquipmentInput);
        }

        // Act
        var response = await client.GetPageableEquipmentAsync(top: 8, skip:0);

        // Assert
        response.Should().NotBeNull();
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(8);
        response.Items.Should().HaveCount(8);
    }

    [Fact]
    public async Task GetEquipmentsWhenEquipmentContainsDeletedAssignmentResponseEquipmentWithoutActiveHolder()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var assign = new AssignEquipmentInput
        {
            UserId = _factory.EmployeeUserId!.Value,
            IssueDate = DateTime.UtcNow.Date.AddDays(5),
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentAssignAsync(createdEquipment!.Id, assign);

        await client.DeleteEquipmentHistoryAsync(createdEquipment.Id);

        // Act
        var response = await client.GetPageableEquipmentAsync(100, 0, createEquipmentInput.ApproverId, createdEquipment.EquipmentTypeId);

        // Assert
        response.Should().NotBeNull();
        response.Page.Should().Be(1);
        response.Items.First(equipment => equipment.Id == createdEquipment.Id).ActiveHolder.Should().BeNull();
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenAuthedAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetEquipmentByFileAsResponseMessageAsync(1, 100, 0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenAuthedAsEmployeeResponseBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetEquipmentByFileAsResponseMessageAsync(3);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenRequestIsValidAndFormatExcelResponseStreamWithExcelFile()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToAlphabetic(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.HeadOfDepartmentUserId!.Value,
            Characteristics = "3.5, SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            RepairPlace = "Test Repair place",
            JiraTask = "qwe"
        };

        var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var expectedTable = new List<string>
        {
            "Id",
            "Availability",
            "Invoice number",
            "Serial number",
            "Inventory number",
            "Name",
            "Type",
            "Price, $",
            "User",
            "Department",
            "Equipment characteristics",
            "Purchase place",
            "Approver",
            "Purchase date",
            "Storage",
            "Guarantee date",
            "Location",
            "Repair place",
            "Notes",
            createdEquipment!.Id.ToString(),
            "Available",
            createEquipmentInput.InvoiceNumber,
            createdEquipment.SerialNumber,
            createdEquipment.InventoryNumber ?? string.Empty,
            createdEquipment.Name,
            "PC accessory: HDD",
            createdEquipment.PurchasePriceUsd.ToString("0.00", CultureInfo.InvariantCulture),
            "",
            "",
            createEquipmentInput.Characteristics,
            "Minsk",
            "Test HeadOfDepartment",
            createdEquipment.PurchaseDate.ToString(_defaultDateFormat),
            createEquipmentInput.Storage.GetDescription(),
            createdEquipment.GuaranteeDate.ToString(_defaultDateFormat),
            "Belarus",
            "Test Repair place",
            createEquipmentInput.Comment
        };

        await using var response = await client.GetEquipmentFileStreamAsync(
            DocumentType.Excel,
            _factory.HeadOfDepartmentUserId!.Value,
            createEquipmentInput.SerialNumber);

        using var workbook = new XLWorkbook(response);
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();

        worksheet.CellsUsed()
            .Should()
            .HaveCount(expectedTable.Count(el => !string.IsNullOrEmpty(el)));

        worksheet
            .Cells("A1:S2")
            .Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture))
            .Should()
            .BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenAuthedAsHeadOfDepartmentAndRequestIsValidAndFormatExcelResponseStreamWithExcelFileWithoutPurchaseDetails()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.GetUserAsync((int)_factory.EmployeeUserId!);

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5, SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "Mouse",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var input = new AssignEquipmentInput
        {
            UserId = _factory.EmployeeUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentHistoryAsync(createdEquipment!.Id, input);

        var expectedTable = new List<string>
        {
            "Id",
            "Availability",
            "Invoice number",
            "Serial number",
            "Inventory number",
            "Name",
            "Type",
            "User",
            "Equipment characteristics",
            "Approver",
            "Purchase date",
            "Storage",
            "Guarantee date",
            "Location",
            "Repair place",
            "Notes",
            createdEquipment.Id.ToString(),
            "Issued",
            createEquipmentInput.InvoiceNumber,
            createdEquipment.SerialNumber,
            createdEquipment.InventoryNumber ?? string.Empty,
            createdEquipment.Name,
            createEquipmentInput.EquipmentTypeId,
            $"{employee!.FirstName} {employee.LastName}",
            createEquipmentInput.Characteristics,
            "Test CEO",
            createdEquipment.PurchaseDate.ToString(_defaultDateFormat),
            createEquipmentInput.Storage.GetDescription(),
            createdEquipment.GuaranteeDate.ToString(_defaultDateFormat),
            "Belarus",
            "",
            createEquipmentInput.Comment
        };

        await client.AuthAsHeadOfDepartment();

        await using var response = await client.GetEquipmentFileStreamAsync(
            DocumentType.Excel,
            _factory.CeoUserId!.Value,
            createEquipmentInput.SerialNumber,
            createdEquipment.EquipmentTypeId);

        using var workbook = new XLWorkbook(response);
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();

        worksheet
            .CellsUsed()
            .Should()
            .HaveCount(expectedTable.Count(el => !string.IsNullOrEmpty(el)));

        worksheet
            .Cells("A1:P2")
            .Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00")
                : cell.Value.ToString())
            .Should()
            .BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenAuthedAsHeadOfDepartmentAndRequestHasOrderingAndFormatExcelResponseStreamWithExcelFileWithoutPurchaseDetailsAndRightOrdering()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.GetUserAsync((int)_factory.EmployeeUserId!);

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = Guid.CreateVersion7().ToAlphabetic(),
            Comment = Guid.CreateVersion7().ToAlphabetic(),
            InvoiceNumber = Guid.CreateVersion7().ToAlphabetic(),
            EquipmentTypeId = "Mouse",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            RepairPlace = "Test Repair place",
            JiraTask = "qwe"
        };

        var firstCreatedEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var input = new AssignEquipmentInput
        {
            UserId = _factory.EmployeeUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentAssignAsResponseMessageAsync(input, firstCreatedEquipment!.Id);

        createEquipmentInput.SerialNumber = Guid.CreateVersion7().ToAlphabetic();
        createEquipmentInput.InvoiceNumber = Guid.CreateVersion7().ToAlphabetic();

        var secondCreatedEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.CreateEquipmentAssignAsResponseMessageAsync(input, secondCreatedEquipment!.Id);

        var expectedTable = new List<string>
        {
            "Id",
            "Availability",
            "Invoice number",
            "Serial number",
            "Inventory number",
            "Name",
            "Type",
            "User",
            "Equipment characteristics",
            "Approver",
            "Purchase date",
            "Storage",
            "Guarantee date",
            "Location",
            "Repair place",
            "Notes",
            secondCreatedEquipment.Id.ToString(),
            "Issued",
            createEquipmentInput.InvoiceNumber,
            secondCreatedEquipment.SerialNumber,
            secondCreatedEquipment.InventoryNumber ?? string.Empty,
            secondCreatedEquipment.Name,
            createEquipmentInput.EquipmentTypeId,
            $"{employee!.FirstName} {employee.LastName}",
            createEquipmentInput.Characteristics,
            "Test CEO",
            secondCreatedEquipment.PurchaseDate.ToString(_defaultDateFormat),
            createEquipmentInput.Storage.GetDescription(),
            secondCreatedEquipment.GuaranteeDate.ToString(_defaultDateFormat),
            "Belarus",
            createEquipmentInput.RepairPlace,
            createEquipmentInput.Comment,
            firstCreatedEquipment.Id.ToString(),
            "Issued",
            firstCreatedEquipment.InvoiceNumber,
            firstCreatedEquipment.SerialNumber,
            firstCreatedEquipment.InventoryNumber ?? string.Empty,
            firstCreatedEquipment.Name,
            firstCreatedEquipment.EquipmentTypeId,
            $"{employee.FirstName} {employee.LastName}",
            firstCreatedEquipment.Characteristics,
            "Test CEO",
            firstCreatedEquipment.PurchaseDate.ToString(_defaultDateFormat),
            createEquipmentInput.Storage.GetDescription(),
            firstCreatedEquipment.GuaranteeDate.ToString(_defaultDateFormat),
            "Belarus",
            createEquipmentInput.RepairPlace,
            firstCreatedEquipment.Comment
        };

        await client.AuthAsHeadOfDepartment();

        await using var response = await client.GetEquipmentFileStreamAsync(DocumentType.Excel, 2, 0);

        using var workbook = new XLWorkbook(response);
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();

        worksheet
            .Cells("A1:P3")
            .Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture))
            .Should()
            .BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenAuthedAsGroupManagerResponseStreamWithExcelFileWithoutPurchaseDetails()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var groupManager = await client.GetUserAsync((int)_factory.GroupManagerId!);
        var createdUser = await client.CreateEmployee(resourceGroupId: groupManager!.ResourceGroupId);

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = Guid.CreateVersion7().ToAlphabetic(),
            Comment = Guid.CreateVersion7().ToAlphabetic(),
            InvoiceNumber = Guid.CreateVersion7().ToAlphabetic(),
            EquipmentTypeId = "Mouse",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            RepairPlace = "Test Repair place",
            JiraTask = "qwe"
        };

        var firstCreatedEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var input = new AssignEquipmentInput
        {
            UserId = createdUser!.Id,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentHistoryAsync(firstCreatedEquipment!.Id, input);

        var expectedTable = new List<string>
        {
            "Id",
            "Availability",
            "Invoice number",
            "Serial number",
            "Inventory number",
            "Name",
            "Type",
            "User",
            "Equipment characteristics",
            "Approver",
            "Purchase date",
            "Storage",
            "Guarantee date",
            "Location",
            "Repair place",
            "Notes",
            firstCreatedEquipment.Id.ToString(),
            "Issued",
            createEquipmentInput.InvoiceNumber,
            firstCreatedEquipment.SerialNumber,
            firstCreatedEquipment.InventoryNumber ?? string.Empty,
            firstCreatedEquipment.Name,
            firstCreatedEquipment.EquipmentTypeId,
            $"{createdUser.FirstName} {createdUser.LastName}",
            createEquipmentInput.Characteristics,
            "Test CEO",
            firstCreatedEquipment.PurchaseDate.ToString(_defaultDateFormat),
            createEquipmentInput.Storage.GetDescription(),
            firstCreatedEquipment.GuaranteeDate.ToString(_defaultDateFormat),
            "Belarus",
            createEquipmentInput.RepairPlace,
            createEquipmentInput.Comment
        };

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        await using var response = await client.GetEquipmentFileStreamAsync(DocumentType.Excel, 5, 0);

        using var workbook = new XLWorkbook(response);
        workbook.TryGetWorksheet(
                _summaryReportWorksheet,
                out var worksheet)
            .Should().BeTrue();

        worksheet
            .Cells("A1:P2")
            .Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture))
            .Should()
            .BeEquivalentTo(
                expectedTable,
                config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetEquipmentWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var equipmentId = 1;

        // Act
        var response = await client.GetEquipmentAsResponseMessageAsync(equipmentId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEquipmentWhenEquipmentDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var equipmentId = 100000;

        // Act
        var response = await client.GetEquipmentAsResponseMessageAsync(equipmentId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEquipmentWhenAuthedAsHeadOfDepartmentAndEquipmentIsNotAssignedShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createEquipmentinput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentinput);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetEquipmentAsResponseMessageAsync(equipment.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEquipmentsWhenAuthedAsHeadOfDepartmentAndAssignUserShouldResponseWithCorrectData()
    {
        // ArrangeS
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        await client.CreateEmployee();

        using var scope = _factory.Services.CreateScope();

        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();

        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetPageableEquipmentAsync(createdEquipment!.Id, 10, 0);

        // Assert
        response!.Items.Should().NotBeNull();
        response.Items.All(a => a.Id == createdEquipment.Id).Should().BeTrue();
    }

    [Fact]
    public async Task GetEquipmentsByPostWhenAuthedAsHeadOfDepartmentAndAssignUserShouldResponseWithCorrectData()
    {
        // ArrangeS
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        await client.CreateEmployee();

        using var scope = _factory.Services.CreateScope();

        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();

        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetPageableEquipmentAsync(createdEquipment!.Id, 10, 0);

        // Assert
        response!.Items.Should().NotBeNull();
        response.Items.All(a => a.Id == createdEquipment.Id).Should().BeTrue();
    }

    [Fact]
    public async Task GetEquipmentsWhenAuthedAsHeadOfDepartmentAndAssignUserWasUnAssignedShouldResponseWithCorrectData()
    {
        // ArrangeS
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        await client.CreateEmployee();

        using var scope = _factory.Services.CreateScope();

        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();

        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qew"
        };

        var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.DeleteEquipmentHistoryAsync(createdEquipment!.Id);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetPageableEquipmentAsync(1000, 0);

        // Assert
        response!.Items.Should().NotBeNull();
        response.Items.Any(a => a.Id == createdEquipment.Id).Should().BeFalse();
    }

    [Fact]
    public async Task GetEquipmentWhenAuthedAsHeadOfDepartmentAndEquipmentIsAssignedToCtoShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createdEquipmentInput);

        var assignInput = new AssignEquipmentInput
        {
            UserId = _factory.CtoUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentHistoryAsync(equipment!.Id, assignInput);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetEquipmentAsResponseMessageAsync(equipment.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEquipmentWhenAuthedAsHeadOfDepartmentAndEquipmentIsAssignedToUserOfOtherDepartmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var user = await client.CreateEmployee();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var assignInput = new AssignEquipmentInput
        {
            UserId = user!.Id,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentHistoryAsync(equipment!.Id, assignInput);

        await client.AuthAsSysAdminHead();

        // Act
        var response = await client.GetEquipmentAsResponseMessageAsync(equipment.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetEquipmentWhenAuthedAsGroupManagerAndEquipmentIsAssignedToUserFromSameResourceGroupShouldResponseWithCorrectData()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var groupManager = await client.GetUserAsync((int)_factory.GroupManagerId!);

        var createdUser = await client.CreateEmployee(resourceGroupId: groupManager!.ResourceGroupId);

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var equipmentInput = new AssignEquipmentInput
        {
            UserId = createdUser!.Id,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        await client.CreateEquipmentHistoryAsync(equipment!.Id, equipmentInput);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var response = await client.GetEquipmentAsync(equipment.Id);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(equipment.Id);
        response.PurchasePlace.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetEquipmentWhenRequestIsValidShouldResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            InvoiceNumber = "DSAMVAS123ASKD123",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        // Act
        var response = await client.GetEquipmentAsResponseMessageAsync(equipment!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<EquipmentOutput>(
            _jsonOptions,
            _cancellationToken);
        body.Should().BeEquivalentTo(equipment);
    }
}
