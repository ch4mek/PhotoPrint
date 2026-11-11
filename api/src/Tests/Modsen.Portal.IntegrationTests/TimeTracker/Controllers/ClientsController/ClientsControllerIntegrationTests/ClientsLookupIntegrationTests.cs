using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ClientsController.ClientsControllerIntegrationTests;

[Collection("TimeTrackerCollection")]
public class ClientsLookupIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientsLookupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

     [Fact]
    public async Task LookupClientsAsyncWhenAuthedAsEmployeeResponseWithClientsWhereUserIsAssignedToAnyOfItsProjects()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var testClient = await httpClient.CreateClientAsync();
        var clientId = testClient!.Id;

        var createdProject = await httpClient.CreateProject(
            clientId: clientId,
            isBillable: false,
            isPublic: false);
        var projectId = createdProject!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        await httpClient.AuthAsEmployee();

        // Act
        var clients = await httpClient.GetClientsLookupAsync();

        // Assert
        clients.Should().HaveCountGreaterThanOrEqualTo(1);
        clients.Any(client => client.Name == testClient.Name)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task LookupClientsAsyncWhenAuthedAsHeadOfDepartmentResponseWithClientsWhereItsProjectsCreatedBySelfAndWhereUserFromSameDepartmentIsAssignedTo()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        const string filter = "((departmentId eq 'Net') and (position eq 'HeadOfDepartment'))";
        var users = await httpClient.GetUsersWithSearch(
            filter: filter,
            top: 1);

        var head = users.Items.First();

        var userFromHeadDepartment = await httpClient.CreateEmployee(departmentId: head.DepartmentId!);
        var userId = userFromHeadDepartment!.Id;

        var clientWithProjectWithUserFromSameDepartment = await httpClient.CreateClientAsync();

        var clientWithProjectCreatedByHead = await httpClient.CreateClientAsync();

        var createdProject = await httpClient.CreateProject(
            clientId: clientWithProjectWithUserFromSameDepartment!.Id,
            isBillable: false,
            isPublic: false);
        var projectId = createdProject!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        await httpClient.AuthAs(head.Email);

        await httpClient.CreateProject(
            clientId: clientWithProjectCreatedByHead!.Id,
            isBillable: false,
            isPublic: false);

        // Act
        var clients = await httpClient.GetClientsLookupAsync();

        // Assert
        clients.Should().HaveCountGreaterThanOrEqualTo(2);
        clients.Any(client => client.Name == clientWithProjectWithUserFromSameDepartment.Name)
            .Should()
            .BeTrue();
        clients.Any(client => client.Name == clientWithProjectCreatedByHead.Name)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task LookupClientsAsyncWhenAuthedAsGroupManagerResponseWithPublicClientsAndWhereUserFromSameGroupOrHimselfIsAssignedTo()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var createdUserFromGroup = await httpClient.CreateEmployee(resourceGroupId: 1);
        var createdUserNotFromGroup = await httpClient.CreateEmployee(resourceGroupId: 2, departmentId: "Java");

        var clientWithProjectWithUserFromSameGroup = await httpClient.CreateClientAsync();
        var clientWithProjectWithUserFromOtherGroup = await httpClient.CreateClientAsync();
        var clientWithPublicProject = await httpClient.CreateClientAsync();
        var clientWithProjectWithGroupManager = await httpClient.CreateClientAsync();

        var projectWithUserFromSameGroup = await httpClient.CreateProject(
            clientId: clientWithProjectWithUserFromSameGroup!.Id, isBillable: false, isPublic: false);
        var projectWithUserFromOtherGroup = await httpClient.CreateProject(
            clientId: clientWithProjectWithUserFromOtherGroup!.Id, isPublic: false, isBillable: false);
        await httpClient.CreateProject(
            clientId: clientWithPublicProject!.Id, isPublic: true, isBillable: false);
        var projectWithGroupManager = await httpClient.CreateProject(
            clientId: clientWithProjectWithGroupManager!.Id, isPublic: false, isBillable: false);

        await httpClient.CreateProjectAssignment(
            projectId: projectWithUserFromSameGroup!.Id,
            userId: createdUserFromGroup!.Id);

        await httpClient.CreateProjectAssignment(
            projectId: projectWithUserFromOtherGroup!.Id,
            userId: createdUserNotFromGroup!.Id);

        var groupManagerId = _factory.GroupManagerId!.Value;

        await httpClient.CreateProjectAssignment(
            projectId: projectWithGroupManager!.Id,
            userId: groupManagerId);

        await httpClient.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var clients = await httpClient.GetClientsLookupAsync();

        // Assert
        clients.Should().HaveCountGreaterThanOrEqualTo(expected: 3);

        var expectedNames = new[]
        {
            clientWithProjectWithUserFromSameGroup.Name,
            clientWithPublicProject.Name,
            clientWithProjectWithGroupManager.Name
        };

        expectedNames.All(name => clients.Any(client => client.Name == name))
            .Should()
            .BeTrue();
        clients.Should().NotContain(client => client.Name == clientWithProjectWithUserFromOtherGroup.Name);
    }

    [Fact]
    public async Task LookupClientsAsyncAuthedAsGroupManagerHavingTrueOnlyManagedClientsShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAs(EmailConstants.GroupManagerEmail);

        //Act
        var response = await client.GetClientsLookupAsHttpMessageAsync(onlyManagedClients: true);

        //Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task LookupClientssAsyncWhenHeadIsProjectManagerHavingTrueOnlyManagedClientsShouldReturnOkResultHavingOnlyManagedClientProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string filter = "((departmentId eq 'PMO') and (position eq 'HeadOfDepartment'))";
        var users = await client.GetUsersWithSearch(
            filter: filter,
            top: 1);

        var pmoHeadId = users.Items.First().Id;

        var createdPmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var userId = createdPmoEmployee!.Id;

        var clientWithProjectWithUserFromSameDepartment = await client.CreateClientAsync();
        var clientWithManagedProject = await client.CreateClientAsync();

        await client.CreateProject(
            clientId: clientWithManagedProject!.Id,
            isBillable: true,
            isPublic: false,
            managerId: pmoHeadId);

        var privateProjectWithUserFromDepartment = await client.CreateProject(
            clientId: clientWithProjectWithUserFromSameDepartment!.Id,
            isBillable: true,
            isPublic: false);

        await client.CreateProjectAssignment(
            projectId: privateProjectWithUserFromDepartment!.Id,
            userId: userId);

        await client.AuthAs(users.Items.First().Email);

        //Act
        var clients = await client.GetClientsLookupAsync(onlyManagedClients: true);

        //Assert
        clients.Should().NotBeNull();
        clients.Should().NotContain(p => p.Id == clientWithProjectWithUserFromSameDepartment.Id);
        clients.Should().Contain(p => p.Id == clientWithManagedProject.Id);
    }

    [Fact]
    public async Task LookupClientssAsyncWhenHeadIsProjectManagerHavingFalseOnlyManagedClientsShouldReturnOkResultHavingNotManagedClientProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string filter = "((departmentId eq 'PMO') and (position eq 'HeadOfDepartment'))";
        var users = await client.GetUsersWithSearch(
            filter: filter,
            top: 1);

        var pmoHeadId = users.Items.First().Id;

        var createdPmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var userId = createdPmoEmployee!.Id;

        var clientWithProjectWithUserFromSameDepartment = await client.CreateClientAsync();

        var clientWithManagedProject = await client.CreateClientAsync();

        await client.CreateProject(
            clientId: clientWithManagedProject!.Id,
            isBillable: true,
            isPublic: false,
            managerId: pmoHeadId);

        var privateProjectWithUserFromDepartment = await client.CreateProject(
            clientId: clientWithProjectWithUserFromSameDepartment!.Id,
            isBillable: true,
            isPublic: false);

        await client.CreateProjectAssignment(
                projectId: privateProjectWithUserFromDepartment!.Id,
                userId: userId);

        await client.AuthAs(users.Items.First().Email);

        //Act
        var clients = await client.GetClientsLookupAsync(onlyManagedClients: false);

        //Assert
        clients.Should().NotBeNull();
        clients.Should().Contain(p => p.Id == clientWithProjectWithUserFromSameDepartment.Id);
        clients.Should().NotContain(p => p.Id == clientWithManagedProject.Id);
    }

    [Fact]
    public async Task LookupClientsAsyncWhenKeyWordIsNotNullShouldResponseWithClientsWhereNamesAreMatching()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCto();
        const string keyWord = "modsen";

        // Act
        var clients = await httpClient.GetClientsLookupAsync(keyWord: keyWord);

        // Assert
        clients.Should().NotBeNull();
        clients.Should().HaveCountGreaterThanOrEqualTo(1);
        clients.Any(x => x.Name.StartsWith(keyWord, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task LookupClientsAsyncWhenKeyWordIsNullShouldResponseWithAllClients()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var client = await httpClient.CreateClientAsync();

        // Act
        var clients = await httpClient.GetClientsLookupAsync();

        // Assert
        clients.Should().NotBeNull();
        clients.Should().HaveCountGreaterThanOrEqualTo(2);
        clients.Any(x => x.Name == client!.Name).Should().BeTrue();
        clients.Any(x => x.Name.StartsWith("Modsen", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }
}
