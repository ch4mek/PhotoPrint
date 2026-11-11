using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class GetProjectsSortedLookupIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectsSortedLookupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;

    }
    [Fact]
    public async Task SortedLookupProjectsAsyncWhenAuthedAsEmployeeResponseWithProjectsWhereUserIsAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var notBillableProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = notBillableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        await client.CreateProject(clientId: clientId, isBillable: true);

        var expectedProjectNames = new List<string>
        {
            notBillableProject.Name
        };

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsSortedLookupAsync();

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenAuthedAsEmployeeAndOtherUsersIdProvidedResponseWithPublicProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var project = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: headId, projectId: projectId, isTimeBillable: false);

        var publicProject = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var expectedProjectNames = new List<string>
        {
            publicProject!.Name
        };

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsSortedLookupAsync(userId: headId);

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenAuthedAsEmployeeAndHisUserIdProvidedResponseWithProjectsWhereUserIsAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var notBillableProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var projectId = notBillableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        await client.CreateProject(clientId: clientId);

        var expectedProjectNames = new List<string>
        {
            notBillableProject.Name
        };

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsSortedLookupAsync(userId: userId);

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenAuthedAsHeadOfDepartmentResponseWithProjectsWhichContainUserFromSameDepartmentOrCreatedBySelfOrPublicProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var userFromHeadDepartment = await client.CreateEmployee();
        var userId = userFromHeadDepartment!.Id;

        var publicProject = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var projectWithUserFromSameDepartment = await client.CreateProject(clientId: clientId,
            isBillable: false);
        var publicProjectId = projectWithUserFromSameDepartment!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: publicProjectId, isTimeBillable: false);

        await client.AuthAsHeadOfDepartment();

        var projectCreatedByHead = await client.CreateProject(clientId: clientId);

        var expectedProjectNames = new List<string>
        {
            publicProject!.Name,
            projectCreatedByHead!.Name,
            projectWithUserFromSameDepartment.Name
        };

        // Act
        var projects = await client.GetProjectsSortedLookupAsync();

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenAuthedAsGroupManagerResponseWithProjectsWhichContainUserFromSameResourceGroupOrPublicProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(resourceGroupId: 1);
        var userId = user!.Id;

        var publicProject = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        var projectWithUserFromSameResourceGroup = await client.CreateProject(clientId: clientId,
            isBillable: false, isPublic: false);
        var projectId = projectWithUserFromSameResourceGroup!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        await client.AuthAsGroupManager();

        var expectedProjectNames = new List<string>
        {
            publicProject!.Name,
            projectWithUserFromSameResourceGroup.Name,
        };

        // Act
        var projects = await client.GetProjectsSortedLookupAsync();

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenAuthedAsCeoAndUserIdIsProvidedResponseWithProjectsWhereUserIsAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var projectWhereUserIsAssignedTo = await client.CreateProject(clientId: clientId,
            isBillable: false, isPublic: false);
        var projectId = projectWhereUserIsAssignedTo!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId, isTimeBillable: false);

        await client.CreateProject(clientId: clientId);

        var expectedProjectNames = new List<string>
        {
            projectWhereUserIsAssignedTo.Name
        };

        // Act
        var projects = await client.GetProjectsSortedLookupAsync(userId: userId);

        // Assert
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenKeyWordIsNotNullShouldResponseWithProjectsWhereNamesAreMatching()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        const string keyWord = "A";

        var billableProject = await client.CreateProject(clientId: clientId, name: $"{keyWord}e");
        var billableProjectId = billableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: billableProjectId);

        var publicProject = await client.CreateProject(clientId: clientId, name: $"{keyWord}d", isPublic: true,
            isBillable: false);

        var notBillableProject = await client.CreateProject(clientId: clientId, name: $"{keyWord}f",
            isBillable: false,
            isPublic: true);
        var notBillableProjectId = notBillableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: notBillableProjectId, isTimeBillable: false);

        var expectedProjectNames = new List<string>
        {
            billableProject.Name,
            publicProject!.Name
        };

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsSortedLookupAsync(keyWord: keyWord);

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThanOrEqualTo(2);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupProjectsAsyncWhenKeyWordIsNullShouldResponseWithAllProjectsWhereUserIsAssignedToOrPublic()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var billableProject = await client.CreateProject(clientId: clientId);
        var billableProjectId = billableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: billableProjectId);

        var publicProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        var notBillableProject = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);
        var notBillableProjectId = notBillableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: notBillableProjectId, isTimeBillable: false);

        var expectedProjectNames = new List<string>
        {
            billableProject.Name,
            publicProject!.Name,
            notBillableProject.Name
        };

        await client.AuthAsEmployee();

        // Act
        var projects = await client.GetProjectsSortedLookupAsync();

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThanOrEqualTo(3);
        projects.Select(x => x.Name).Intersect(expectedProjectNames).Should().HaveCount(expectedProjectNames.Count);
        projects.Should().BeEquivalentTo(
            projects.OrderBy(x => x.Type).ThenBy(x => x.Id),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupPersonalProjectsAsyncWhenAuthedAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const string keyWord = "test";

        // Act
        var response = await client.GetPersonalProjectsSortedLookupAsHttpMessageAsync(keyWord: keyWord);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SortedLookupPersonalProjectsAsyncWhenKeyWordIsNotNullShouldResponseWithProjectsWhereNamesAreMatchingAndUserIsAssignedTo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var clientId = _factory.ModsenClientId!.Value;

        var userId = _factory.HeadOfDepartmentUserId!.Value;
        const string keyWord = "c";

        var matchingProject = await client.CreateProject(clientId: clientId, isBillable: false,
            name: $"{keyWord}atching");
        var matchingProjectId = matchingProject!.Id;

        var notMatchingProject = await client.CreateProject(clientId: clientId, isBillable: false,
            name: "House");
        var notMatchingProjectId = notMatchingProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: matchingProjectId, isTimeBillable: false);
        await client.CreateProjectAssignment(userId: userId, projectId: notMatchingProjectId, isTimeBillable: false);

        await client.AuthAsHeadOfDepartment();

        // Act
        var projects = await client.GetPersonalProjectsSortedLookupAsync(
            keyWord: keyWord);

        // Assert
        projects.Should().NotBeEmpty();
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
        projects.Any(project => project.Name == matchingProject.Name).Should().BeTrue();
        projects.Any(project => project.Name == notMatchingProject.Name).Should().BeFalse();
        projects.Should().BeEquivalentTo(
            projects
                .OrderBy(project => project.Type)
                .ThenBy(project => project.Name)
                .ToList(),
            options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task SortedLookupPersonalProjectsAsyncWhenKeyWordIsNullShouldResponseWithAllProjectsWhereUserIsAssignedToOrPublic()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.HeadOfDepartmentUserId!.Value;

        var billableProject = await client.CreateProject(clientId: clientId);
        var billableProjectId = billableProject!.Id;

        var notBillableProject = await client.CreateProject(clientId: clientId, isBillable: false, isPublic: false);
        var notBillableProjectId = notBillableProject!.Id;

        await client.CreateProjectAssignment(userId: userId, billableProjectId);
        await client.CreateProjectAssignment(userId: userId, notBillableProjectId, isTimeBillable: false);

        var publicProject = await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);

        var expectedProjects = new List<ProjectOutput>
        {
            billableProject,
            notBillableProject,
            publicProject!
        }.ConvertAll(project => new ProjectSortedLookupOutput
        {
            Id = project.Id,
            Name = project.Name,
            Color = project.Color,
            Client = "Modsen",
            Type = project.IsPublic
                ? PortalProjectTypeNames.Public
                : PortalProjectTypeNames.Private
        });

        await client.AuthAsHeadOfDepartment();

        // Act
        var projects = await client.GetPersonalProjectsSortedLookupAsync();

        // Assert
        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThanOrEqualTo(3);
        projects.IntersectBy(expectedProjects.ConvertAll(x => x.Name), project => project.Name)
            .Should().BeEquivalentTo(expectedProjects);
        projects.Should().BeEquivalentTo(
            projects
                .OrderBy(project => project.Type)
                .ThenBy(project => project.Name)
                .ToList(),
            options => options.WithStrictOrdering());
    }
}
