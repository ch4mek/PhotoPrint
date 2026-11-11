using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class GetProjectByIdAsyncIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectByIdAsyncIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetByIdAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int projectId = 1;

        // Act
        var response = await client.GetProjectByIdAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetByIdAsyncWhenProjectDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int projectId = int.MaxValue;

        // Act
        var response = await client.GetProjectByIdAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsyncWhenAuthedAsHeadOfDepartmentAndNonPublicProjectIsCreatedByOtherUserShouldResponseWithForbiddenResult(
            bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Mobile");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetProjectByIdAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsyncWhenAuthedAsGroupManagerAndNonPublicProjectIsCreatedByOtherUserShouldResponseWithForbiddenResult(
            bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee();
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetProjectByIdAsHttpMessageAsync(projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsyncWhenNonPublicProjectIsCreatedBySameHeadOfDepartmentAsAuthedShouldResponseWithOkResult(
            bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        var expectedBody = new ProjectWithManagerOutput
        {
            Id = project.Id,
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = true,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Name = project.Name,
            IsSelfCreatedProject = true,
            ManagerId = null,
            ManagerFullName = null
        };

        // Act
        var response = await client.GetProjectByIdAsync(projectId);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsyncWhenNonPublicProjectIsCreatedBySameGroupManagerAsAuthedShouldResponseWithOkResult(
            bool isProjectBillable)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(resourceGroupId: _factory.ResourceGroupId);
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId, isBillable: isProjectBillable);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.CreateProjectAsHttpMessageAsync(clientId: clientId,
            isBillable: isProjectBillable);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetByIdAsyncWhenAuthedAsHeadOfDepartmentAndProjectIsPublicShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var projectId = project!.Id;

        await client.AuthAsHeadOfDepartment();

        var expectedBody = new ProjectWithManagerOutput
        {
            Id = projectId,
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = true,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Name = project.Name,
            IsSelfCreatedProject = false,
            ManagerId = null,
            ManagerFullName = null
        };

        // Act
        var response = await client.GetProjectByIdAsync(projectId);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task GetByIdAsyncWhenAuthedAsCtoShouldResponseWithOkResult(
        bool isBillableProject,
        bool isPublicProject)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId,
            isBillable: isBillableProject,
            isPublic: isPublicProject);
        var projectId = project!.Id;

        await client.AuthAsCto();

        var expectedBody = new ProjectWithManagerOutput
        {
            Id = project.Id,
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = true,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Name = project.Name,
            IsSelfCreatedProject = false,
            ManagerId = null,
            ManagerFullName = null
        };

        // Act
        var response = await client.GetProjectByIdAsync(projectId);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }

    [Fact]
    public async Task GetByIdAsyncWhenProjectHasManagerShouldReturnProjectWithManagerInfo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var managerUser = await client.CreateEmployee(departmentId: "PMO");
        var managerId = managerUser!.Id;

        await client.AuthAsEmployee();
        var user = await client.GetCurrentUser();
        var userId = user.Id;

        await client.AuthAsCeo();

        var project = await client.CreateProject(clientId: clientId, managerId: managerId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var expectedBody = new ProjectWithManagerOutput
        {
            Id = project.Id,
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = true,
            IsSelfCreatedProject = true,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Name = project.Name,
            ManagerId = managerUser.Id,
            ManagerFullName = $"{managerUser.LastName} {managerUser.FirstName}"
        };

        // Act
        var response = await client.GetProjectByIdAsync(projectId);

        // Assert
        response.Should().BeEquivalentTo(expectedBody);
    }
}
