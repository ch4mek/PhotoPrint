using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetTimeStackIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetTimeStackIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetTimeStackAsyncWhenAuthAsEmployeeShouldReturnForbiddenResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var result = await client.GetTimeStackAsHttpMessageAsync();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTimeStackAsyncWhenAuthAsCtoWithDepartmentShouldReturnAllUsersFromDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        const string departmentId = "Python";

        var firstUser = await client.CreateEmployee(departmentId: departmentId);
        var firstUserId = firstUser!.Id;

        await client.CreateEmployee(departmentId: departmentId);

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: firstUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(60));

        await client.TrackFactAsync(userId: firstUserId, projectId: firstProjectId, date: date);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId, date: date);
        await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId, date: date);

        // Act
        var result = await client.GetTimeStackAsync(
            departmentId: departmentId,
            year: date.Year,
            month: date.Month);

        // Assert
        result.Should().NotBeNull();
        result.Any(s => s.ReportForDate == date && s.TotalTrackedInMinutes == 300).Should().BeTrue();
    }

    [Fact]
    public async Task GetTimeStackAsyncWhenAuthAsHeadOfDepartmentShouldReturnAllUsersFromDepartment()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstUser = await client.CreateEmployee(departmentId: "Net");
        var firstUserId = firstUser!.Id;

        var secondUser = await client.CreateEmployee(departmentId: "Python");
        var secondUserId = secondUser!.Id;

        var firstClient = await client.CreateClientAsync();
        var firstClientId = firstClient!.Id;

        var secondClient = await client.CreateClientAsync();
        var secondClientId = secondClient!.Id;

        var firstProject = await client.CreateProject(clientId: firstClientId);
        var firstProjectId = firstProject!.Id;

        var secondProject = await client.CreateProject(clientId: secondClientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: secondUserId, projectId: firstProjectId);
        await client.CreateProjectAssignment(userId: firstUserId, projectId: secondProjectId);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: secondUserId, projectId: firstProjectId),
            await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId),
            await client.TrackFactAsync(userId: firstUserId, projectId: secondProjectId)
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var result = await client.GetTimeStackAsync();

        // Assert
        result.Should().NotBeNull();
        result.Any(s => s.ReportForDate == DateOnly.FromDateTime(DateTime.UtcNow) && s.TotalTrackedInMinutes == 200).Should().BeTrue();

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
