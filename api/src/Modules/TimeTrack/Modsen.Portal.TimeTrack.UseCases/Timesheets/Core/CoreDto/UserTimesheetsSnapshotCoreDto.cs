using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal sealed class UserTimesheetsSnapshotCoreDto
{
    public int UserId { get; set; }

    public string? Color { get; set; }

    public required string UserName { get; set; }

    public string? Position { get; set; }

    public string? UserDepartmentId { get; set; }

    public int? UserResourceGroupId { get; set; }

    public bool IsActive { get; set; }

    public int TotalFactTime { get; set; }

    public int TotalBillableTime { get; set; }

    public TimesheetSubmitStatusType? Status { get; set; }

    public IReadOnlyCollection<TimesheetSnapshotCoreDto> Timesheets { get; set; } = [];
}
