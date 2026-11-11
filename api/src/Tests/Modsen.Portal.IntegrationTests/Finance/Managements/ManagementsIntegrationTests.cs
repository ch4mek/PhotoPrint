using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Finance;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;

namespace Modsen.Portal.IntegrationTests.Finance.Managements;

[Collection("GeneralCollection")]
public class ManagementsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly FinanceTestDataGenerator _testDataGenerator;

    public ManagementsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;

        _testDataGenerator = new FinanceTestDataGenerator(factory);
    }

    [Fact]
    public async Task GetManagementsWhenRequestValidResponseWithPageableManagementsDto()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2026, 7, 1);

        (var user, var project) = await _testDataGenerator.GetUserAndProjectAndCreateProjectAssignmentAsync(client, date);

        ExecuteMigrationTasks();

        await CreateRateAndTrackFact(client, user!.Id, project!.Id, date, "GetManagementsWhenRequestValidResponseWithPageableManagementsDto");

        // Act
        var response = await client.GetManagementsAsResponseMessageAsync(2026, 7, new GetManagementsInput());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var managements = await response.Content.ReadFromJsonAsync<PageableManagementsOutput>(_jsonOptions, _cancellationToken);
        managements.Should().NotBeNull();

        var managementUser = managements!.Items.Should().ContainSingle(x => x.Id == user.Id).Which;
        managementUser.Projects.Should().ContainSingle(x => x.ProjectId == project.Id);
    }

    [Fact]
    public async Task GetManagementsWhenFilterByUserIdResponseWithFilteredManagements()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var date = new DateOnly(2026, 7, 1);

        (var user, var project) = await _testDataGenerator.GetUserAndProjectAndCreateProjectAssignmentAsync(client, date);
        (var user2, var project2) = await _testDataGenerator.GetUserAndProjectAndCreateProjectAssignmentAsync(client, date);

        ExecuteMigrationTasks();

        await CreateRateAndTrackFact(client, user!.Id, project!.Id, date, "GetManagementsWhenFilterByUserIdResponseWithFilteredManagements 1");
        await CreateRateAndTrackFact(client, user2!.Id, project2!.Id, date, "GetManagementsWhenFilterByUserIdResponseWithFilteredManagements 2");

        // Act
        var response = await client.GetManagementsAsResponseMessageAsync(2026, 7, new GetManagementsInput { UserFullName = user.FullName, ShouldCalculateMainStatistics = false });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var managements = await response.Content.ReadFromJsonAsync<PageableManagementsOutput>(_jsonOptions, _cancellationToken);
        managements.Should().NotBeNull();

        managements!.Items.Should().NotContain(x => x.Id == user2.Id);
        var managementUser = managements!.Items.Should().ContainSingle(x => x.Id == user.Id).Which;
        managementUser.Projects.Should().ContainSingle(x => x.ProjectId == project.Id);
    }

    private void ExecuteMigrationTasks()
    {
        using var scope = _factory.Services.CreateScope();
        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();
        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }
    }

    private static async Task CreateRateAndTrackFact(HttpClient client, int userId, int projectId, DateOnly date, string taskName)
    {
        var rate = await client.CreateProjectRateAsync(userId, projectId, 1, CurrencyCodes.USDollar, date);
        var input = FinanceTestDataGenerator.GetUpsertFactDto(taskName, date, projectId, userId);
        var tracked = await client.TrackFactAsync(input);
    }
}
