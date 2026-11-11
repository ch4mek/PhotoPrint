using System.Globalization;
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
public class UpdateProjectsEndpointIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateProjectsEndpointIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = project!.Id,
            ClientId = project.ClientId,
            Color = "#999999",
            IsBillable = !project.IsBillable,
            IsPublic = !project.IsPublic,
            Name = "Update project test"
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenProjectDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var updateProjectInput = new UpdateProjectInput
        {
            Id = int.MaxValue,
            ClientId = _factory.ModsenClientId.Value,
            Color = "#999999",
            IsBillable = true,
            IsPublic = false,
            Name = "Update project test"
        };

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenHeadOfDepartmentTriesToUpdateDeactivatedProjectShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.SetProjectInactiveAsync(projectId);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            ClientId = project.ClientId,
            Color = project.Color,
            IsBillable = true,
            IsPublic = false,
            Name = project.Name
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenGroupManagerTriesToUpdateDeactivatedProjectShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.SetProjectInactiveAsync(projectId);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            ClientId = project.ClientId,
            Color = project.Color,
            IsBillable = true,
            IsPublic = false,
            Name = project.Name
        };

        await client.AuthAsGroupManager();

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    [Fact]
    public async Task UpdateProjectAsyncWhenTryingToMarkProjectAsPublicAndBillableShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            ClientId = project.ClientId,
            Color = project.Color,
            IsBillable = true,
            IsPublic = true,
            Name = project.Name
        };

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectAsyncProjectWithSameNameAlreadyExistsShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var existingProject = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var projectToUpdate = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectToUpdate!.Id,
            ClientId = projectToUpdate.ClientId,
            Color = projectToUpdate.Color,
            IsBillable = true,
            IsPublic = false,
            Name = existingProject!.Name
        };

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenProjectManagerProvidedAndUserCannotBeManagerShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var createdManager = await client.CreateEmployee();
        var managerId = createdManager!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            Name = project.Name,
            Color = project.Color,
            ManagerId = managerId,
            ClientId = project.ClientId,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable
        };

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenHeadOfDepartmentTriesToUpdateManagedProjectShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var managerId = createdManager!.Id;

        var createdUser = await client.CreateEmployee(departmentId: "Net");
        var userId = createdUser!.Id;

        var project = await client.CreateProject(clientId: clientId, managerId: managerId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(projectId: projectId, userId: userId);

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            Name = "New Name",
            Color = project.Color,
            ManagerId = managerId,
            ClientId = project.ClientId,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable
        };

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenHeadTriesToUpdateProjectNotRelatedToHimShouldThrowActionNotAllowedException()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            Name = "New Name",
            Color = project.Color,
            ManagerId = null,
            ClientId = project.ClientId,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable
        };

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenHeadOfDepartmentTriesToAssignProjectManagerShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var managerId = createdManager!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            Name = project.Name,
            Color = project.Color,
            ManagerId = managerId,
            ClientId = project.ClientId,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable
        };

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenGroupManagerTriesToAssignProjectManagerShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var managerId = createdManager!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            Name = project.Name,
            Color = project.Color,
            ManagerId = managerId,
            ClientId = project.ClientId,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable
        };

        // Act
        await client.AuthAsGroupManager();

        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenProjectManagerProvidedAndUserCanBeManagerShouldReturnProjectWithProjectManger()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        var createdManager = await client.CreateEmployee(departmentId: "PMO");
        var managerId = createdManager!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            Name = project.Name,
            Color = project.Color,
            ManagerId = managerId,
            ClientId = project.ClientId,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable
        };

        var expectedBody = new UpdateProjectOutput
        {
            Id = projectId,
            Name = project.Name,
            Color = project.Color,
            ClientId = project.ClientId,
            ClientName = null!,
            IsPublic = project.IsPublic,
            IsActive = true,
            IsBillable = project.IsBillable,
            IsCreatedByCurrentUser = true,
            ManagerId = managerId,
            TrackedTime = 0
        };

        // Act
        var response = await client.UpdateProjectAsync(updateProjectInput);

        // Assert
        response.Should().BeEquivalentTo(expectedBody, options =>
        {
            return options
                .Excluding(x => x.ClientName);
        });
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenClientDoesNotExistShouldResponseWithBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            ClientId = int.MaxValue,
            Color = project.Color,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Name = project.Name
        };

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(GetInvalidProjectParams))]
    public async Task UpdateProjectAsyncWhenInputParamsAreInvalidShouldResponseWithBadRequestResult(
        string name,
        string color)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            ClientId = project.ClientId,
            Color = color,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Name = name
        };

        // Act
        var response = await client.UpdateProjectAsHttpMessageAsync(updateProjectInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProjectAsyncWhenUserIsCeoAndRequestIsValidShouldResponseWithOkResultAndUpdatedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        var updateProjectInput = new UpdateProjectInput
        {
            Id = projectId,
            ClientId = project.ClientId,
            Color = "#555555",
            IsBillable = true,
            IsPublic = false,
            Name = Guid.CreateVersion7().ToAlphabetic()
        };

        var expectedBody = new UpdateProjectOutput
        {
            Id = updateProjectInput.Id,
            ClientId = updateProjectInput.ClientId,
            ClientName = "Modsen",
            Color = updateProjectInput.Color,
            IsBillable = updateProjectInput.IsBillable,
            IsPublic = updateProjectInput.IsPublic,
            IsCreatedByCurrentUser = false,
            TrackedTime = 0,
            Name = updateProjectInput.Name,
            IsActive = true
        };

        await client.AuthAsCeo();

        // Act
        var response = await client.UpdateProjectAsync(updateProjectInput);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task DeleteProjectPermissionAssignmentWhenStatusIsActiveChangedShouldResponseWithOkResultAndDeletePermissionAssignmentsFromDb()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;
        var projectName = Guid.CreateVersion7().ToAlphabetic();

        var project = await client.CreateProject(clientId: clientId, name: projectName, isBillable: false,
            isPublic: true);
        var projectId = project!.Id;

        await client.UpdateUserPermissionAssignments(userId: userId,
            resources: new Dictionary<string, string>
            {
                { projectId.ToString(CultureInfo.InvariantCulture), projectName }
            });

        var assignmentsBeforeDeleting = await client.GetPermissionAssignments(_jsonOptions);

        var countOfAssignments = assignmentsBeforeDeleting!.Items.Count;

        // Act
        var response = await client.SetProjectInactiveAsHttpMessageAsync(projectId);


        // Assert
        var assignmentsAfterDeleting = await client.GetPermissionAssignments(_jsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        assignmentsBeforeDeleting.Items.Should().HaveCount(countOfAssignments);
        assignmentsAfterDeleting!.Items.Should().HaveCount(countOfAssignments - 1);
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
