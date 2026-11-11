using System.Text.Json.Serialization;
using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class UserTimesheetSnapshotDto
{
    public int UserId { get; set; }

    public string? Color { get; set; }

    public required string UserName { get; set; }

    public string? Role { get; set; }

    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }

    public bool IsActive { get; set; }

    [JsonPropertyName("tft")]
    public int TotalFactTime { get; set; }

    [JsonPropertyName("tbt")]
    public int TotalBillableTime { get; set; }

    public TimesheetSubmitStatusType? Status { get; set; }

    public IReadOnlyCollection<TimesheetSnapshotDto> Timesheets { get; set; } = [];
}
