using System.Globalization;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

namespace Modsen.Portal.Excel.UnitTests;

public class ReportToExcelAccountExporterTests : IDisposable
{
    private readonly IServiceScope _scope;
    private readonly string _summaryReportWorksheet = "Summary Report";
    private readonly TimeTrackPortalSettingsDto _settingsDto;

    public ReportToExcelAccountExporterTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                 { "FontNames:DefaultFontName", "Calibri" }
            })
            .Build();
        var serviceCollection = new ServiceCollection();
        var rootModule = new RegistrationModuleAggregate(configuration);

        rootModule.AddModule<ExcelModule>();
        rootModule.LoadRoot(serviceCollection);

        _scope = serviceCollection.BuildServiceProvider().CreateScope();

        _settingsDto = new TimeTrackPortalSettingsDto
        {
            ExcelSettings = new TimeTrackPortalSettingsDto.Excel
            {
                SummaryReportWorksheet = "Summary Report",
                SummaryWorkingHoursReportWorksheet = "Working hours",
                DefaultFontName = "Calibri",
                BaseFontSize = 11,
                NumberFormat = "0.00",
                DefaultDateFormat = "dd/MM/yyyy"
            }
        };
    }

    [Fact]
    public async Task ExportProjectWithoutClientWhenProjectToExportProvidedShouldCreateXlWorksheetWithExpectedDataAsync()
    {
        // Arrange
        var projectToExport = new ProjectToExportModel
        {
            ProjectName = "Test",
            TotalTimeInMinutes = 960,
            Users = new List<UserToExportModel>
            {
                new()
                {
                    Department = null,
                    FirstName = "John",
                    LastName = "Doe",
                    TotalTimeInMinutes = 960,
                    Dates = new List<TasksPerDateToExportModel>
                    {
                        new()
                        {
                            Date = new DateOnly(2023, 06, 26),
                            TotalTimeInMinutes = 320,
                            Tasks = new List<TaskToExportModel>
                            {
                                new() { TaskName = "Test task 1", TrackedInMinutes = 320 }
                            }
                        },
                        new()
                        {
                            Date = new DateOnly(2023, 06, 27),
                            TotalTimeInMinutes = 320,
                            Tasks = new List<TaskToExportModel>
                            {
                                new() { TaskName = "Test task 2", TrackedInMinutes = 320 }
                            }
                        },
                        new()
                        {
                            Date = new DateOnly(2023, 06, 28),
                            TotalTimeInMinutes = 320,
                            Tasks = new List<TaskToExportModel>
                            {
                                new() { TaskName = "Test task 3", TrackedInMinutes = 160 },
                                new() { TaskName = "Test task 4", TrackedInMinutes = 160 }
                            }
                        }
                    }
                }
            }
        };

        var startDate = new DateTime(2023, 06, 26, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2023, 06, 28, 0, 0, 0, DateTimeKind.Utc);

        var expectedTable = new List<string>
        {
            "Test/client1", "", "", "",
            "John Doe - Июнь", "", "", "",
            "#", "Задача", "Департамент", "Время",
            "1.00", "Test task 1", "", "5.33",
            "2.00", "Test task 2", "", "5.33",
            "3.00", "Test task 3", "", "2.67",
            "4.00", "Test task 4", "", "2.67",
            "", "", "Итого", "16.00",
        };

        var exporter = _scope.ServiceProvider.GetRequiredService<IProjectToExcelAccountExporter>();

        // Act
        var result = await exporter.Export(new ProjectWithDatesToExportModel
        {
            Project = projectToExport,
            StartTime = startDate,
            EndTime = endDate,
            ExcelConfig = _settingsDto.ExcelSettings,
            ReportsAuthorName = "test",
            ClientName = "client1"
        }, CancellationToken.None);

        // Assert
        using var workbook = new XLWorkbook(new MemoryStream(result));
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet).Should().BeTrue();
        var actualTable = worksheet.Cells("A1:D8").Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString()).ToList();

        actualTable.Should().BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());

        worksheet.CellsUsed().Should().HaveCount(expectedTable.Count(el => !el.IsNullOrEmpty()));
    }

    [Fact]
    public async Task ExportProjectWithoutClientWhenCalledShouldSeekResultStreamToBeginAsync()
    {
        // Arrange
        var projectToExport = new ProjectToExportModel
        {
            ProjectName = "Test",
            TotalTimeInMinutes = 320,
            Users = new List<UserToExportModel>
            {
                new()
                {
                    Department = "Net",
                    FirstName = "John",
                    LastName = "Doe",
                    TotalTimeInMinutes = 320,
                    Dates = new List<TasksPerDateToExportModel>
                    {
                        new()
                        {
                            Date = new DateOnly(2023, 06, 26),
                            TotalTimeInMinutes = 320,
                            Tasks = new List<TaskToExportModel>
                            {
                                new() { TaskName = "Test task 1", TrackedInMinutes = 320 }
                            }
                        }
                    }
                }
            }
        };

        var startDate = new DateTime(2023, 06, 26, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2023, 06, 28, 0, 0, 0, DateTimeKind.Utc);

        var exporter = _scope.ServiceProvider.GetRequiredService<IProjectToExcelAccountExporter>();

        // Act
        var result = await exporter.Export(new ProjectWithDatesToExportModel
        {
            Project = projectToExport,
            StartTime = startDate,
            EndTime = endDate,
            ExcelConfig = _settingsDto.ExcelSettings!,
            ReportsAuthorName = "test",
            ClientName = "test"
        }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ExportClientWhenClientsToExportProvidedShouldCreateXlWorksheetWithExpectedData()
    {
        // Arrange
        var clientsToExport = new List<ClientToExportModel>
        {
            new()
            {
                Name = "Test client",
                TotalTimeInMinutes = 960,
                Projects = new List<ProjectToExportModel>
                {
                    new()
                    {
                        ProjectName = "Test project",
                        TotalTimeInMinutes = 960,
                        Users = new List<UserToExportModel>
                        {
                            new()
                            {Department = null,
                                FirstName = "John",
                                LastName = "Doe",
                                TotalTimeInMinutes = 960,
                                Dates = new List<TasksPerDateToExportModel>
                                {
                                    new()
                                    {
                                        Date = new DateOnly(2023, 06, 26),
                                        TotalTimeInMinutes = 320,
                                        Tasks = new List<TaskToExportModel>
                                        {
                                            new() { TaskName = "Test task 1", TrackedInMinutes = 320 }
                                        }
                                    },
                                    new()
                                    {
                                        Date = new DateOnly(2023, 06, 27),
                                        TotalTimeInMinutes = 320,
                                        Tasks = new List<TaskToExportModel>
                                        {
                                            new() { TaskName = "Test task 2", TrackedInMinutes = 320 }
                                        }
                                    },
                                    new()
                                    {
                                        Date = new DateOnly(2023, 06, 28),
                                        TotalTimeInMinutes = 320,
                                        Tasks = new List<TaskToExportModel>
                                        {
                                            new() { TaskName = "Test task 3", TrackedInMinutes = 160 },
                                            new() { TaskName = "Test task 4", TrackedInMinutes = 160 }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var startDate = new DateTime(2023, 06, 26, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2023, 06, 28, 0, 0, 0, DateTimeKind.Utc);

        var expectedTable = new List<string>
        {
            "Test project/Test client", "", "", "",
            "John Doe - Июнь", "", "", "",
            "#", "Задача", "Департамент", "Время",
            "1.00", "Test task 1", "", "5.33",
            "2.00", "Test task 2", "", "5.33",
            "3.00", "Test task 3", "", "2.67",
            "4.00", "Test task 4", "", "2.67",
            "", "", "Итого", "16.00",
        };

        var exporter = _scope.ServiceProvider.GetRequiredService<IClientsToExcelAccountExporter>();

        // Act
        var result = await exporter.Export(new ClientsWithDatesToExportModel
        {
            Clients = clientsToExport,
            StartTime = startDate,
            EndTime = endDate,
            ExcelConfig = _settingsDto.ExcelSettings,
            ReportsAuthorName = "TEST"
        }, CancellationToken.None);

        // Assert
        using var workbook = new XLWorkbook(new MemoryStream(result));
        workbook.TryGetWorksheet(_summaryReportWorksheet, out var worksheet)
            .Should().BeTrue();

        var actualTable = worksheet.Cells("A1:D8").Select(cell => cell.Value.IsNumber
                ? cell.Value.GetNumber().ToString(".00", CultureInfo.InvariantCulture)
                : cell.Value.ToString()).ToList();
        actualTable.Should().BeEquivalentTo(expectedTable, config => config.WithStrictOrdering());

        worksheet.CellsUsed().Should().HaveCount(expectedTable.Count(el => !el.IsNullOrEmpty()));
    }

    [Fact]
    public async void ExportClientWhenCalledShouldSeekResultStreamToBegin()
    {
        // Arrange
        var clientsToExport = new List<ClientToExportModel>
        {
            new()
            {
                Name = "Test",
                TotalTimeInMinutes = 320,
                Projects = new List<ProjectToExportModel>
                {
                    new()
                    {
                        ProjectName = "Test",
                        TotalTimeInMinutes = 320,
                        Users = new List<UserToExportModel>
                        {
                            new()
                            {Department = "Net",
                                FirstName = "John",
                                LastName = "Doe",
                                TotalTimeInMinutes = 320,
                                Dates = new List<TasksPerDateToExportModel>
                                {
                                    new()
                                    {
                                        Date = new DateOnly(2023, 06, 26),
                                        TotalTimeInMinutes = 320,
                                        Tasks = new List<TaskToExportModel>
                                        {
                                            new() { TaskName = "Test task 1", TrackedInMinutes = 320 }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        var startDate = new DateTime(2023, 06, 26, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2023, 06, 28, 0, 0, 0, DateTimeKind.Utc);

        var exporter = _scope.ServiceProvider.GetRequiredService<IClientsToExcelAccountExporter>();

        // Act
        var result = await exporter.Export(new ClientsWithDatesToExportModel
        {
            Clients = clientsToExport,
            StartTime = startDate,
            EndTime = endDate,
            ExcelConfig = _settingsDto.ExcelSettings,
            ReportsAuthorName = "TEST"
        }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    public void Dispose()
    {
        _scope?.Dispose();
    }
}
