namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

public class DashboardTasksCoreDto
{
    public DateOnly ReportForDate { get; set; }

    public string? Color { get; set; }

    public required string TaskName { get; set; }

    public int TrackedInMinutes { get; set; }
}
