using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.UserProjectsController;

[Collection("TimeTrackerCollection")]
public class UpdateUserProjectAssignmentIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateUserProjectAssignmentIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateUserProjectAssignmentWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var createdProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = createdProject!.Id;

        var createdUserProject = await client.CreateProjectAssignment(userId: userId, projectId: projectId,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow));

        var userProjectToUpdate = new UpdateUserProjectAssignmentInput(createdUserProject!.Id,
            createdUserProject.UserId, createdUserProject.ProjectId,
            IsTimeOnProjectBillable: true, UserOnProjectSeniorityLevelId: "SeniorL2", Fte: 0.6m,
            UserOnProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)),
            UserOnProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var response = await client.UpdateProjectAssignmentAsHttpMessageAsync(userProjectToUpdate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var retrievedUserProject = await client.GetUserProjectsAsync(createdUserProject.Id);

        retrievedUserProject.Id.Should().Be(createdUserProject.Id);
        retrievedUserProject.UserId.Should().Be(createdUserProject.UserId);
        retrievedUserProject.ProjectId.Should().Be(createdUserProject.ProjectId);
        retrievedUserProject.UserFullName.Should().NotBeEmpty();
        retrievedUserProject.ProjectName.Should().NotBeEmpty();
        retrievedUserProject.SeniorityLevel.Should().NotBeEmpty();
    }
}
