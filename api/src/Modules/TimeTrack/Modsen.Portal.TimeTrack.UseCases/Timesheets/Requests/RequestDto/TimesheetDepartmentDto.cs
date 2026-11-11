namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class TimesheetDepartmentDto
{
    public long Duration { get; set; }

    public string? DepartmentId { get; set; }

    public int Count { get; set; }
}
