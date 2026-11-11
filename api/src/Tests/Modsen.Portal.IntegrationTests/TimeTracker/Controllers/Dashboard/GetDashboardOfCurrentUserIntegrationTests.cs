using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.Dashboard;

[Collection("TimeTrackerCollection")]
public class GetDashboardOfCurrentUserIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetDashboardOfCurrentUserIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetDashboardForCurrentUserAndCurrentDateWhenAuthAsCEOShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var result = await client.GetDashboardAsHttpMessageAsync();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDashboardForCurrentUserAndCurrentDateWhenAuthAsCTOShouldReturnForbiddenResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var result = await client.GetDashboardAsHttpMessageAsync();

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDashboardForCurrentUserAndCurrentDateWhenAuthAsEmployeeShouldReturnCurrentUsersDashboard()
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

        await client.CreateProjectAssignment(userId: _factory.CeoUserId!.Value, projectId: secondProjectId);

        await client.AuthAs(createdUser.Email);
        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId),
            await client.TrackFactAsync(projectId: secondProjectId),
            await client.TrackFactAsync(projectId: secondProjectId)
        };

        // Act
        var output = await client.GetDashboardAsync();

        // Assert
        output.Should().NotBeNull();
        output.TrackedInMinutesDay.Should().Be(300);
        output.TrackedInMinutesWeek.Should().Be(300);
        output.TrackedInMinutesMonth.Should().Be(300);
        output.Activities.Items.Count.Should().Be(3);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetDashboardForCurrentMonthWhenAuthAsEmployeeShouldReturnCurrentUsersDashboard()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
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

        await client.CreateProjectAssignment(userId: _factory.CeoUserId!.Value, projectId: secondProjectId);

        await client.AuthAs(createdUser.Email);
        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: firstProjectId, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, date: date),
            await client.TrackFactAsync(projectId: secondProjectId, date: date)
        };

        // Act
        var output = await client.GetDashboardTrackedTimeAsync();

        // Assert
        output.Should().NotBeNull();
        output.TrackedTime.Should().NotBeNull();
        output.TrackedTime.Should().NotBeEmpty();
        output.TrackedTime.FirstOrDefault(x => x.Date == date)?.TotalTrackedTimeInMinutes.Should().Be(300);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
