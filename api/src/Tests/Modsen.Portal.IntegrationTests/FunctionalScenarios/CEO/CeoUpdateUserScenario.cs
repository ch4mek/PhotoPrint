using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoUpdateUserScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoUpdateUserScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task UpdateUserScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO update User scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        var createdUser = await scenario.Step("Create User", async () =>
        {
            var userFirstName = "CreateUserTestName";

            await scenario.Step("Get all users", async () =>
            {
                return await httpClient.GetUsersWithSearch(orderBy: "id desc");
            });

            var createdUser = await scenario.Step("Call User POST endpoint", async () =>
            {
                return await httpClient.CreateEmployee(firstName: userFirstName);
            });

            await scenario.Step("Get all users", async () =>
            {
                return await httpClient.GetUsersWithSearch(orderBy: "id desc");
            });

            return createdUser;
        });

        await scenario.Step("Update user", async () =>
        {
            var users = await scenario.Step("Get users with created user first name", async () =>
            {
                return await httpClient.GetUsersWithSearch(
                    filter: $"contains(firstName,'{createdUser!.FirstName}')",
                    orderBy: "id desc");
            });

            var userForUpdateId = users.Items.First(user => user.Id == createdUser!.Id).Id;

            var userForUpdate = await scenario.Step("Get user info", async () =>
            {
                return await httpClient.GetUserAsync(userForUpdateId);
            });

            await scenario.Step("Get projects lookup", async () =>
            {
                await httpClient.GetProjectsLookupAsync();
            });

            var updatedUser = await scenario.Step("Update User", async () =>
            {
                var updateUserInput = new UpdateUserInput
                {
                    Id = userForUpdateId,
                    FirstName = null,
                    LastName = null,
                    Position = PortalRoleNames.Employee,
                    Email = null,
                    Location = userForUpdate!.Location!.Value,
                    DepartmentId = userForUpdate.DepartmentId,
                    CountryId = 1,
                    RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
                };

                return await httpClient.UpdateUserAsync(updateUserInput);
            });

            var usersWithFilters = await scenario.Step("Get users with updated user name", async () =>
            {
                return await httpClient.GetUsersWithSearch(
                    filter: $"contains(firstName,'{updatedUser.FirstName}')",
                    orderBy: "id desc");
            });

            await scenario.Step("Verify if user was successfully updated", () =>
            {
                updatedUser.Should().NotBeNull();
                updatedUser.FirstName.Should().BeEquivalentTo("CreateUserTestName");
                updatedUser.Position.Should().Be(PortalRoleNames.Employee);
                updatedUser.Location.Should().Be(userForUpdate!.Location);
                updatedUser.DepartmentId.Should().Be(userForUpdate.DepartmentId);
                usersWithFilters.Items.Should().NotBeEmpty();
                usersWithFilters.Items.Should().Contain(user => user.Id == createdUser!.Id);
            });
        });
    }

}
