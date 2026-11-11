using System.Text.Json.Serialization;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class TimesheetSnapshotDto
{
    public int? Id { get; set; }

    [JsonPropertyName("ftm")]
    public int? FactTrackedInMinutes { get; set; }

    [JsonPropertyName("btm")]
    public int? BillableTrackedInMinutes { get; set; }

    public DateOnly? ReportForDate { get; set; }
}
