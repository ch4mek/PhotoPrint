using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.UserProjectsController;

[Collection("TimeTrackerCollection")]
public class GetUserProjectAssignmentByIdIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserProjectAssignmentByIdIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserProjectAssignmentByIdWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectId = createdProject.Id;

        var projectUserToCreate = new CreateUserProjectAssignmentInput(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), DateOnly.FromDateTime(DateTime.UtcNow),
            IsTimeOnProjectBillable: true, UserOnProjectSeniorityLevelId: "SeniorL3", Fte: 1m);

        var createdUserProjectResponse = await client.CreateProjectAssignment(userId,projectId,
            projectUserToCreate);

        var createdUserProject = await createdUserProjectResponse.Content.ReadFromJsonAsync<UserProjectAssignmentOutput>(
                _jsonOptions, _cancellationToken);

        // Act
        var response = await client.GetUserProjectsByAssignementId(createdUserProject.Id)
            .WithResponseType<UserProjectWithNamesDto>();

        // Assert
        response.Id.Should().Be(createdUserProject.Id);
        response.UserId.Should().Be(createdUserProject.UserId);
        response.ProjectId.Should().Be(createdUserProject.ProjectId);
        response.UserFullName.Should().NotBeEmpty();
        response.ProjectName.Should().NotBeEmpty();
        response.SeniorityLevel.Should().Be("Senior +");
    }
}
