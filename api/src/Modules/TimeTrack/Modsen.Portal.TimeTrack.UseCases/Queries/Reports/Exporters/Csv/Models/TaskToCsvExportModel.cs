
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

public class TaskToCsvExportModel
{
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public int TimeInMinutes { get; set; }

    internal static TaskToCsvExportModel Of(Timesheet source)
    {
        return new TaskToCsvExportModel
        {
            Date = source.ReportForDate,
            Description = source.TaskName,
            TimeInMinutes = source.FactTrackedInMinutes,
        };
    }
}
