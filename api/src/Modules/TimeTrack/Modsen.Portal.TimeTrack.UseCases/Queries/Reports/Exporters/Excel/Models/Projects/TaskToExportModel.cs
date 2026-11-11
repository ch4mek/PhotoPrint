namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

public class TaskToExportModel
{
    public required string TaskName { get; set; }
    public int TrackedInMinutes { get; set; }
}
