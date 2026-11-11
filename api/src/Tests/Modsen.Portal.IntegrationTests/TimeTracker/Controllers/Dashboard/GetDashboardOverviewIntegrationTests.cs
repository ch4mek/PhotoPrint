using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetDashboardOverviewIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetDashboardOverviewIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetOverviewAsyncWhenAuthAsHeadOfDepartmentShouldReturnOKWithFinalInfo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: userId, projectId: firstProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId)
        };

        await client.TrackBillableAsync(projectId: firstProjectId, userId: userId);

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetDashboardOverviewAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalFactTime.Should().Be(300);
        result.TotalBillableTime.Should().Be(100);
        result.TopBillableClientId.Should().Be(firstProject.ClientId);
        result.TopFactClientId.Should().Be(secondProject.ClientId);
        result.TopBillableProjectId.Should().Be(firstProject.Id);
        result.TopFactProjectId.Should().Be(secondProject.Id);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetOverviewAsyncWhenAuthAsCeoWithDepartmentShouldReturnOKWithFinalInfo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        const string departmentId = "Python";

        var createdUser = await client.CreateEmployee(departmentId: departmentId);
        var userId = createdUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: userId, projectId: firstProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId)
        };

        await client.TrackBillableAsync(projectId: firstProjectId, userId: userId);

        // Act
        var result = await client.GetDashboardOverviewAsync(departmentId: departmentId);

        // Assert
        result.TotalFactTime.Should().Be(300);
        result.TotalBillableTime.Should().Be(100);
        result.TopBillableClientId.Should().Be(firstProject.ClientId);
        result.TopFactClientId.Should().Be(secondProject.ClientId);
        result.TopBillableProjectId.Should().Be(firstProject.Id);
        result.TopFactProjectId.Should().Be(secondProject.Id);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetOverviewAsyncWhenAuthAsCtoWithDepartmentShouldReturnOKWithFinalInfo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Java";

        var createdUser = await client.CreateEmployee(departmentId: departmentId);
        var userId = createdUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: userId, projectId: firstProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId)
        };

        await client.TrackBillableAsync(projectId: firstProjectId, userId: userId);

        // Act
        var result = await client.GetDashboardOverviewAsync(departmentId: departmentId);

        // Assert
        result.Should().NotBeNull();
        result.TotalFactTime.Should().Be(300);
        result.TotalBillableTime.Should().Be(100);
        result.TopBillableClientId.Should().Be(firstProject.ClientId);
        result.TopFactClientId.Should().Be(secondProject.ClientId);
        result.TopBillableProjectId.Should().Be(firstProject.Id);
        result.TopFactProjectId.Should().Be(secondProject.Id);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetOverviewAsyncWhenAuthAsGroupManagerShouldReturnOKWithFinalInfo()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var createdUser = await client.CreateEmployee(resourceGroupId: 1);
        var userId = createdUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: userId, projectId: firstProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId),
            await client.TrackFactAsync(userId: userId, projectId: secondProjectId)
        };

        await client.TrackBillableAsync(projectId: firstProjectId, userId: userId);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var result = await client.GetDashboardOverviewAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalFactTime.Should().Be(300);
        result.TotalBillableTime.Should().Be(100);
        result.TopBillableClientId.Should().Be(firstProject.ClientId);
        result.TopFactClientId.Should().Be(secondProject.ClientId);
        result.TopBillableProjectId.Should().Be(firstProject.Id);
        result.TopFactProjectId.Should().Be(secondProject.Id);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetOverviewAsyncWhenAuthAsEmployeeShouldReturnForbiddenResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetDashboardOverviewAsHttpMessageAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
