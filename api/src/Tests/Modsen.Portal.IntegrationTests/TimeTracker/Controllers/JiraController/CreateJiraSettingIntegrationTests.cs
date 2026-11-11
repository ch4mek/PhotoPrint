using System.Net;
using System.Net.Http.Json;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.Jira.Contract.Exceptions;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Moq;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

[Collection("JiraControllerCollection")]
public sealed class CreateJiraSettingIntegrationTests : JiraControllerIntegrationTestsBase
{
    public CreateJiraSettingIntegrationTests(JiraIntegrationTestFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateAsyncWhenAuthedAsAnotherUserShouldRespondWithForbiddenResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateJiraSettingAsHttpMessageAsync(CreateValidInput());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [MemberData(nameof(GetInvalidCreateParams), MemberType = typeof(JiraControllerIntegrationTestsBase))]
    public async Task CreateAsyncWhenInputIsInvalidShouldRespondWithBadRequestResult(CreateJiraSettingInput input)
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.CreateJiraSettingAsHttpMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(GetJiraCredentialsErrors))]
    public async Task CreateAsyncWhenJiraCredentialsCheckFailsShouldRespondWithExpectedError(
        Exception exception,
        HttpStatusCode expectedStatusCode,
        string expectedMessage)
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCto();
        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var input = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);

        JiraServiceMock
            .Setup(x => x.CheckAccessAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var response = await client.CreateJiraSettingAsHttpMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);
        var error = await response.Content.ReadFromJsonAsync<ExceptionResponse>(Factory.JsonOptions);
        error!.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task CreateAsyncWhenRequestIsValidShouldRespondWithCreatedJiraSettingAndCheckCredentials()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCto();
        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var input = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);

        // Act
        var response = await client.CreateJiraSettingAsync(input);

        // Assert
        response.Should().BeEquivalentTo(input, options => options.ExcludingMissingMembers());
        response.Id.Should().BeGreaterThan(0);
        response.ProjectName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateAsyncWhenActiveJiraSettingAlreadyExistsShouldRespondWithConflictResult()
    {
        // Arrange
        var client = Factory.CreateClient();
        await client.AuthAsCeo();

        var projectId = await CreateNewProjectAssigment(client, Factory.EmployeeUserId!.Value);

        await client.AuthAsEmployee();
        var input = BuildInput(
            projectId: projectId,
            userId: Factory.EmployeeUserId!.Value);

        await client.CreateJiraSettingAsync(input);

        // Act
        var response = await client.CreateJiraSettingAsHttpMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await response.Content.ReadFromJsonAsync<ExceptionResponse>(Factory.JsonOptions);
        error!.Message.Should().Be("Jira setting already exists");
    }

    public static IEnumerable<object?[]> GetJiraCredentialsErrors()
    {
        yield return
        [
            new InvalidDataAppException($"Jira project {JiraProjectKey} not found"),
            HttpStatusCode.BadRequest,
            $"Jira project {JiraProjectKey} not found"
        ];

        yield return
        [
            new JiraUnauthorizedException("Invalid PAT"),
            HttpStatusCode.BadRequest,
            "Invalid PAT"
        ];

        yield return
        [
            new ActionNotAllowedException($"Access to Jira project {JiraProjectKey} is denied"),
            HttpStatusCode.Forbidden,
            $"Access to Jira project {JiraProjectKey} is denied"
        ];

        yield return
        [
            new EntityNotFoundException("Jira url not found"),
            HttpStatusCode.NotFound,
            "Jira url not found"
        ];

        yield return
        [
            new JiraCredentialsCheckFailedException(),
            HttpStatusCode.BadRequest,
            "Something went wrong. Please try again later."
        ];
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

        var projectUserToCreate = new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.UtcNow), IsTimeOnProjectBillable: true, Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        await client.CreateProjectAssignment(userId, response!.Id, projectUserToCreate);

        return response!.Id;
    }
}
