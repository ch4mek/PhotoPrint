using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.SysAdmin;

[Collection("FuncScenarioCollection")]
public class SysAdminCreateEquipmentScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public SysAdminCreateEquipmentScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateEquipmentScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[System Administrator]",
            title: "System Administrator Create Equipment scenario");

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

        await scenario.Step("Create equipment", async () =>
        {
            var createdEquipment = await scenario.Step("Call Equipment POST endpoint", async () =>
            {
                var createUserInput = new CreateEquipmentRequestDto
                {
                    SerialNumber = Guid.CreateVersion7().ToString(),
                    BrokenStatus = EquipmentBrokenStatus.None,
                    ApproverId = _factory.CeoUserId!.Value,
                    Characteristics =
                        "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
                    Comment = "=== TEST DATA ===",
                    InvoiceNumber = "plvsdo1234555",
                    EquipmentTypeId = "PcAccessoriesHdd",
                    GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Location = EquipmentLocationType.Belarus,
                    Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
                    PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PurchasePlace = "Minsk",
                    PurchaseCurrency = MoneyCurrencyType.Byn,
                    PurchasePrice = 134,
                    PurchasePriceUsd = 44.5m,
                    Storage = LocationType.Vitebsk,
                    JiraTask = "qwe"
                };

                return await httpClient.CreateEquipmentAsync(createUserInput);
            });

            var equipment = await scenario.Step("Get created equipment by id", async () =>
            {
                return await httpClient.GetEquipmentAsync(createdEquipment!.Id);
            });

            await scenario.Step("Verify if equipment was created", () =>
            {
                equipment.Should().NotBeNull();
                equipment.Should().BeEquivalentTo(createdEquipment);
            });
        });
    }

}
