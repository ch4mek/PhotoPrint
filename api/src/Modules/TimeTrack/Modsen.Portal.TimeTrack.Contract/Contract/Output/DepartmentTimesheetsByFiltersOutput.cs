namespace Modsen.Portal.TimeTrack.Contract.Contract.Output;

public class DepartmentTimesheetsByFiltersOutput
{
    public int Month { get; set; }

    public int Year { get; set; }

    public long Duration { get; set; }

    public IReadOnlyCollection<TimesheetDepartmentOutput> Departments { get; set; } = [];
}
