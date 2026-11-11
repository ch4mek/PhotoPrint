using System.Globalization;
using System.Net;
using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.ReportsController;

public class GetTrackedTimeByFileIntegrationTests : IClassFixture<ReportsIntegrationTestFactory>
{
    private const string s_folderName = "application-files-tests";

    private readonly ReportsIntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string _summaryReportWorksheet = "Summary Report";

    public GetTrackedTimeByFileIntegrationTests(ReportsIntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetTrackedTimeByFileWhenRequestIsValidResponseStreamWithExcelFile()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var reportName = GetReportName(DocumentType.ExcelAccount);

        var date = DateTime.UtcNow.AddYears(2);
        var monthName = CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetMonthName(date.Month);
        monthName = monthName[0].ToString().ToUpper() + monthName.Substring(1);

        var format = nameof(DocumentType.ExcelAccount);

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        await client.AuthAsCeo();
        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId);

        var firstEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = firstProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 1 - First task",
            FactTrackedInMinutes = 480
        };

        var secondEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            FactTrackedInMinutes = 180
        };

        var thirdEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Third task",
            FactTrackedInMinutes = 120
        };

        var firstHeadTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 1 - First task",
            FactTrackedInMinutes = 100
        };

        var secondHeadTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            FactTrackedInMinutes = 150
        };

        await client.AuthAsEmployee();
        await client.CreateTimesheetFact(firstEmployeeTrackedTask);
        await client.CreateTimesheetFact(secondEmployeeTrackedTask);
        await client.CreateTimesheetFact(thirdEmployeeTrackedTask);

        await client.AuthAsHeadOfDepartment();
        await client.CreateTimesheetFact(firstHeadTrackedTask);
        await client.CreateTimesheetFact(secondHeadTrackedTask);

        var expectedTable = new List<string>
        {
            $"{firstProject.Name}/Modsen",
            "",
            "",
            "",
            $"Test Employee - {monthName}",
            "",
            "",
            "",
            "#",
            "Задача",
            "Департамент",
            "Время",
            "1.00",
            "Project 1 - First task",
            ".NET",
            "8.00",
            "",
            "",
            "Итого",
            "8.00",
            "",
            "",
            "",
            "",
            $"{secondProject.Name}/Modsen",
            "",
            "",
            "",
            $"Test Employee - {monthName}",
            "",
            "",
            "",
            "#",
            "Задача",
            "Департамент",
            "Время",
            "1.00",
            "Project 2 - Second task",
            ".NET",
            "3.00",
            "2.00",
            "Project 2 - Third task",
            ".NET",
            "2.00",
            "",
            "",
            "Итого",
            "5.00",
            $"Test HeadOfDepartment - {monthName}",
            "",
            "",
            "",
            "#",
            "Задача",
            "Департамент",
            "Время",
            "1.00",
            "Project 1 - First task",
            ".NET",
            "1.67",
            "2.00",
            "Project 2 - Second task",
            ".NET",
            "2.50",
            "",
            "",
            "Итого",
            "4.17",
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetReportsWithFormatAsync(format: format, year: date.Year,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();

        using var stream = await cloudStorageManager.GetFileContentAsync(s_folderName, fileName,
            _factory.CancellationToken);

        using var workbook = new XLWorkbook(stream);
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();

        var actualTable = worksheet.Cells("A1:D17").Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture))
            .ToList();

        actualTable.Should().BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetTrackedTimeByFileWhenFilterByBillableHoursResponseStreamWithExcelFile()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        var reportName = GetReportName(DocumentType.ExcelAccount);

        var date = DateTime.UtcNow.AddYears(3);
        var monthName = CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetMonthName(date.Month);
        monthName = monthName[0].ToString().ToUpper() + monthName.Substring(1);

        var format = nameof(DocumentType.ExcelAccount);

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        await client.AuthAsCeo();
        var firstProject = await client.CreateProject(clientId: clientId);
        var firstProjectId = firstProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: firstProjectId);

        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);
        var secondProjectId = secondProject!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: secondProjectId);
        await client.CreateProjectAssignment(userId: headId, projectId: secondProjectId);

        await client.UpdateUserPermissionAssignments(
            userId: userId,
            resources: new Dictionary<string, string>
            {
                { firstProjectId.ToString(CultureInfo.InvariantCulture), firstProject.Name },
                { secondProjectId.ToString(CultureInfo.InvariantCulture), secondProject.Name }
            });

        var firstEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = firstProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 1 - First task",
            FactTrackedInMinutes = 500,
        };

        var secondEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            FactTrackedInMinutes = 200,
        };

        var thirdEmployeeTrackedTask = new UpsertFactDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Third task",
            FactTrackedInMinutes = 150,
        };

        var firstEmployeeTrackedTaskBillable = new UpsertBillableDto
        {
            ProjectId = firstProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 1 - First task",
            BillableTrackedInMinutes = 480
        };

        var secondEmployeeTrackedTaskBillable = new UpsertBillableDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            BillableTrackedInMinutes = 180
        };

        var thirdEmployeeTrackedTaskBillable = new UpsertBillableDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Third task",
            BillableTrackedInMinutes = 120
        };

        var firstHeadTrackedTaskBillable = new UpsertBillableDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 1 - First task",
            BillableTrackedInMinutes = 100
        };

        var secondHeadTrackedTaskBillable = new UpsertBillableDto
        {
            ProjectId = secondProjectId,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Project 2 - Second task",
            BillableTrackedInMinutes = 150
        };


        await client.AuthAsEmployee();
        await client.CreateTimesheetFact(firstEmployeeTrackedTask);
        await client.CreateTimesheetFact(secondEmployeeTrackedTask);
        await client.CreateTimesheetFact(thirdEmployeeTrackedTask);
        await client.TrackBillableAsync(firstEmployeeTrackedTaskBillable);
        await client.TrackBillableAsync(secondEmployeeTrackedTaskBillable);
        await client.TrackBillableAsync(thirdEmployeeTrackedTaskBillable);

        await client.AuthAsHeadOfDepartment();
        await client.TrackBillableAsync(firstHeadTrackedTaskBillable);
        await client.TrackBillableAsync(secondHeadTrackedTaskBillable);

        var expectedTable = new List<string>
        {
            $"{firstProject.Name}/Modsen",
            "",
            "",
            "",
            $"Test Employee - {monthName}",
            "",
            "",
            "",
            "#",
            "Задача",
            "Департамент",
            "Время",
            "1.00",
            "Project 1 - First task",
            ".NET",
            "8.00",
            "",
            "",
            "Итого",
            "8.00",
            "",
            "",
            "",
            "",
            $"{secondProject.Name}/Modsen",
            "",
            "",
            "",
            $"Test Employee - {monthName}",
            "",
            "",
            "",
            "#",
            "Задача",
            "Департамент",
            "Время",
            "1.00",
            "Project 2 - Second task",
            ".NET",
            "3.00",
            "2.00",
            "Project 2 - Third task",
            ".NET",
            "2.00",
            "",
            "",
            "Итого",
            "5.00",
            $"Test HeadOfDepartment - {monthName}",
            "",
            "",
            "",
            "#",
            "Задача",
            "Департамент",
            "Время",
            "1.00",
            "Project 1 - First task",
            ".NET",
            "1.67",
            "2.00",
            "Project 2 - Second task",
            ".NET",
            "2.50",
            "",
            "",
            "Итого",
            "4.17",
        };

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetReportsWithFormatAsync(format: format, year: date.Year,
            trackedTimeType: TrackedTimeType.Billable);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();

        using var stream = await cloudStorageManager.GetFileContentAsync(s_folderName, fileName,
            _factory.CancellationToken);

        using var workbook = new XLWorkbook(stream);
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();

        var actualTable = worksheet.Cells("A1:D17")
            .Select(cell =>
                cell.Value.IsNumber
                    ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                    : cell.Value.ToString(CultureInfo.InvariantCulture)).ToList();

        actualTable.Should().BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetTrackedTimeInFileWhenUserIdAndPdfFormatSpecifiedShouldReturnPdfFile()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        var format = nameof(DocumentType.Pdf);

        var reportName = GetReportName(DocumentType.Pdf);

        var date = DateTime.UtcNow.Date;

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var trackedTask = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "First task",
            FactTrackedInMinutes = 480,
        };

        await client.AuthAsEmployee();
        await client.CreateFactTimesheetAsHttpMessageAsync(trackedTask);

        await client.AuthAsCeo();

        // Act

        var response = await client.GetReportsWithFormatAsync(format: format, userId: userId, year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();

        var result = await cloudStorageManager.FileExistsByListingAsync(s_folderName, fileName,
            _factory.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetTrackedTimeInFileWhenProjectIdAndWordFormatSpecifiedShouldReturnWordFile()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        var format = nameof(DocumentType.Word);

        var reportName = "123"; // GetMockedReportName(DocumentType.Word);

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        var date = DateTime.UtcNow.Date;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var trackedTask = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "First task",
            FactTrackedInMinutes = 480,
        };

        await client.AuthAsEmployee();
        await client.CreateFactTimesheetAsHttpMessageAsync(trackedTask);

        // Act
        await client.AuthAsCeo();

        var response = await client.GetReportsWithFormatAsync(format: format, projectId: projectId, year: date.Year,
            month: date.Month, trackedTimeType: TrackedTimeType.Fact);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();
        var result = await cloudStorageManager.FileExistsByListingAsync(s_folderName, fileName,
            _factory.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetTrackedTimeInFileWhenClientIdAndPdfFormatSpecifiedShouldReturnPdfFile()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var format = nameof(DocumentType.Pdf);

        var reportName = GetReportName(DocumentType.Pdf);

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        await httpClient.AuthAsCeo();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var project = await httpClient.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        var trackedTask = new UpsertFactDto
        {
            ProjectId = project.Id, ReportForDate = date, TaskName = "First task", FactTrackedInMinutes = 480,
        };

        await httpClient.AuthAsEmployee();
        await httpClient.CreateFactTimesheetAsHttpMessageAsync(trackedTask);

        // Act
        await httpClient.AuthAsCeo();

        var response = await httpClient.GetReportsWithFormatAsync(format: format, clientId: clientId, year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();
        var result = await cloudStorageManager.FileExistsByListingAsync(s_folderName, fileName,
            _factory.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetTrackedTimeInFileWhenWordFormatAndPeriodOnlySpecifiedShouldReturnWordFile()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var userId = _factory.EmployeeUserId!.Value;
        var format = nameof(DocumentType.Word);

        var reportName = GetReportName(DocumentType.Word);

        var date = DateTime.UtcNow.Date;

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        await httpClient.AuthAsCeo();

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var project = await httpClient.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await httpClient.CreateProjectAssignment(userId: userId, projectId: projectId);

        var trackedTask = new UpsertFactDto
        {
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "First task",
            FactTrackedInMinutes = 480,
        };

        await httpClient.AuthAsEmployee();
        await httpClient.CreateFactTimesheetAsHttpMessageAsync(trackedTask);

        // Act
        await httpClient.AuthAsCeo();

        var response = await httpClient.GetReportsWithFormatAsync(format: format, year: date.Year, month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();
        var result = await cloudStorageManager.FileExistsByListingAsync(s_folderName, fileName,
            _factory.CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetTrackedTimeByFileWhenProjectDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetReportsAsExcelAsync(projectId: 99999);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTrackedTimeByFileWhenNoTrackedTimeForPeriodResponseExcelWithoutTrackedTime()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();
        var clientId = _factory.ModsenClientId!.Value;
        var userId = _factory.EmployeeUserId!.Value;

        var reportName = GetReportName(DocumentType.ExcelAccount);

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        var date = DateTime.UtcNow.Date;

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId, projectId: projectId);

        var expectedTable = new List<string> { $"{project.Name}/" };

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetReportsAsExcelAsync(year: date.Year, projectId: projectId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();
        using var stream = await cloudStorageManager.GetFileContentAsync(s_folderName, fileName,
            _factory.CancellationToken);

        using var workbook = new XLWorkbook(stream);
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();
        var actualTable = worksheet.Cells("A1:A1").Select(cell =>
            cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString("0.00", CultureInfo.InvariantCulture)
                : cell.Value.ToString(CultureInfo.InvariantCulture)).ToList();

        actualTable.Should().BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task GetTrackedTimeInFileWhenPerUserExportEnabledShouldReturnZipArchive()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId1 = _factory.EmployeeUserId!.Value;
        var userId2 = _factory.AdminUserId!.Value;
        var clientId = _factory.ModsenClientId!.Value;
        var format = nameof(DocumentType.Pdf);

        var reportName = GetReportArchiveName(DocumentType.Pdf);

        var date = DateTime.UtcNow.Date;

        var fileGuid = Guid.CreateVersion7();

        _factory.GuidGeneratorMock.Setup(x => x.Create())
            .Returns(fileGuid);

        var fileId = $"{fileGuid:N}";
        var fileName = $"{fileId}{Path.GetExtension(reportName)}";

        await client.AuthAsCeo();
        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: userId1, projectId: projectId);
        await client.CreateProjectAssignment(userId: userId2, projectId: projectId);

        var trackedTask1 = new UpsertFactDto
        {
            UserId = userId1,
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "First task",
            FactTrackedInMinutes = 480
        };

        var trackedTask2 = new UpsertFactDto
        {
            UserId = userId2,
            ProjectId = project.Id,
            ReportForDate = DateOnly.FromDateTime(date),
            TaskName = "Second task",
            FactTrackedInMinutes = 120,
        };

        await client.AuthAsCeo();
        await client.CreateFactTimesheetAsHttpMessageAsync(trackedTask1);
        await client.CreateFactTimesheetAsHttpMessageAsync(trackedTask2);

        // Act
        var response = await client.GetReportsWithFormatAsync(
            format: format,
            year: date.Year,
            trackedTimeType: TrackedTimeType.Fact,
            isPerUserExport: true);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var cloudStorageManager = scope.ServiceProvider.GetRequiredService<ICloudStorageManager>();

        var result = await cloudStorageManager.FileExistsByListingAsync(s_folderName, fileName,
            _factory.CancellationToken);

        result.Should().BeTrue();
    }

    private string GetReportName(DocumentType documentType)
    {
        var extension = documentType.GetExtensionWithDot();
        var reportFileNameDto = $"test-report{extension}";

        return reportFileNameDto;
    }

    private string GetReportArchiveName(DocumentType documentType)
    {
        var extension = documentType.GetExtensionWithDot();
        var reportFileNameDto = $"test-report{extension}.zip";

        return reportFileNameDto;
    }
}
