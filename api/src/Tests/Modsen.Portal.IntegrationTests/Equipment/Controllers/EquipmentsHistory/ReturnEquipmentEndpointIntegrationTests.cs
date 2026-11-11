using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Equipment.Controllers.EquipmentsHistory;


[Collection("EquipmentCollection")]
public class ReturnEquipmentEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken;
    private readonly Faker<CreateUserInput> _createUserInputFaker;

    public ReturnEquipmentEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _cancellationToken = CancellationToken.None;
        _factory = factory;
        _jsonOptions = factory.JsonOptions;

        _createUserInputFaker = new Faker<CreateUserInput>()
            .CustomInstantiator(x => new CreateUserInput
            {
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = Guid.CreateVersion7().ToAlphabetic(),
                LastNameRu = Guid.CreateVersion7().ToAlphabetic(),
                PatronymicRu = Guid.CreateVersion7().ToAlphabetic(),
                Color = x.Internet.Color(),
                Email = x.Internet.Email(provider: "modsen-software.com", uniqueSuffix: Guid.CreateVersion7().ToAlphabetic()),
                Position = PortalRoleNames.Employee,
                Location = x.PickRandom<LocationType>(),
                DepartmentId = x.PickRandom("Net"),
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
            });
    }

    [Fact]
    public async Task ReturnEquipmentWhenRoleIsAdminReturnOkResultWithUpdatedEquipmentHolder()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var userId = _factory.HeadOfDepartmentUserId!.Value;

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

        var assignmentBody = await client.CreateEquipmentHistoryAsOutputAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow.Date.AddDays(-10),
            ApproverId = _factory.AdminUserId!.Value
        });

        var assignmentsBodyBeforeReturnDate = await client.GetEquipmentHistoryAsync(
            equipment.Id,
            20,
            0,
            "isDeleted eq false");

        var returnDate = assignmentBody!.IssueDate.AddDays(3);

        // Act
        var updatedAssigmentResponse = await client.CreateEquipmentHistoryWithReturnAsync(equipment.Id, new ReturnEquipmentInput { ReturnDate = returnDate });

        var updatedAssigmentBody = await updatedAssigmentResponse
            .Content
            .ReadFromJsonAsync<EquipmentHolderOutput>(cancellationToken: _cancellationToken);

        var assignmentsBody = await client.GetEquipmentHistoryAsync(
            equipment.Id,
            20,
            0,
            "isDeleted eq false");

        // Assert
        updatedAssigmentResponse.StatusCode
            .Should().Be(HttpStatusCode.OK);
        updatedAssigmentBody!.ReturnDate
            .Should().Be(assignmentBody.IssueDate.AddDays(3));
        assignmentsBody!.Items.Should().HaveCount(1);
        assignmentsBody.Items.First(x => x.Id == updatedAssigmentBody.Id).IssueDate.Date
            .Should().Be(assignmentBody.IssueDate.Date);
        assignmentsBody.Items.First(x => x.Id == updatedAssigmentBody.Id).ReturnDate!.Value.Date
            .Should().Be(returnDate.Date);
        assignmentsBody.Items.Should().HaveCount(assignmentsBodyBeforeReturnDate!.Items.Count);
        assignmentsBody.Items.First(x => x.Id == updatedAssigmentBody.Id).ReturnDate
            .Should().NotBe(assignmentsBodyBeforeReturnDate.Items.First(x => x.Id == updatedAssigmentBody.Id).ReturnDate);
    }

    [Fact]
    public async Task ReturnEquipmentWhenTwoIssueDatesAreSameAndReturnDatesAreDifferentReturnOkResultWithAppropriateSorting()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var userId = _factory.HeadOfDepartmentUserId!.Value;

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

        var equipmentBody = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.CreateEquipmentHistoryAsync(equipmentBody!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.AdminUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipmentBody.Id, new ReturnEquipmentInput
        {
            ReturnDate = DateTime.UtcNow.Date
        });

        await client.CreateEquipmentHistoryAsync(equipmentBody.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.AdminUserId!.Value
        });

        var returnDate = DateTime.UtcNow.Date.AddDays(3);

        await client.CreateEquipmentHistoryWithReturnAsync(equipmentBody.Id, new ReturnEquipmentInput
        {
            ReturnDate = returnDate
        });

        // Act
        var assignmentsBody = await client.GetEquipmentHistoryAsync(
            equipmentBody.Id,
            20,
            0,
            "isDeleted eq false&orderby=issueDate desc,returnDate desc,equipmentHistoryId desc");

        // Assert
        assignmentsBody!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task ReturnEquipmentWhenThreeIssueDatesAreSameAndReturnDatesTooReturnOkResultWithAppropriateSorting()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var headId = _factory.HeadOfDepartmentUserId!.Value;
        var employeeId = _factory.EmployeeUserId!.Value;
        var ceoId = _factory.CeoUserId!.Value;

        var createEquipmentInput = new CreateEquipmentRequestDto
        {
            SerialNumber = Guid.CreateVersion7().ToString(),
            ApproverId = _factory.CeoUserId!.Value,
            BrokenStatus = EquipmentBrokenStatus.None,
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

        var equipmentBody = await client.CreateEquipmentAsync(createEquipmentInput);

        var headAssignBody = await client.CreateEquipmentHistoryAsOutputAsync(equipmentBody!.Id, new AssignEquipmentInput
        {
            UserId = headId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.AdminUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipmentBody.Id, new ReturnEquipmentInput
        {
            ReturnDate = DateTime.UtcNow.Date
        });

        await client.CreateEquipmentHistoryAsync(equipmentBody.Id, new AssignEquipmentInput
        {
            UserId = employeeId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.AdminUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipmentBody.Id, new ReturnEquipmentInput
        {
            ReturnDate = DateTime.UtcNow.Date
        });

        var ceoAssignBody = await client.CreateEquipmentHistoryAsOutputAsync(equipmentBody.Id, new AssignEquipmentInput
        {
            UserId = ceoId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.AdminUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipmentBody.Id, new ReturnEquipmentInput
        {
            ReturnDate = DateTime.UtcNow.Date
        });

        // Act
        var assignmentsBody = await client.GetEquipmentHistoryAsync(
            equipmentBody.Id,
            20,
            0,
            "isDeleted eq false&orderby=issueDate desc,returnDate desc,equipmentHistoryId desc");

        // Assert
        assignmentsBody!.Items.Should().HaveCount(3);
        assignmentsBody.Items.Last().Id.Should().Be(headAssignBody.Id);
        assignmentsBody.Items.First().Id.Should().Be(ceoAssignBody.Id);
    }

    [Fact]
    public async Task ReturnEquipmentWhenIssueAndReturnDatesAreSameMoreThanOneTimeInRowReturnOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = (int)_factory.EmployeeUserId;

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

        var equipmentBody = await client.CreateEquipmentAsync(createEquipmentInput);

        var assignmentBody = await client.CreateEquipmentHistoryAsOutputAsync(equipmentBody!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipmentBody.Id, new ReturnEquipmentInput { ReturnDate = assignmentBody.IssueDate });

        await client.CreateEquipmentHistoryAsync(equipmentBody.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = assignmentBody.IssueDate,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Act
        var assignmentResponse = await client.PostAsJsonAsync(
            $"api/EquipmentHistory/{equipmentBody.Id}/return",
            new ReturnEquipmentInput { ReturnDate = assignmentBody.IssueDate },
            _cancellationToken);

        // Assert
        assignmentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReturnEquipmentWhenDatesAreTheSameReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.HeadOfDepartmentUserId!.Value;

        _createUserInputFaker.Generate();

        var createResponse = await client.CreateEmployee();

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

        var equipmentBody = await client.CreateEquipmentAsync(createEquipmentInput);

        var assignmentBody = await client.CreateEquipmentHistoryAsOutputAsync(equipmentBody!.Id, new AssignEquipmentInput
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Act
        await client.AuthAs(createResponse.Email);

        var updatedAssigmentResponse = await client.CreateEquipmentHistoryWithReturnAsync(
            equipmentBody.Id,
            new ReturnEquipmentInput { ReturnDate = assignmentBody!.IssueDate.AddDays(3) });

        // Assert
        updatedAssigmentResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ReturnEquipmentAfterDeletingPreviousAssignReturnSuccessResultAnd()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var presentDateTime = DateTime.UtcNow.Date;
        var futureDateTime = presentDateTime.AddYears(1);
        var pastDateTime = presentDateTime.AddYears(-1);

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
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        var headUserId = _factory.HeadOfDepartmentUserId!.Value;
        var ceoUserId = _factory.CeoUserId!.Value;

        var returnEquipmentInput = new ReturnEquipmentInput
        {
            ReturnDate = presentDateTime
        };

        var assign = await client.CreateEquipmentHistoryAsOutputAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = headUserId,
            IssueDate = presentDateTime,
            ApproverId = _factory.CeoUserId!.Value
        });

        await client.CreateEquipmentHistoryWithReturnAsync(equipment.Id, returnEquipmentInput);

        await client.DeleteEquipmentHistoryByIdAsync(equipment.Id, assign!.EquipmentHistoryId);

        await client.CreateEquipmentHistoryAsync(equipment.Id, new AssignEquipmentInput
        {
            UserId = ceoUserId,
            IssueDate = presentDateTime,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Act
        var equipmentHolderOutput = await client.CreateEquipmentHistoryWithAsOutputReturnAsync(equipment.Id, returnEquipmentInput);

        // Assert
        equipmentHolderOutput.Should().NotBeNull();
        equipmentHolderOutput.Id.Should().Be(ceoUserId);
    }
}
