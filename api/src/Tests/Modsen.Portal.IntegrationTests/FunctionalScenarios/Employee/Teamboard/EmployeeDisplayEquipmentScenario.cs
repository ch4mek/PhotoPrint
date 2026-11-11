using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.Teamboard;

[Collection("FuncScenarioCollection")]
public class EmployeeDisplayEquipmentScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeDisplayEquipmentScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task DisplayEquipmentScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Display user equipment scenario");

        var ceoUser = await scenario.Step("Login as CEO", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
            });

            var user = await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
            });

            return user;
        });

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            return await scenario.Step("Create new user", async () =>
            {
                return await httpClient.CreateEmployee(departmentId: "PMO");
            });
        });

        const string equipmentName = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)";

        await scenario.Step("Create new equipment assign", async () =>
        {
            var createdEquipment = await scenario.Step("Create equipment", async () =>
            {
                var createEquipmentInput = new CreateEquipmentRequestDto
                {
                    SerialNumber = Guid.CreateVersion7().ToString(),
                    BrokenStatus = EquipmentBrokenStatus.None,
                    ApproverId = ceoUser.Id,
                    Characteristics = Guid.CreateVersion7().ToAlphabetic(),
                    Comment = Guid.CreateVersion7().ToAlphabetic(),
                    InvoiceNumber = Guid.CreateVersion7().ToAlphabetic(),
                    EquipmentTypeId = "Mouse",
                    GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Location = EquipmentLocationType.Belarus,
                    Name = equipmentName,
                    PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    PurchasePlace = "Minsk",
                    PurchasePrice = 134,
                    PurchaseCurrency = MoneyCurrencyType.Byn,
                    PurchasePriceUsd = 44.5m,
                    Storage = LocationType.Vitebsk,
                    RepairPlace = "Test Repair place",
                    JiraTask = "qwe"
                };

                return await httpClient.CreateEquipmentAsync(createEquipmentInput);
            });

            return await scenario.Step("Create equipment assign history", async () =>
            {
                var input = new AssignEquipmentInput
                {
                    UserId = createdUser!.Id,
                    IssueDate = DateTime.UtcNow.Date,
                    ApproverId = ceoUser.Id
                };

                return await httpClient.CreateEquipmentHistoryAsync(createdEquipment!.Id, input);

            });
        });

        await scenario.Step("Display user equipment", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(createdUser!.Email);
            });

            var equipments = await scenario.Step("Get user equipment", async () =>
            {
                return await httpClient.GetUserEquipmentAsync(createdUser!.Id, "top=100&skip=0");
            });

            await scenario.Step("Verify user equipment", () =>
            {
                equipments!.Items.First().EquipmentName.Should().Be(equipmentName);
            });
        });
    }
}
