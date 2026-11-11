using System.Net;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

[Collection("JiraControllerCollection")]
public sealed class UpdateJiraSettingIntegrationTests : JiraControllerIntegrationTestsBase
{
    public UpdateJiraSettingIntegrationTests(JiraIntegrationTestFactory factory)
        : base(factory) { }

    [Fact]
    public async Task UpdateAsyncWhenAuthedAsAnotherUserShouldRespondWithForbiddenResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var input = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);
        var updateInput = BuildUpdateInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);

        var created = await client.CreateJiraSettingAsync(input);

        await client.AuthAsCeo();

        // Act
        var response = await client.UpdateJiraSettingAsHttpMessageAsync(created!.Id, updateInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateAsyncWhenJiraSettingDoesNotExistShouldRespondWithNotFoundResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateJiraSettingAsHttpMessageAsync(int.MaxValue, CreateValidUpdateInput());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateParams), MemberType = typeof(JiraControllerIntegrationTestsBase))]
    public async Task UpdateAsyncWhenInputIsInvalidShouldRespondWithBadRequestResult(UpdateJiraSettingInput input)
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateJiraSettingAsHttpMessageAsync(1, input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAsyncWhenIdIsInvalidShouldRespondWithBadRequestResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateJiraSettingAsHttpMessageAsync(0, CreateValidUpdateInput());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAsyncWhenRequestIsValidShouldUpdateJiraSetting()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var inputOld = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value,
            jiraProjectTag: $"OLD{Guid.NewGuid():N}"[..12]);
        var inputNew = BuildUpdateInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value,
            jiraProjectTag: $"NEW{Guid.NewGuid():N}"[..12],
            email: "new@email.com");

        await client.AuthAsEmployee();
        var created = await client.CreateJiraSettingAsync(inputOld);

        // Act
        var response = await client.UpdateJiraSettingAsync(created!.Id, inputNew);

        // Assert
        response.Should().BeEquivalentTo(inputNew, options => options.ExcludingMissingMembers());
        response.Id.Should().Be(created.Id);
        response.Email.Should().Be(inputNew.Email);
        response.JiraProjectTag.Should().Be(inputNew.JiraProjectTag);
    }

    [Fact]
    public async Task UpdateAsyncWhenPatIsNotProvidedShouldKeepExistingPatAndUpdateJiraSetting()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var inputOld = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value,
            jiraProjectTag: $"OLD{Guid.NewGuid():N}"[..12]);
        var inputNew = BuildUpdateInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value,
            jiraProjectTag: $"NEW{Guid.NewGuid():N}"[..12],
            email: "new@email.com");

        await client.AuthAsEmployee();
        var created = await client.CreateJiraSettingAsync(inputOld);

        // Act
        var response = await client.UpdateJiraSettingAsync(created!.Id, inputNew);

        // Assert
        response.Should().BeEquivalentTo(inputNew, options => options.ExcludingMissingMembers());
        response.Id.Should().Be(created.Id);
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
