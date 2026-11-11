using System.Net;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

[Collection("JiraControllerCollection")]
public sealed class SearchJiraSettingsIntegrationTests : JiraControllerIntegrationTestsBase
{
    public SearchJiraSettingsIntegrationTests(JiraIntegrationTestFactory factory)
        : base(factory) { }

    [Fact]
    public async Task SearchAsyncWhenRequestIsValidShouldRespondWithJiraSettings()
    {
        // Arrange
        var client = Factory.CreateClient();

        await client.AuthAsCeo();
        var clientId = Factory.ModsenClientId!.Value;

        var projectInput = new CreateProjectInput
        {
            Name = Guid.NewGuid().ToString(),
            ClientId = clientId,
            IsBillable = true,
            IsPublic = false
        };

        // Act
        var project = await client.CreateProject(projectInput);

        await client.CreateProjectAssignment(
            Factory.EmployeeUserId!.Value,
            project!.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow),
            1m,
            true,
            "SeniorL2"
            );

        await client.AuthAsEmployee();
        var input1 = BuildInput(
            projectId: project.Id,
            userId: Factory.EmployeeUserId!.Value,
            jiraProjectTag: "MMS1");
        var input2 = BuildInput(
            projectId: project.Id,
            userId: Factory.EmployeeUserId!.Value,
            jiraProjectTag: "MMS2");

        var first = await client.CreateJiraSettingAsync(input1);
        var second = await client.CreateJiraSettingAsync(input2);

        // Act
        var response = await client.SearchJiraSettingsAsync();

        // Assert
        response!.Items.Should().Contain(x => x.Id == first!.Id);
        response.Items.Should().Contain(x => x.Id == second!.Id);
        response.Total.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task SearchAsyncWhenRequestIsInvalidShouldRespondWithBadRequestResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.SearchJiraSettingsAsHttpMessageAsync("?top=10&skip=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
