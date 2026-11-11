namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

public class TasksToCsvExportModel
{
    public IEnumerable<TaskToCsvExportModel> Tasks { get; set; } = [];
    public required string DateFormat { get; set; }
}
