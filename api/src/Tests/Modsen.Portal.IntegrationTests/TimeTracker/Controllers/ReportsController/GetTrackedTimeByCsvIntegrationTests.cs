using System.Globalization;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

[Collection("TimeTrackerCollection")]
public class GetTrackedTimeByCsvIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string _defaultDateFormat = "dd/MM/yyyy";

    public GetTrackedTimeByCsvIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _factory.CleanTimesheets();
    }

    [Fact]
    public async Task GetTrackedTimeByExcelWhenRequestIsValidResponseStreamWithExcelFile()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var clientId = _factory.ModsenClientId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var date = DateTime.UtcNow;
        
        var createdUser = await client.CreateEmployee();
        var userId = createdUser!.Id;

        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId);

        var firstEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = firstProject.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 1 - First task",
            FactTrackedInMinutes = 480
        };

        var secondEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProject.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            FactTrackedInMinutes = 180
        };

        var thirdEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProject.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Third task",
            FactTrackedInMinutes = 120
        };

        var firstHeadTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProject.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - First task 2",
            FactTrackedInMinutes = 100
        };

        var secondHeadTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProject.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            FactTrackedInMinutes = 150
        };

        await client.AuthAs(createdUser.Email);

        await client.CreateTimesheetFact(firstEmployeeTrackedTask);
        await client.CreateTimesheetFact(secondEmployeeTrackedTask);
        await client.CreateTimesheetFact(thirdEmployeeTrackedTask);

        await client.AuthAsHeadOfDepartment();

        await client.CreateTimesheetFact(firstHeadTrackedTask);
        await client.CreateTimesheetFact(secondHeadTrackedTask);

        var expectedTable =
            "Date,Description,Time (h)\r\n" +
            $"{date.ToString(_defaultDateFormat, CultureInfo.InvariantCulture)},Project 1 - First task,8:00\r\n" +
            $"{date.ToString(_defaultDateFormat, CultureInfo.InvariantCulture)},Project 2 - Second task,3:00\r\n" +
            $"{date.ToString(_defaultDateFormat, CultureInfo.InvariantCulture)},Project 2 - Third task,2:00\r\n" +
            $"{date.ToString(_defaultDateFormat, CultureInfo.InvariantCulture)},Project 2 - First task 2,1:40\r\n" +
            $"{date.ToString(_defaultDateFormat, CultureInfo.InvariantCulture)},Project 2 - Second task,2:30\r\n";

        await client.AuthAsHeadOfDepartment();

        // Act
        await using var response = await client.GetReportsAsCsvAsync();

        // Assert
        using var reader = new StreamReader(response);
        var textResult = await reader.ReadToEndAsync(_factory.CancellationToken);
        textResult.Should().Be(expectedTable);
    }
}
