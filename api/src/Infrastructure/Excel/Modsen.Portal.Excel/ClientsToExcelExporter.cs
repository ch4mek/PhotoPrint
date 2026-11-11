using System.Globalization;
using ClosedXML.Excel;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

namespace Modsen.Portal.Excel;

internal sealed class ClientsToExcelExporter : IClientsToExcelExporter
{
    private int _currentRow;
    private int _currentColumn;
    private int _reportTypeColumnShift;
    private readonly int _startRow = 1;
    private readonly int _startColumn = 1;
    private const int s_projectColumnShift = 0;
    private const int s_userColumnShift = 1;
    private const int s_dateColumnShift = 2;
    private const int s_taskColumnShift = 3;
    private const int s_timeColumnShift = 4;

    private static readonly string[] _header = ["Client", "Project", "User", "Date", "Description", "Time (h)", "Time (decimal)"
    ];

    public Task<byte[]> Export(ClientsWithDatesToExportModel model, CancellationToken token)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(model.ExcelConfig.SummaryReportWorksheet);

        _currentRow = _startRow;
        _currentColumn = _startColumn;
        _reportTypeColumnShift = 1;

        PrintClientHeader(sheet);
        foreach (var client in model.Clients)
        {
            PrintClient(sheet, client);
            foreach (var project in client.Projects)
            {
                PrintProject(sheet, project, model.ExcelConfig);
                foreach (var user in project.Users)
                {
                    PrintUser(sheet, user, model.ExcelConfig);
                    foreach (var tasksPerDate in user.Dates)
                    {
                        PrintDate(sheet, tasksPerDate, model.ExcelConfig);
                        foreach (var task in tasksPerDate.Tasks)
                        {
                            PrintTask(sheet, task, model.ExcelConfig);
                        }
                    }
                }
            }
        }

        PrintClientsTotal(sheet, model.Clients, model.StartTime, model.EndTime, model.ExcelConfig);
        SetWorkBookProperties(workbook, model.ReportsAuthorName);

        SetBaseStyle(sheet,
            nameColumns: [1, 2, 3],
            dateColumn: 4,
            descriptionColumn: 5,
            trackedTimeColumns: [6, 7],
            model.ExcelConfig);

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return Task.FromResult(stream.ToArray());
    }

    private void PrintClient(IXLWorksheet sheet, ClientToExportModel client)
    {
        sheet.Cell(_currentRow, _currentColumn).SetValue($"{client.Name}");
        sheet.Row(_currentRow).Style.Font.SetBold();
        _currentRow++;
    }

    private static void SetBaseStyle(IXLWorksheet sheet, int[] nameColumns, int dateColumn, int descriptionColumn,
        int[] trackedTimeColumns, TimeTrackPortalSettingsDto.Excel config)
    {
        foreach (var column in nameColumns)
        {
            sheet.Column(column).Width = 30;
        }

        sheet.Column(dateColumn).Width = 12;
        sheet.Column(descriptionColumn).Width = 60;

        foreach (var column in trackedTimeColumns)
        {
            sheet.Cell(1, column).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            sheet.Column(column).Width = 14;
        }

        sheet.Style
            .Font.SetFontName(config.DefaultFontName)
            .Font.SetFontSize(config.BaseFontSize)
            .Alignment.SetWrapText(true)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
    }

    private static void SetWorkBookProperties(XLWorkbook workbook, string reportsAuthorName)
    {
        workbook.Properties.Author = reportsAuthorName;
    }

    private void PrintTask(IXLWorksheet sheet, TaskToExportModel task, TimeTrackPortalSettingsDto.Excel config)
    {
        sheet.Cell(_currentRow, _currentColumn + s_taskColumnShift + _reportTypeColumnShift).Value = task.TaskName;
        SetTimeInRow(sheet, task.TrackedInMinutes, config);
        _currentRow++;
    }

    private void PrintDate(IXLWorksheet sheet, TasksPerDateToExportModel tasksPerDate, TimeTrackPortalSettingsDto.Excel config)
    {
        sheet.Cell(_currentRow, _currentColumn + s_dateColumnShift + _reportTypeColumnShift)
            .SetValue(tasksPerDate.Date.ToString(config.DefaultDateFormat, CultureInfo.InvariantCulture))
            .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        SetTimeInRow(sheet, tasksPerDate.TotalTimeInMinutes, config);
        sheet.Row(_currentRow).Style.Font.SetBold();
        _currentRow++;
    }

    private void PrintUser(IXLWorksheet sheet, UserToExportModel user, TimeTrackPortalSettingsDto.Excel config)
    {
        sheet.Cell(_currentRow, _currentColumn + s_userColumnShift + _reportTypeColumnShift).SetValue($"{user.FirstName} {user.LastName}");
        SetTimeInRow(sheet, user.TotalTimeInMinutes, config);
        sheet.Row(_currentRow).Style.Font.SetBold();
        _currentRow++;
    }

    private void PrintClientHeader(IXLWorksheet sheet)
    {
        sheet.Cell(_currentRow, _currentColumn).InsertData(_header, true);
        sheet.Row(_currentRow).Style.Font.SetBold();
        _currentRow++;
    }

    private void PrintClientsTotal(IXLWorksheet sheet, IReadOnlyCollection<ClientToExportModel> clients, DateTime startTime, DateTime endTime,
        TimeTrackPortalSettingsDto.Excel config)
    {
        sheet.Cell(_currentRow, _currentColumn)
            .SetValue($"Total({startTime.ToString(config.DefaultDateFormat, CultureInfo.InvariantCulture)}-{endTime.ToString(config.DefaultDateFormat, CultureInfo.InvariantCulture)})");
        var trackedTime = clients.Sum(x => x.TotalTimeInMinutes);
        SetTimeInRow(sheet, trackedTime, config);
        sheet.Row(_currentRow).Style.Font.SetBold();
        _currentRow++;
    }

    private void PrintProject(IXLWorksheet sheet, ProjectToExportModel project, TimeTrackPortalSettingsDto.Excel config)
    {
        sheet.Cell(_currentRow, _currentColumn + s_projectColumnShift + _reportTypeColumnShift).SetValue(project.ProjectName);
        SetTimeInRow(sheet, project.TotalTimeInMinutes, config);
        sheet.Row(_currentRow).Style.Font.SetBold();
        _currentRow++;
    }

    private void SetTimeInRow(IXLWorksheet sheet, int timeInMinutes, TimeTrackPortalSettingsDto.Excel config)
    {
        var interval = TimeSpan.FromMinutes(timeInMinutes);

        sheet.Cell(_currentRow, _currentColumn + _reportTypeColumnShift + s_timeColumnShift)
            .SetValue($"{interval.Days * 24 + interval.Hours}:{interval.Minutes:d2}")
            .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        sheet.Cell(_currentRow, _currentColumn + _reportTypeColumnShift + s_timeColumnShift + 1)
            .SetValue(interval.TotalHours)
            .Style.NumberFormat.SetFormat(config.NumberFormat);
    }
}
