using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Equipment.Controllers.EquipmentsHistory;

[Collection("EquipmentCollection")]
public class RemoveHistoryEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken;

    public RemoveHistoryEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _cancellationToken = CancellationToken.None;
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task RemoveHistoryEquipmentWhenRequestIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var presentDateTime = DateTime.UtcNow.Date;
        var futureDateTime = presentDateTime.AddMonths(1);
        var pastDateTime = presentDateTime.AddYears(-1);

        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = futureDateTime,
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = pastDateTime,
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            InvoiceNumber = "ADSAADA",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Minsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var assign= await client.CreateEquipmentHistoryAsOutputAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = presentDateTime,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Act
        var response = await client.DeleteEquipmentHistoryByIdAsync(equipment.Id, assign!.EquipmentHistoryId);

        var equipmentAfterDeletion = await client.GetEquipmentAsync(equipment.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<EquipmentHolderOutput>(cancellationToken: _cancellationToken);

        body.Should().NotBeNull();
        body.FullName.Should().NotBeEmpty();
        body.DepartmentId.Should().NotBeEmpty();
        body.ReturnDate.Should().BeNull();

        equipmentAfterDeletion.Should().NotBeNull();
        equipmentAfterDeletion.ActiveHolder.Should().BeNull();
        equipmentAfterDeletion.Approver.Id.Should().Be(createEquipmentInput.ApproverId);
    }

    [Fact]
    public async Task RemoveHistoryEquipmentWhenReturnDateIsNotNullResponseBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var presentDateTime = DateTime.UtcNow.Date;
        var futureDateTime = presentDateTime.AddMonths(1);
        var pastDateTime = presentDateTime.AddYears(-1);

        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = futureDateTime,
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = pastDateTime,
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            InvoiceNumber = "ADSAADA",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Minsk,
            JiraTask = "qwe"
        };

        var returnEquipmentInput = new ReturnEquipmentInput
        {
            ReturnDate = presentDateTime
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var assign = await client.CreateEquipmentHistoryAsOutputAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = presentDateTime,
            ApproverId = _factory.CeoUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipment.Id, returnEquipmentInput);

        // Act
        var response = await client.DeleteEquipmentHistoryByIdAsync(equipment.Id, assign!.EquipmentHistoryId);

        await client.GetEquipmentAsResponseMessageAsync(equipment.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveHistoryEquipmentWhenStatusIsNotIssuedResponseBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var presentDateTime = DateTime.UtcNow.Date;
        var futureDateTime = presentDateTime.AddMonths(1);
        var pastDateTime = presentDateTime.AddYears(-1);

        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.Broken,
            ApproverId = _factory.CeoUserId!.Value,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = futureDateTime,
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = pastDateTime,
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            InvoiceNumber = "ADSAADA",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Minsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var assign = await client.CreateEquipmentHistoryAsOutputAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = presentDateTime,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Act

        var response = await client.DeleteEquipmentHistoryByIdAsync(equipment.Id, assign!.EquipmentHistoryId);

        await client.GetEquipmentAsResponseMessageAsync(equipment.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveHistoryEquipmentWhenAuthAsHeadResponseForbidden()
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
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            InvoiceNumber = "ADSAADA",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var assign = await client.CreateEquipmentHistoryAsOutputAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.DeleteEquipmentHistoryByIdAsync(equipment.Id, assign!.EquipmentHistoryId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RemoveHistoryEquipmentWhenAssignDoesNotExistResponseBadRequest()
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
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchasePrice = 134,
            InvoiceNumber = "ADSAADA",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var equipmentHistoryId = 99999;

        // Act
        var response = await client.DeleteEquipmentHistoryByIdAsync(equipment!.Id, equipmentHistoryId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
