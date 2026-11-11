namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal class DepartmentTimesheetsDto
{
    public int Month { get; set; }

    public int Year { get; set; }

    public long Duration { get; set; }

    public IReadOnlyCollection<TimesheetDepartmentDto> Departments { get; set; } = [];
}
