namespace Modsen.Portal.TimeTrack.Domain;

internal class UserWithTimesheets
{
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public IReadOnlyCollection<TimesheetSummary> Timesheets { get; set; } = [];
}
