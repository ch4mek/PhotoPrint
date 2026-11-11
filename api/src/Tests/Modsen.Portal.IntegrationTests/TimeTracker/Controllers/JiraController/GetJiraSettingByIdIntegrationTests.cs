using System.Net;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

[Collection("JiraControllerCollection")]
public sealed class GetJiraSettingByIdIntegrationTests : JiraControllerIntegrationTestsBase
{
    public GetJiraSettingByIdIntegrationTests(JiraIntegrationTestFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetByIdAsyncWhenJiraSettingDoesNotExistShouldRespondWithNotFoundResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetJiraSettingByIdAsHttpMessageAsync(int.MaxValue);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByIdAsyncWhenIdIsInvalidShouldRespondWithBadRequestResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetJiraSettingByIdAsHttpMessageAsync(0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetByIdAsyncWhenRequestIsValidShouldRespondWithJiraSetting()
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
        var input = BuildInput(
            projectId: project.Id,
            userId: Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var created = await client.CreateJiraSettingAsync(input);

        // Act
        var response = await client.GetJiraSettingByIdAsync(created!.Id);

        // Assert
        response.Should().BeEquivalentTo(created);
    }
}
