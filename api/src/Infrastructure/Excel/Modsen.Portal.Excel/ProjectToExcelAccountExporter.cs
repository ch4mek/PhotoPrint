using System.Globalization;
using ClosedXML.Excel;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

namespace Modsen.Portal.Excel;

public sealed class ProjectToExcelAccountExporter : IProjectToExcelAccountExporter
{
    private int _currentRow;
    private int _currentColumn;
    private readonly int _startRow = 1;
    private readonly int _startColumn = 1;
    private readonly int _taskNumberColumn = 1;
    private readonly int _taskNameColumn = 2;
    private readonly int _departmentColumn = 3;
    private readonly int _trackedTimeColumn = 4;

    public Task<byte[]> Export(ProjectWithDatesToExportModel model, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(model);

        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet(model.ExcelConfig.SummaryReportWorksheet);

        _currentRow = _startRow;
        _currentColumn = _startColumn;

        PrintClientProject(sheet, model.Project, model.ClientName);

        foreach (var user in model.Project.Users)
        {
            var tasksPerMonth = user.Dates.GroupBy(taskModel => taskModel.Date.Month);

            foreach (var tasksWithDates in tasksPerMonth)
            {
                PrintUserMonth(sheet, user, tasksWithDates.Key);
                PrintTasksHeader(sheet);

                var tasksCounter = 1;

                var tasks = tasksWithDates
                    .SelectMany(t => t.Tasks)
                    .GroupBy(t => t.TaskName)
                    .Select(g => new TaskToExportModel
                    {
                        TaskName = g.Key,
                        TrackedInMinutes = g.Sum(t => t.TrackedInMinutes)
                    });

                foreach (var task in tasks)
                {
                    PrintTask(sheet, task, user.Department, tasksCounter);
                    tasksCounter++;
                }

                var totalMinutes = tasksWithDates
                    .SelectMany(taskModel => taskModel.Tasks)
                    .Sum(task => task.TrackedInMinutes);
                PrintUserMonthTotal(sheet, totalMinutes);
            }
        }

        SetWorkBookProperties(workbook, model.ReportsAuthorName);

        SetBaseStyle(sheet);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return Task.FromResult(stream.ToArray());
    }

    private void PrintClientProject(IXLWorksheet sheet, ProjectToExportModel project, string clientName)
    {
        var leftCell = sheet.Cell(_currentRow, _currentColumn);
        var rightCell = sheet.Cell(_currentRow, _currentColumn + 3);

        var range = sheet.Range(leftCell, rightCell);
        range.Merge();
        range.Value = $"{project.ProjectName}/{clientName}";
        range.Style.Font.SetBold();

        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        _currentRow++;
    }

    private void PrintUserMonth(IXLWorksheet sheet, UserToExportModel user, int month)
    {
        var leftCell = sheet.Cell(_currentRow, _currentColumn);
        var rightCell = sheet.Cell(_currentRow, _currentColumn + 3);

        var range = sheet.Range(leftCell, rightCell);
        range.Merge();

        var fullName = $"{user.FirstName} {user.LastName}";
        var fullNameRu =  user.FirstNameRu.IsNullOrEmpty() ? string.Empty : $"({user.FirstNameRu} {user.LastNameRu})";
        var monthName = CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat
            .GetMonthName(month)
            .WithFirstCharToUpper();

        range.Value = $"{fullName}{fullNameRu} - {monthName}";

        range.Style.Font.SetBold();

        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        _currentRow++;
    }

    private void PrintTasksHeader(IXLWorksheet sheet)
    {
        sheet.Cell(_currentRow, _taskNumberColumn).Value = "#";
        sheet.Cell(_currentRow, _taskNumberColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        sheet.Cell(_currentRow, _taskNameColumn).Value = "Задача";

        sheet.Cell(_currentRow, _departmentColumn).Value = "Департамент";
        sheet.Cell(_currentRow, _departmentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        sheet.Cell(_currentRow, _trackedTimeColumn).Value = "Время";
        sheet.Cell(_currentRow, _trackedTimeColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        _currentRow++;
    }

    private void PrintTask(IXLWorksheet sheet, TaskToExportModel task, string? department, int taskNumber)
    {
        var interval = TimeSpan.FromMinutes(task.TrackedInMinutes);
        var timeInHours = interval.Days * 24 + interval.Hours + interval.Minutes / 60m;
        var timeText = $"{timeInHours:F2}";

        sheet.Cell(_currentRow, _taskNumberColumn).Value = taskNumber;
        sheet.Cell(_currentRow, _taskNumberColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        sheet.Cell(_currentRow, _taskNameColumn).Value = task.TaskName;
        sheet.Cell(_currentRow, _taskNameColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        sheet.Cell(_currentRow, _departmentColumn).Value = department;
        sheet.Cell(_currentRow, _departmentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        sheet.Cell(_currentRow, _trackedTimeColumn).Value = timeText;
        sheet.Cell(_currentRow, _trackedTimeColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        _currentRow++;
    }

    private void PrintUserMonthTotal(IXLWorksheet sheet, int totalMinutes)
    {
        var interval = TimeSpan.FromMinutes(totalMinutes);
        var totalHours = interval.Days * 24 + interval.Hours + interval.Minutes / 60m;
        var totalTimeText = $"{totalHours:F2}";

        sheet.Cell(_currentRow, _departmentColumn).Value = "Итого";
        sheet.Cell(_currentRow, _departmentColumn).Style.Font.SetBold();
        sheet.Cell(_currentRow, _departmentColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        sheet.Cell(_currentRow, _trackedTimeColumn).Value = totalTimeText;
        sheet.Cell(_currentRow, _trackedTimeColumn).Style.Font.SetBold();
        sheet.Cell(_currentRow, _trackedTimeColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        _currentRow++;
    }

    private void SetBaseStyle(IXLWorksheet sheet)
    {
        sheet.Column(_taskNumberColumn).Width = 4.5;
        sheet.Column(_taskNameColumn).Width = 45;
        sheet.Column(_departmentColumn).Width = 13;
        sheet.Column(_trackedTimeColumn).Width = 8;

        sheet.RowHeight = 14.5;

        sheet.Style
            .Font.SetFontName("Calibri")
            .Font.SetFontSize(11)
            .Alignment.SetWrapText(false)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
    }

    private static void SetWorkBookProperties(XLWorkbook workbook, string reportsAuthorName)
    {
        workbook.Properties.Author = reportsAuthorName;
    }
}
