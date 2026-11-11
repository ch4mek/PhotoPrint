namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

public class TasksPerDateToExportModel
{
    public DateOnly Date { get; set; }
    public IReadOnlyCollection<TaskToExportModel> Tasks { get; set; } = [];
    public int TotalTimeInMinutes { get; set; }
}
