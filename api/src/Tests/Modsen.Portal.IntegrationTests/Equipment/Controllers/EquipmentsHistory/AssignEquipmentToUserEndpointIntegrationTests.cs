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
public class AssignEquipmentToUserEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken;
    private readonly Faker<CreateUserInput> _createUserInputFaker = new();

    public AssignEquipmentToUserEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _cancellationToken = CancellationToken.None;
        _factory = factory;
        _jsonOptions = factory.JsonOptions;

        _createUserInputFaker = _createUserInputFaker
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
                DepartmentId = "Net",
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
            });
    }

    [Fact]
    public async Task AssignUserToEquipmentWhenAuthedAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

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
            InvoiceNumber = "ADSAADA",
            PurchasePrice = 134,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.AuthAs(createResponse!.Email);

        // Act
        var response = await client.CreateEquipmentHistoryAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = _factory.HeadOfDepartmentUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AssignUserToEquipmentWhenEquipmentDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateEquipmentHistoryAsync(10000000, new AssignEquipmentInput
        {
            UserId = _factory.HeadOfDepartmentUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignUserToEquipmentWhenAssigneeDoesNotExistResponseWithNotFoundResult()
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

        // Act
        var response = await client.CreateEquipmentHistoryAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = int.MaxValue,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignUserToEquipmentWhenEquipmentAlreadyAssignedResponseWithConflictResult()
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

        await client.CreateEquipmentHistoryAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = _factory.HeadOfDepartmentUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Act
        var response = await client.CreateEquipmentHistoryAsync(equipment.Id, new AssignEquipmentInput
        {
            UserId = _factory.CtoUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [MemberData(nameof(GetInvalidAssignUserToEquipmentParams))]
    public async Task AssignUserToEquipmentWhenRequestParamsIsInvalidResponseWithBadRequestResult(
        int equipmentId,
        DateTime issueDate)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateEquipmentHistoryAsync(equipmentId, new AssignEquipmentInput
        {
            UserId = _factory.HeadOfDepartmentUserId!.Value,
            IssueDate = issueDate,
            ApproverId = _factory.CeoUserId!.Value
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AssignUserToEquipmentWhenRequestIsValidResponseWithOkResult()
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

        var input = new AssignEquipmentInput
        {
            UserId = _factory.HeadOfDepartmentUserId!.Value,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = _factory.CeoUserId!.Value
        };

        // Act
        var response = await client.CreateEquipmentHistoryAsync(equipment!.Id, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<EquipmentHolderOutput>(cancellationToken: _cancellationToken);
        body.Should().NotBeNull();
        body.Id.Should().Be(input.UserId);
        body.FullName.Should().NotBeEmpty();
        body.DepartmentId.Should().NotBeEmpty();
        body.ReturnDate.Should().BeNull();
    }

    public static IEnumerable<object[]> GetInvalidAssignUserToEquipmentParams()
    {
        yield return
        [
            0,
            new DateTime(2023, 7, 22, 0, 0, 0, DateTimeKind.Utc)
        ];
        yield return
        [
            1,
            new DateTime(2023, 7, 22, 0, 0, 1, DateTimeKind.Utc)
        ];
        yield return
        [
            0,
            new DateTime(2023, 7, 22, 11, 56, 12, DateTimeKind.Utc)
        ];
    }
}
