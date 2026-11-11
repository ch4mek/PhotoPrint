using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class CreateProjectsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateProjectsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var clientId = _factory.ModsenClientId!.Value;

        // Act
        var response = await client.CreateProjectAsHttpMessageAsync(clientId: clientId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAsyncWhenProjectIsPublicAndBillableShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        // Act
        var response = await client.CreateProjectAsHttpMessageAsync(clientId: clientId,
            isBillable: true,
            isPublic: true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("TimeTracker-Modsen", "timetracker-modsen")]
    [InlineData("timetracker-modsen", "TimeTracker-Modsen")]
    [InlineData("TimeTracker-modsen", "TimeTracker-Modsen")]
    [InlineData("timetrAcker-mOdSEN", "TIMetracKer-mOdseN")]
    public async Task CreateAsyncWhenProjectWithTheSameNameAlreadyExistsShouldResponseWithBadRequestResult(
        string existingProjectName,
        string newProjectName)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var input = new CreateProjectInput
        {
            Name = existingProjectName, ClientId = clientId, IsBillable = true, IsPublic = false
        };

        await client.CreateProjectAsHttpMessageAsync(input: input);

        input.Name = newProjectName;

        // Act
        var response = await client.CreateProjectAsHttpMessageAsync(input: input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [MemberData(nameof(GetInvalidProjectParams))]
    public async Task CreateAsyncWhenInputParamsAreInvalidShouldResponseWithBadRequestResult(
        string name,
        string color)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var input = new CreateProjectInput
        {
            Name = name, Color = color, ClientId = clientId, IsBillable = true, IsPublic = false
        };

        // Act
        var response = await client.CreateProjectAsHttpMessageAsync(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsyncWhenRequestIsValidShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var expectedBody = new ProjectOutput
        {
            Id = 0,
            ClientId = clientId,
            Color = "#000000",
            IsBillable = true,
            IsActive = true,
            IsPublic = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
        };

        // Act
        var response = await client.CreateProject(
            clientId: clientId,
            isPublic: expectedBody.IsPublic,
            name: expectedBody.Name,
            isBillable: expectedBody.IsBillable);

        // Assert
        response.Should().BeEquivalentTo(
            expectedBody,
            options => options.Excluding(x => x.Id).Excluding(x => x.Color));
        response.Id.Should().BeGreaterThan(0);
        response.Color.Should().MatchRegex("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }

    [Fact]
    public async Task CreateAsyncWhenProjectManagerProvidedAndUserCannotBeManagerShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdManager = await client.CreateEmployee(departmentId: "Net");
        var userId = createdManager!.Id;

        // Act
        var response = await client.CreateProjectAsHttpMessageAsync(clientId: clientId, managerId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAsyncWhenHeadOfDepartmentTriesToAssignProjectManagerShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var userId = createdManager!.Id;

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.CreateProjectAsHttpMessageAsync(clientId: clientId, managerId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAsyncWhenGroupManagerTriesToAssignProjectManagerShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var userId = createdManager!.Id;

        // Act
        await client.AuthAsGroupManager();

        var response = await client.CreateProjectAsHttpMessageAsync(clientId: clientId, managerId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

    }

    [Fact]
    public async Task CreateAsyncWhenProjectManagerProvidedAndUserCanBeManagerShouldReturnProjectWithProjectManger()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var userId = createdManager!.Id;

        var expectedBody = new ProjectOutput
        {
            Id = 0,
            ClientId = clientId,
            ManagerId = userId,
            Color = "#433e90",
            IsBillable = true,
            IsActive = true,
            IsPublic = false,
            Name = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.CreateProject(
            clientId: clientId,
            managerId: userId,
            color: expectedBody.Color,
            isBillable: expectedBody.IsBillable,
            isPublic: expectedBody.IsPublic,
            name: expectedBody.Name);

        // Assert
        response.Should().BeEquivalentTo(expectedBody, options => options.Excluding(x => x.Id));
    }

    public static IEnumerable<object?[]> GetInvalidProjectParams
    {
        get
        {
            yield return [string.Empty, "#fff"];
            yield return [null, "#123456"];
            yield return ["A", null];
            yield return [new string('l', 251), "#000"];
            yield return ["Test project 1", "#aa"];
            yield return ["Test project 2", "eee"];
            yield return ["Test project 3", "777#"];
            yield return ["Test project 4", "123456"];
            yield return ["Test project 5", "*234567"];
            yield return ["Test project 6", "#efg"];
            yield return ["Test project 7", "#"];
            yield return [new string('p', 64), "%abc"];
        }
    }
}
