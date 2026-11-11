using System.Text.Json.Serialization;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

public class TimesheetCoreDto
{
    public int Id { get; set; }

    public int? CreatedById { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string? TaskName { get; set; }

    [JsonPropertyName("ftm")]
    public int FactTrackedInMinutes { get; set; }

    [JsonPropertyName("btm")]
    public int BillableTrackedInMinutes { get; set; }

    public DateOnly ReportForDate { get; set; }

    public int UserId { get; set; }

    public int ProjectId { get; set; }
}
