using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ProjectsController;

[Collection("TimeTrackerCollection")]
public class GetUserProjectsWithDisabledDaysIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetUserProjectsWithDisabledDaysIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserProjectsWithDisabledDaysWhenAuthAsHeadAndTryToGetAnotherDepartmentProjectsShouldReturnForbidden()
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
        var response = await client.GetProjectsWithDisabledDaysByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProjectsWithDisabledDaysWhenAuthAsGMAndTryToGetAnotherResourceGRoupProjectsShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var user = await client.CreateEmployee();
        var userId = user!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetProjectsWithDisabledDaysByUserIdAsHttpMessageAsync(userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUserProjectsWithDisabledDaysWhenRequestIsValidShouldReturnPrivateProjectsAssignedToUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdEmployee = await client.CreateEmployee();
        var employeeId = createdEmployee!.Id;

        await client.CreateProject(clientId: clientId, isBillable: false, isPublic: true);

        await client.CreateProject(clientId: clientId, isBillable: true,
            isPublic: false);

        var privateProjectWithEmployee = await client.CreateProject(clientId: clientId, isBillable: true,
            isPublic: false);
        var privateProjectWithEmployeeId = privateProjectWithEmployee!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: privateProjectWithEmployeeId,
            onProjectFrom: new DateOnly(2024, 1, 1));

        var timeTrackIntervalFrom = new DateOnly(2024, 1, 1);
        var timeTrackIntervalTo = new DateOnly(2024, 2, 1);

        // Act
        var response = await client.GetProjectsWithDisabledDaysByUserIdAsync(
            userId: employeeId,
            trackIntervalFrom: timeTrackIntervalFrom,
            trackIntervalTo: timeTrackIntervalTo);

        // Assert
        response.Should().NotBeNull();
        response.Should().ContainSingle(x => x.Project.Id == privateProjectWithEmployeeId);
        response[0].DisabledDates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserProjectsWithDisabledDaysWhenEmployeeIsFromPmoDepartmentShouldReturnOkResult()
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
        var privateProjectId = createdPrivateProject!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: privateProjectId,
            onProjectFrom: new DateOnly(2024, 1, 2));

        var timeTrackIntervalFrom = new DateOnly(2024, 1, 1);
        var timeTrackIntervalTo = new DateOnly(2024, 2, 1);

        await client.AuthAs(createdPmoEmployee.Email);

        // Act
        var projects = await client.GetProjectsWithDisabledDaysByUserIdAsync(
            userId: employeeId,
            onlyManaged: true,
            trackIntervalFrom: timeTrackIntervalFrom,
            trackIntervalTo: timeTrackIntervalTo);

        // Assert
        projects.Should().NotBeEmpty();
        projects[0].DisabledDates.Should().Contain(new DateOnly(2024, 1, 1));
    }

    [Fact]
    public async Task GetUserProjectsWithDisabledDaysWithTrackIntervalReturnExpectedResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;

        var createdEmployee = await client.CreateEmployee();
        var userId = createdEmployee!.Id;

        var trackIntervalFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5));
        var trackIntervalTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        var utcNow = DateOnly.FromDateTime(DateTime.UtcNow);

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
            onProjectTo: utcNow);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignIntersectingLeftInterval!.Id,
            onProjectFrom: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-50)),
            onProjectTo: utcNow);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignIntersectingRightInterval!.Id,
            onProjectFrom: utcNow,
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignCoveringInterval!.Id,
            onProjectFrom: utcNow.AddDays(-50),
            onProjectTo: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithRightNullAssignIntersectingInterval!.Id,
            onProjectFrom: utcNow.AddDays(-1),
            onProjectTo: null);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithRightNullAssignCoveringInterval!.Id,
            onProjectFrom: utcNow.AddDays(-30),
            onProjectTo: null);

        await client.CreateProjectAssignment(userId: userId,
            projectId: projectWithAssignTouchingLeftInterval!.Id,
            onProjectFrom: utcNow.AddDays(-30),
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

        // Act
        var projects = await client.GetProjectsWithDisabledDaysByUserIdAsync(
            userId: userId,
            trackIntervalTo: trackIntervalTo,
            trackIntervalFrom: trackIntervalFrom);

        // Assert
        projects.Should().Contain(p => p.Project.Id == projectWithAssignInsideInterval.Id);
        var projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithAssignInsideInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo([trackIntervalFrom, trackIntervalTo]);
        projects.Should().Contain(p => p.Project.Id == projectWithAssignIntersectingLeftInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithAssignIntersectingLeftInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo([trackIntervalTo]);
        projects.Should().Contain(p => p.Project.Id == projectWithAssignIntersectingRightInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithAssignIntersectingRightInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo(CreateDateRange(trackIntervalFrom, utcNow.AddDays(-1)));
        projects.Should().Contain(p => p.Project.Id == projectWithAssignCoveringInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithAssignCoveringInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEmpty();
        projects.Should().Contain(p => p.Project.Id == projectWithRightNullAssignIntersectingInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithRightNullAssignIntersectingInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo(CreateDateRange(trackIntervalFrom, utcNow.AddDays(-2)));
        projects.Should().Contain(p => p.Project.Id == projectWithRightNullAssignCoveringInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithRightNullAssignCoveringInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEmpty();
        projects.Should().Contain(p => p.Project.Id == projectWithAssignTouchingLeftInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithAssignTouchingLeftInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo(CreateDateRange(trackIntervalFrom.AddDays(1), trackIntervalTo));
        projects.Should().Contain(p => p.Project.Id == projectWithAssignTouchingRightInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithAssignTouchingRightInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo(CreateDateRange(trackIntervalFrom, trackIntervalTo.AddDays(-1)));
        projects.Should().Contain(p => p.Project.Id == projectWithRightNullAssignTouchingRightInterval.Id);
        projectWithDisabledDaysDto = projects.First(p => p.Project.Id == projectWithRightNullAssignTouchingRightInterval.Id);
        projectWithDisabledDaysDto.DisabledDates.Should().BeEquivalentTo(CreateDateRange(trackIntervalFrom, trackIntervalTo.AddDays(-1)));

        projects.Should().NotContain(p => p.Project.Id == projectWithAssignOutsideLeftInterval.Id);
        projects.Should().NotContain(p => p.Project.Id == projectWithAssignOutsideRightInterval.Id);
        projects.Should().NotContain(p => p.Project.Id == projectWithRightNullAssignOutsideInterval.Id);
    }

    public DateOnly[] CreateDateRange(DateOnly startDate, DateOnly endDate)
    {
       return Enumerable.Range(0, endDate.DayNumber - startDate.DayNumber + 1)
            .Select(startDate.AddDays)
            .ToArray();
    }
}
