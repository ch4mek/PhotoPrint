using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.SysAdmin;

[Collection("FuncScenarioCollection")]
public class SysAdminAssignEquipmentToUserScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public SysAdminAssignEquipmentToUserScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task AssignEquipmentToUserScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);
        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[System Administrator]",
            title: "System Administrator Assign equipment to user scenario");

        await httpClient.AuthAsCeo();
        var allEquipment = await httpClient.GetPageableEquipmentAsync(skip: 0,
            top: 20);

        if (allEquipment!.Items.All(equipment => equipment.ActiveHolder is null))
        {
            await httpClient.CreateEquipmentAssignAsync(id: allEquipment.Items.First().Id,
                new AssignEquipmentInput
                {
                    UserId = 2,
                    IssueDate = DateTime.UtcNow.Date.AddDays(-2),
                    ApproverId = _factory.CeoUserId!.Value
                });
        }

        await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsAdmin());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.SystemAdministrator);
            });
        });

        await httpClient.CreateEquipmentAsync(
            new CreateEquipmentRequestDto
            {
                SerialNumber = Guid.CreateVersion7().ToString(),
                BrokenStatus = EquipmentBrokenStatus.None,
                ApproverId = 2,
                Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
                Comment = "=== TEST DATA ===",
                InvoiceNumber = "OEMPAM314",
                EquipmentTypeId = "PcAccessoriesHdd",
                GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Location = EquipmentLocationType.Belarus,
                Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
                PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                PurchaseCurrency = MoneyCurrencyType.Byn,
                PurchasePlace = "Minsk",
                PurchasePrice = 134,
                PurchasePriceUsd = 44.5m,
                Storage = LocationType.Vitebsk,
                JiraTask = "qwe"
            });

        var equipment = await scenario.Step("Get unassigned equipment", async () =>
        {
            var availableEquipment = await scenario.Step("Get all available equipment", async () =>
            {
                return await httpClient.GetPageableEquipmentWithFilterAsync(skip: 0,
                    top: 20,
                    filter: "ActiveHolderId eq null");
            });

            return await scenario.Step("Pick first equipment", () =>
            {
                return availableEquipment!.Items
                    .First(x => x.ActiveHolder is null);
            });
        });

        var employee = await scenario.Step("Get an employee", async () =>
        {
            var availableEmployees = await scenario.Step("Get all available employees", async () =>
            {
                return await httpClient.GetUsersLookup(roles: [PortalRoleNames.Employee], isActive: true);
            });

            return await scenario.Step(
                "Pick last Employee",
                () => availableEmployees[^1]);
        });

        await scenario.Step("Assign equipment to user", async () =>
        {
            var response = await scenario.Step("Call POST endpoint for assigning equipment", async () =>
            {
                return await httpClient.CreateEquipmentHistoryAsync(
                    equipment.Id,
                    new AssignEquipmentInput
                    {
                        UserId = employee.Id,
                        IssueDate = DateTime.UtcNow.Date,
                        ApproverId = _factory.CeoUserId!.Value
                    });
            });

            await scenario.Step("Verify response", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var body = await response.Content.ReadFromJsonAsync<EquipmentHolderOutput>(
                    _serializerOptions,
                    _cancellationToken);

                body!.ReturnDate.Should().BeNull();
                body.FullName.Should().Be($"{employee.LastName} {employee.FirstName}");
            });
        });

        await scenario.Step("Verify that the equipment now contains a holder", async () =>
        {
            var equipmentWithHolder = await scenario.Step("Get updated equipment", async () =>
            {
                return await httpClient.GetEquipmentAsync(equipment.Id);
            });

            await scenario.Step("Verify response", () =>
            {
                equipmentWithHolder.Should().NotBeNull();
                equipmentWithHolder.ActiveHolder.Should().NotBeNull();
                equipmentWithHolder.ActiveHolder!.FullName.Should().Be($"{employee.LastName} {employee.FirstName}");
                equipmentWithHolder.ActiveHolder.ReturnDate.Should().BeNull();
            });
        });
    }
}
