using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal class GroupedUserTimesheetCoreDto
{
    public int? TimesheetId { get; set; }

    public string? TaskName { get; set; }

    public int? FactTrackedInMinutes { get; set; }

    public int? BillableTrackedInMinutes { get; set; }

    public DateOnly? ReportForDate { get; set; }

    public int UserId { get; set; }

    public required string UserFullName { get; set; }

    public string? Position { get; set; }

    public string? DepartmentId { get; set; }

    public int? ResourceGroupId { get; set; }

    public string? UserColor { get; set; }

    public bool UserIsActive { get; set; }

    public TimesheetSubmitStatusType? Status { get; set; }
}
