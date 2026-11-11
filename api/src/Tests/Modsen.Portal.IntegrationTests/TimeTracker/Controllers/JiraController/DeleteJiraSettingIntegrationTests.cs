using System.Net;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

[Collection("JiraControllerCollection")]
public sealed class DeleteJiraSettingIntegrationTests : JiraControllerIntegrationTestsBase
{
    public DeleteJiraSettingIntegrationTests(JiraIntegrationTestFactory factory)
        : base(factory) { }

    [Fact]
    public async Task DeleteAsyncWhenAuthedAsAnotherUserShouldRespondWithForbiddenResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var input = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var created = await client.CreateJiraSettingAsync(input);

        await client.AuthAsCeo();

        // Act
        var response = await client.DeleteJiraSettingAsHttpMessageAsync(created!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAsyncWhenJiraSettingDoesNotExistShouldRespondWithNotFoundResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.DeleteJiraSettingAsHttpMessageAsync(int.MaxValue);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAsyncWhenIdIsInvalidShouldRespondWithBadRequestResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.DeleteJiraSettingAsHttpMessageAsync(0);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteAsyncWhenRequestIsValidShouldDeactivateJiraSetting()
    {
        // Arrange
        var client = Factory.CreateClient();

        await client.AuthAsCeo();

        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var input = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var created = await client.CreateJiraSettingAsync(input);

        // Act
        await client.DeleteJiraSettingAsync(created!.Id);
        var getResponse = await client.GetJiraSettingByIdAsHttpMessageAsync(created.Id);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<int> CreateNewProjectAssigment(HttpClient client, int userId)
    {
        await client.AuthAsCeo();
        var clientId = Factory.ModsenClientId!.Value;

        var input = new CreateProjectInput
        {
            Name = Guid.NewGuid().ToString(),
            ClientId = clientId,
            IsBillable = true,
            IsPublic = false
        };

        // Act
        var response = await client.CreateProject(input);

        await client.CreateProjectAssignment(
            userId,
            response!.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow),
            1m,
            true,
            "SeniorL2"
            );

        return response!.Id;
    }
}
