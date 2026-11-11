namespace Modsen.Portal.TimeTrack.Contract.Contract.Output;

public class TimesheetByFiltersOutput
{
    public int Month { get; set; }

    public int Year { get; set; }

    public long Duration { get; set; }

    public IReadOnlyCollection<TimesheetUserOutput> Users { get; set; } = [];
}
