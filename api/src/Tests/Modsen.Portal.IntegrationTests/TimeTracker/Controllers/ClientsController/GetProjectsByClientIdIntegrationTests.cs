using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ClientsController;

[Collection("TimeTrackerCollection")]
public class GetProjectsByClientIdIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectsByClientIdIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetProjectsByClientIdShouldReturnOnlyActiveProjects()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCto();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var projectToBeActive = await httpClient.CreateProject(clientId: clientId);

        var projectToBeInactive = await httpClient.CreateProject(clientId: clientId);
        var inactiveProjectId = projectToBeInactive!.Id;

        await httpClient.SetProjectInactiveAsHttpMessageAsync(inactiveProjectId);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = projectToBeActive!.Id,
                Color = projectToBeActive.Color,
                IsActive = projectToBeActive.IsActive,
                Name = projectToBeActive.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            }
        };

        // Act
        var clients = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        clients.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsEmployeeWhichNotAssignedToAnyOfClientProjectsShouldResponseWithClientsPublicProjects()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var createdProject = await httpClient.CreateProject(clientId: clientId);
        var projectId = createdProject!.Id;

        await httpClient.CreateProjectAssignment(
            userId: headId,
            projectId: projectId);

        var publicProject = await httpClient.CreateProject(
            clientId: clientId,
            isBillable: false,
            isPublic: true);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = publicProject!.Id,
                Color = publicProject.Color,
                IsActive = publicProject.IsActive,
                Name = publicProject.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            }
        };

        await httpClient.AuthAsEmployee();

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedEmployeeIsFromPmoDepartmentShouldResponseWithClientsPublicProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdPmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var employeeId = createdPmoEmployee!.Id;

        const string filter = "((departmentId eq 'PMO') and (position eq 'HeadOfDepartment'))";

        var users = await client.GetUsersWithSearch(filter: filter, top: 1);

        var pmoHeadId = users.Items.First().Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var expectedProject = await client.CreateProject(
            clientId: clientId,
            managerId: employeeId,
            isBillable: true,
            isPublic: false);
        var projectId = expectedProject!.Id;

        await client.CreateProjectAssignment(userId: pmoHeadId, projectId: projectId);

        await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = expectedProject.Id,
                Color = expectedProject.Color,
                IsActive = expectedProject.IsActive,
                Name = expectedProject.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            }
        };

        await client.AuthAs(createdPmoEmployee.Email);

        // Act
        var result = await client.GetProjectsByClientIdAsync(id: clientId,
            onlyManagedProjects: true);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsHeadOfPmoDepartmentShouldResponseWithClientsPublicProjects()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();
        var employeeId = _factory.EmployeeUserId!.Value;

        const string filter = "((departmentId eq 'PMO') and (position eq 'HeadOfDepartment'))";
        var users = await httpClient.GetUsersWithSearch(top: 1, filter: filter);
        var pmoHeadId = users.Items.First().Id;

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var expectedProject = await httpClient.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectId = expectedProject!.Id;

        await httpClient.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        await httpClient.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = expectedProject.Id,
                Color = expectedProject.Color,
                IsActive = expectedProject.IsActive,
                Name = expectedProject.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            }
        };

        await httpClient.AuthAs(users.Items.First().Email);

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId,
            onlyManagedProjects: true);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsEmployeeAndAssignedToClientsBillableProjectShouldResponseWithClientsBillableProject()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var employeeId = _factory.EmployeeUserId!.Value;
        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var billableProject = await httpClient.CreateProject(clientId: clientId, isBillable: true, isPublic: false);
        var projectId = billableProject!.Id;

        await httpClient.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        var publicProject = await httpClient.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = billableProject.Id,
                Color = billableProject.Color,
                IsActive = billableProject.IsActive,
                Name = billableProject.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            },
            new()
            {
                Id = publicProject!.Id,
                Color = publicProject.Color,
                IsActive = publicProject.IsActive,
                Name = publicProject.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            }
        };

        await httpClient.AuthAsEmployee();

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects,
            opt => opt.WithStrictOrdering());
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsEmployeeAndProjectIdIsNotProvidedShouldResponseWithAllProjectUserAssignedToAndPublic()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();
        var employeeId = _factory.EmployeeUserId!.Value;

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var billableProject = await httpClient.CreateProject(clientId: clientId);
        var projectId = billableProject!.Id;

        await httpClient.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = billableProject.Id,
                Color = billableProject.Color,
                IsActive = billableProject.IsActive,
                Name = billableProject.Name,
                FactTrackedTime = 0,
                BillableTrackedTime = 0
            }
        };

        await httpClient.AuthAsEmployee();

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsGroupManagerShouldResponseWithProjectsWithUsersFromSameResourceGroup()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var createdUserFromGroup = await httpClient.CreateEmployee(resourceGroupId: 1);
        var userId = createdUserFromGroup!.Id;

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var project = await httpClient.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        await httpClient.CreateProject(clientId: clientId, isPublic: false, isBillable: false);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = project.Id,
                Color = project.Color,
                IsActive = project.IsActive,
                Name = project.Name,
                FactTrackedTime = null,
                BillableTrackedTime = null
            }
        };

        await httpClient.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsCeoShouldResponseWithAllClientsProjects()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCto();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var billableProject = await httpClient.CreateProject(clientId: clientId);

        var notBillableProject = await httpClient.CreateProject(clientId: clientId, isPublic: false,
            isBillable: false);

        var publicProject = await httpClient.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        var expectedProjects = new List<ProjectOutput>
        {
            billableProject!,
            notBillableProject!,
            publicProject!
        }.ConvertAll(x => new ClientProjectOutput
        {
            Id = x.Id,
            Color = x.Color,
            IsActive = x.IsActive,
            Name = x.Name,
            FactTrackedTime = 0,
            BillableTrackedTime = 0
        });

        await httpClient.AuthAsCeo();

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        result.Should().BeEquivalentTo(
            expectedProjects,
            opt => opt.WithStrictOrdering());
    }
}
