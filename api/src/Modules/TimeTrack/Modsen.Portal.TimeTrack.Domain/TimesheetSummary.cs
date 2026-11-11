namespace Modsen.Portal.TimeTrack.Domain;

public class TimesheetSummary
{
    public int Id { get; set; }

    public required string ProjectName { get; set; }

    public string? TaskName { get; set; }

    public int TrackedInMinutes { get; set; }

    public DateOnly ReportForDate { get; set; }

    public int ProjectId { get; set; }
}
