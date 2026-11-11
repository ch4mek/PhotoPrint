namespace Modsen.Portal.TimeTrack.Domain;

public class DashboardActivity
{
    public required string ProjectName { get; set; }

    public required string TaskName { get; set; }

    public int TrackedInMinutes { get; set; }

    public DateOnly ReportForDate { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? OrderByDateUtc { get; set; }
}
