using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class GetProjectsLookupIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectsLookupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenAuthedAsEmployeeResponseWithProjectsWhereUserIsAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var projectOutput = await client.CreateProject(clientId: clientId);
        var projectId = projectOutput!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsLookupAsync();

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Any(project => project.Name == projectOutput.Name).Should().BeTrue();
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenAuthedAsHeadOfDepartmentResponseWithProjectsCreatedBySelfAndWhereUserFromSameDepartmentIsAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var projectWithUserFromSameDepartment = await client.CreateProject(clientId: clientId);
        var projectWithUserId = projectWithUserFromSameDepartment!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectWithUserId);

        await client.AuthAsHeadOfDepartment();

        var projectCreatedByHead = await client.CreateProject(clientId: clientId, isBillable: false);

        // Act
        var projects = await client.GetProjectsLookupAsync();

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Any(project => project.Name == projectWithUserFromSameDepartment.Name).Should().BeTrue();
        projects.Any(project => project.Name == projectCreatedByHead!.Name).Should().BeTrue();
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenAuthedAsGroupManagerResponseWithProjectsWhereUsersFromResourceGroupAreAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var resourceGroupId = _factory.ResourceGroupId;

        var firstUserFromGroup = await client.CreateEmployee(resourceGroupId: resourceGroupId);
        var firstUserId = firstUserFromGroup!.Id;
        var secondUserFromGroup = await client.CreateEmployee(resourceGroupId: resourceGroupId);
        var secondUserId = secondUserFromGroup!.Id;

        var projectWithFirstUserFromSameGroup = await client.CreateProject(clientId: clientId);
        var firstProjectId = projectWithFirstUserFromSameGroup!.Id;
        var projectWithSecondUserFromSameGroup = await client.CreateProject(clientId: clientId);
        var secondProjectId = projectWithSecondUserFromSameGroup!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: secondUserId, projectId: secondProjectId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var projects = await client.GetProjectsLookupAsync();

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Any(project => project.Name == projectWithFirstUserFromSameGroup.Name).Should().BeTrue();
        projects.Any(project => project.Name == projectWithSecondUserFromSameGroup.Name).Should().BeTrue();
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenAuthedAsManagerOfProjectRequestingPublicProjectsResponseWithManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;
        var clientId = _factory.ModsenClientId!.Value;

        var createdPublicProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId, isPublic: true,
            isBillable: false);
        var publicProjectId = createdPublicProject!.Id;

        await client.CreateProject(clientId: clientId, managerId: pmoHeadId);

        // Act
        await client.AuthAs(pmoHeadEmail);

        var projects = await client.GetProjectsLookupAsync(
            onlyManagedProjects: true,
            isActive: true,
            isPublic: true);

        // Assert
        projects.Should().Contain(x => x.Id == publicProjectId);
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenAuthedAsHeadOfDepartmentAndIsManagerOfProjectResponseWithManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var createdProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);
        var projectId = createdProject!.Id;

        await client.AuthAs(pmoHeadEmail);

        // Act
        var projects = await client.GetProjectsLookupAsync(onlyManagedProjects: true);

        // Assert
        projects.Should().Contain(x => x.Id == projectId);
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenKeyWordIsNotNullShouldResponseWithProjectsWhereNamesAreMatching()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        const string keyWord = "idle";

        // Act
        var projects = await client.GetProjectsLookupAsync(keyWord: keyWord);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Any(x => x.Name == "Idle").Should().BeTrue();
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenKeyWordIsNullShouldResponseWithAllProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        var project = await client.CreateProject(clientId: clientId);

        await client.AuthAsCeo();

        // Act
        var projects = await client.GetProjectsLookupAsync();

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Any(x => x.Name == project!.Name).Should().BeTrue();
        projects.Any(x => x.Name == "Idle").Should().BeTrue();
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenIsActiveFilterIsEqualToTrueShouldResponseWithActiveProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        var activeProject = await client.CreateProject(clientId: clientId);
        var activeProjectId = activeProject!.Id;

        var inactiveProject = await client.CreateProject(clientId: clientId);
        var inactiveProjectId = inactiveProject!.Id;

        await client.SetProjectInactiveAsHttpMessageAsync(inactiveProjectId);

        await client.AuthAsCeo();

        // Act
        var projects = await client.GetProjectsLookupAsync(isActive: true);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Should().Contain(p => p.Id == activeProjectId);
        projects.Should().NotContain(p => p.Id == inactiveProjectId);
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenIsPublicIsFalseShouldResponseWithAllNotPublicProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var billableProject = await client.CreateProject(clientId: clientId);
        var billableProjectId = billableProject!.Id;

        var publicProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        var notBillableProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        var notBillableProjectId = notBillableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, billableProjectId);
        await client.CreateProjectAssignment(userId: userId, notBillableProjectId, isTimeBillable: false);

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsLookupAsync(isPublic: false);

        // Assert
        projects.Should().NotBeNull();
        projects.Any(p => p.Id == billableProject.Id).Should().BeTrue();
        projects.Any(p => p.Id == publicProject.Id).Should().BeFalse();
        projects.Any(p => p.Id == notBillableProject.Id).Should().BeFalse();
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenHeadIsProjectManagerHavingTrueOnlyManagedProjectsShouldReturnOkResultHavingOnlyManagedProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var pmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var pmoEmployeeId = pmoEmployee!.Id;

        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var managedProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);

        var privateProjectWithUserFromDepartment = await client.CreateProject(clientId: clientId);
        var privateProjectId = privateProjectWithUserFromDepartment!.Id;

        await client.CreateProjectAssignment(userId: pmoEmployeeId, projectId: privateProjectId);

        var publicProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        await client.AuthAs(pmoHeadEmail);

        //Act
        var projects = await client.GetProjectsLookupAsync(onlyManagedProjects: true);

        //Assert
        projects.Should().NotBeNull();
        projects.Should().NotContain(p => p.Id == privateProjectWithUserFromDepartment.Id);
        projects.Should().NotContain(p => p.Id == publicProject!.Id);
        projects.Should().Contain(p => p.Id == managedProject!.Id);
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenHeadIsProjectManagerHavingFalseOnlyManagedProjectsShouldReturnOkResultHavingOnlyManagedProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var pmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var pmoEmployeeId = pmoEmployee!.Id;

        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var managedProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId);

        var privateProjectWithUserFromDepartment = await client.CreateProject(clientId: clientId);
        var privateProjectId = privateProjectWithUserFromDepartment!.Id;

        await client.CreateProjectAssignment(userId: pmoEmployeeId, projectId: privateProjectId);

        var publicProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        await client.AuthAs(pmoHeadEmail);

        //Act
        var projects = await client.GetProjectsLookupAsync(onlyManagedProjects: false);

        //Assert
        projects.Should().NotBeNull();
        projects.Should().Contain(p => p.Id == privateProjectWithUserFromDepartment.Id);
        projects.Should().Contain(p => p.Id == publicProject!.Id);
        projects.Should().NotContain(p => p.Id == managedProject!.Id);
    }

    [Fact]
    public async Task LookupProjectsAsyncWhenUserIsHeadOfDepartmentAndWithoutQueryParametersShouldResponseWithAllPublicOrCreatedByUserProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        var userFromHeadDepartment = await client.CreateEmployee();
        var employeeId = userFromHeadDepartment!.Id;

        var privateProjectWithUserFromDepartment = await client.CreateProject(clientId: clientId);
        var privateProjectId = privateProjectWithUserFromDepartment!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: privateProjectId);

        var publicProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        var anotherPrivateProject = await client.CreateProject(clientId: clientId);

        await client.AuthAsHeadOfDepartment();

        var projectCreatedByHead = await client.CreateProject(clientId: clientId);
        var projectCreatedByHeadId = projectCreatedByHead!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: projectCreatedByHeadId);

        // Act
        var projects = await client.GetProjectsLookupAsync();

        // Assert
        projects.Should().NotBeNull();
        projects.Should().Contain(p => p.Id == privateProjectWithUserFromDepartment.Id);
        projects.Should().Contain(p => p.Id == publicProject!.Id);
        projects.Should().Contain(p => p.Id == projectCreatedByHead.Id);
        projects.Should().NotContain(p => p.Id == anotherPrivateProject!.Id);
    }
}
