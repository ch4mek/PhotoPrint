using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios.SysAdmin;

public class SysAdminReturnEquipmentScenario : IClassFixture<FunctionalTestFactory>
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public SysAdminReturnEquipmentScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task ReturnEquipmentScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);
        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[System Administrator]",
            title: "System Administrator Return equipment scenario");

        await httpClient.AuthAsCeo();
        var allEquipment = await httpClient.GetPageableEquipmentAsync(skip: 0, top: 20);

        if (allEquipment!.Items.All(equipment => equipment.ActiveHolder is null))
        {
            await httpClient.CreateEquipmentAssignAsync(
                allEquipment.Items.First().Id,
                new AssignEquipmentInput
                {
                    UserId = 2,
                    IssueDate = DateTime.UtcNow.Date.AddDays(-2),
                    ApproverId = _factory.CeoUserId!.Value
                });
        }

        var returnDate = DateTime.UtcNow.Date.AddDays(-1);

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

        var equipment = await scenario.Step("Get assigned equipment", async () =>
        {
            var availableEquipment = await scenario.Step("Get all available equipment", async () =>
            {
                return await httpClient.GetPageableEquipmentAsync(skip: 0, top: 20);
            });

            return await scenario.Step("Pick first assigned without return date equipment ", () =>
            {
                return availableEquipment!.Items
                    .First(x => x.ActiveHolder is not null && x.ActiveHolder.ReturnDate is null);
            });
        });

        await scenario.Step("Return equipment", async () =>
        {
            var response = await scenario.Step("Call POST endpoint for returning equipment", async () =>
            {
                return await httpClient.CreateEquipmentHistoryWithReturnAsync(equipment.Id,
                    new ReturnEquipmentInput { ReturnDate = returnDate });
            });

            await scenario.Step("Verify response", async () =>
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var body = await response.Content.ReadFromJsonAsync<EquipmentHolderOutput>(
                    _serializerOptions,
                    _cancellationToken);

                body.Should().NotBeNull();
                body.ReturnDate.Should().Be(returnDate);
                body.IssueDate.Should().Be(equipment.ActiveHolder!.IssueDate);
            });
        });

        await scenario.Step("Verify that the equipment now doesn't contain a holder", async () =>
        {
            var equipmentWithoutHolder = await scenario.Step("Get updated equipment", async () =>
            {
                return await httpClient.GetEquipmentAsync(equipment.Id);
            });

            await scenario.Step("Verify response", () =>
            {
                equipmentWithoutHolder.Should().NotBeNull();
                equipmentWithoutHolder.ActiveHolder.Should().BeNull();
            });
        });
    }
}
