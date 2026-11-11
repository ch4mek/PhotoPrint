using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class GetProjectsByUserIdIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetProjectsByUserIdIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenAuthAsHeadAndTryToGetAnotherDepartmentProjectsShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Js");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetProjectsByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenAuthAsGMAndTryToGetAnotherDepartmentProjectsShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee(departmentId: "Js");
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetProjectsByUserIdAsHttpMessageAsync(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenRequestIsValidShouldReturnOnlyPrivateProjectsAssignedToUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdEmployee = await client.CreateEmployee();
        var userId = createdEmployee!.Id;

        await client.CreateProject(clientId: clientId, isPublic: true, isBillable: false);
        await client.CreateProject(clientId: clientId, isPublic: false, isBillable: true);

        var createdPrivateProjectWithEmployee = await client.CreateProject(clientId: clientId);
        var projectWithEmployeeId = createdPrivateProjectWithEmployee!.Id;

        await client.CreateProjectAssignment(
            userId: userId, projectId: projectWithEmployeeId);

        // Act
        var response = await client.GetProjectsByUserIdAsync(userId);

        // Assert
        response.Should().ContainSingle(x => x.Id == projectWithEmployeeId);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenAuthAsHeadOfPmoDepartmentShouldNotReturnUnassignedPrivateProjects()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;

        var createdEmployee = await client.CreateEmployee(departmentId: "PMO");
        var employeeId = createdEmployee!.Id;

        var createdPrivateProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId,
            isBillable: false);
        var projectId = createdPrivateProject!.Id;

        const int fte = 1;
        const string assignmentSeniorityLevel = "SeniorL3";
        var assignment = await client.CreateProjectAssignment(userId: employeeId, projectId: projectId,
            fte: fte,
            seniorityLevelId: assignmentSeniorityLevel);

        var newDateFrom = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10);
        var newDateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

        var updateAssignInput = new UpdateUserProjectAssignmentInput(
            Id: assignment!.Id,
            UserId: createdEmployee.Id,
            ProjectId: createdPrivateProject.Id,
            IsTimeOnProjectBillable: assignment.IsTimeOnProjectBillable,
            Fte: fte,
            UserOnProjectSeniorityLevelId: assignmentSeniorityLevel,
            UserOnProjectFrom: newDateFrom,
            UserOnProjectTo: newDateTo);

        await client.UpdateProjectAssignmentAsHttpMessageAsync(updateAssignInput);

        var intervalFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        var intervalTo = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7);

        // Act
        var response = await client.GetProjectsByUserIdAsync(userId: employeeId,
            onlyManaged: true,
            trackIntervalFrom: intervalFrom,
            trackIntervalTo: intervalTo);

        // Assert
        response.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenEmployeeIsFromPmoDepartmentShouldReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdPmoEmployee = await client.CreateEmployee(departmentId: "PMO");
        var managerId = createdPmoEmployee!.Id;

        var createdEmployee = await client.CreateEmployee();
        var employeeId = createdEmployee!.Id;

        var createdPrivateProject = await client.CreateProject(clientId: clientId, managerId: managerId);
        var projectId = createdPrivateProject!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        // Act
        await client.AuthAs(createdPmoEmployee.Email);

        var response = await client.GetProjectsByUserIdAsHttpMessageAsync(
            userId: employeeId,
            onlyManaged: true);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenHeadOfDepartmentIsManagerOfProjectWhereUserHasTrackedTimeShouldReturnOnlyTimesheetsFromManagedProject()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var createdMobileEmployee = await client.CreateEmployee(departmentId: "Mobile");
        var employeeId = createdMobileEmployee!.Id;
        var employeeEmail = createdMobileEmployee.Email;

        var createdProjectForHeadToManage = await client.CreateProject(
            clientId: clientId,
            managerId: pmoHeadId);
        var projectId = createdProjectForHeadToManage!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        await client.AuthAs(employeeEmail);
        await client.TrackFactAsync(projectId: projectId, trackedInMinutes: 120);

        await client.AuthAs(pmoHeadEmail);

        // Act
        var response = await client.GetProjectsByUserIdAsync(employeeId);

        // Assert
        response.Should().ContainSingle(x => x.Id == projectId);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenAuthAsHeadIfEmployeeHasTrackedTimeOnlyOnPublicProjectShouldReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var createdEmployee = await client.CreateEmployee(departmentId: "PMO");
        var employeeId = createdEmployee!.Id;

        var createdProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId,
            isPublic: true,
            isBillable: false);
        var projectId = createdProject!.Id;

        await client.TrackFactAsync(projectId: projectId, userId: employeeId);

        await client.AuthAs(pmoHeadEmail);

        // Act
        var response = await client.GetProjectsByUserIdAsHttpMessageAsync(userId: employeeId, onlyManaged: true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenAuthedHeadIsManagerOfPublicProjectAndEmployeeDoesNotHaveTrackedTimeOnPublicManagedProjectShouldReturnOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var createdEmployee = await client.CreateEmployee();
        var employeeId = createdEmployee!.Id;

        await client.CreateProject(clientId: clientId, managerId: pmoHeadId, isPublic: true, isBillable: false);

        var secondCreatedProject = await client.CreateProject(clientId: clientId,
            isPublic: true,
            isBillable: false);
        var secondProjectId = secondCreatedProject!.Id;

        await client.TrackFactAsync(userId: employeeId, projectId: secondProjectId);

        await client.AuthAs(pmoHeadEmail);

        // Act
        var response = await client.GetProjectsByUserIdAsHttpMessageAsync(userId: employeeId,
            onlyManaged: true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjectsByUserIdWhenAuthedHeadIsProjectManagerAndRequestingOnlyPublicProjectsShouldReturnExpectedResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var pmoHeadId = _factory.PmoHeadOfDepartmentUserId!.Value;
        var pmoHeadEmail = _factory.PmoHeadOfDepartmentEmail!;

        var createdEmployee = await client.CreateEmployee(departmentId: "PMO");
        var employeeId = createdEmployee!.Id;

        var publicProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId,
            isPublic: true,
            isBillable: false);
        var publicProjectId = publicProject!.Id;

        var privateProject = await client.CreateProject(clientId: clientId, managerId: pmoHeadId,
            isPublic: false,
            isBillable: false);
        var privateProjectId = privateProject!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: privateProjectId);
        await client.TrackFactAsync(userId: employeeId, projectId: publicProjectId);

        await client.AuthAs(pmoHeadEmail);

        var expectedResponse = new List<ProjectLookupOutput>
        {
            new()
            {
                Id = publicProject.Id,
                Name = publicProject.Name,
                ClientName = "Modsen",
                IsBillable = publicProject.IsBillable,
                IsCurrentUserAbleToSeeBillableTrackedTime = true,
                Color = publicProject.Color
            }
        };

        // Act
        var response = await client.GetProjectsByUserIdAsync(userId: employeeId,
            onlyManaged: true,
            isPublic: true);

        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetProjectsByUserIdWithTrackIntervalReturnExpectedResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var trackIntervalFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));
        var trackIntervalTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var projectWithAssignInsideInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignOutsideLeftInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignOutsideRightInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignIntersectingLeftInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignIntersectingRightInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignCoveringInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignTouchingLeftInterval = await client.CreateProject(clientId: clientId);

        var projectWithAssignTouchingRightInterval = await client.CreateProject(clientId: clientId);

        var projectWithRightNullAssignTouchingRightInterval = await client.CreateProject(clientId: clientId);

        var projectWithRightNullAssignOutsideInterval = await client.CreateProject(clientId: clientId);

        var projectWithRightNullAssignIntersectingInterval = await client.CreateProject(clientId: clientId);

        var projectWithRightNullAssignCoveringInterval = await client.CreateProject(clientId: clientId);


        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignInsideInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-4)),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignIntersectingLeftInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-50)),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignIntersectingRightInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignCoveringInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-50)),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithRightNullAssignIntersectingInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            onProjectTo: null);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignTouchingLeftInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            onProjectTo: trackIntervalFrom);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignTouchingRightInterval!.Id,
            onProjectFrom: trackIntervalTo,
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithRightNullAssignTouchingRightInterval!.Id,
            onProjectFrom: trackIntervalTo,
            onProjectTo: null);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignOutsideLeftInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-50)),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignOutsideRightInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithRightNullAssignOutsideInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            onProjectTo: null);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithRightNullAssignCoveringInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            onProjectTo: null);

        // Act
        var projects = await client.GetProjectsByUserIdAsync(userId: userId, isPublic: false,
            trackIntervalFrom: trackIntervalFrom,
            trackIntervalTo: trackIntervalTo);

        // Assert
        projects.Should().Contain(p => p.Id == projectWithAssignInsideInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithAssignIntersectingLeftInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithAssignIntersectingRightInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithAssignCoveringInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithRightNullAssignIntersectingInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithRightNullAssignCoveringInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithAssignTouchingLeftInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithAssignTouchingRightInterval.Id);
        projects.Should().Contain(p => p.Id == projectWithRightNullAssignTouchingRightInterval.Id);

        projects.Should().NotContain(p => p.Id == projectWithAssignOutsideLeftInterval.Id);
        projects.Should().NotContain(p => p.Id == projectWithAssignOutsideRightInterval.Id);
        projects.Should().NotContain(p => p.Id == projectWithRightNullAssignOutsideInterval.Id);
    }
}
