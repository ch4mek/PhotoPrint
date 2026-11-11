namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal class UsersTimesheetsDto
{
    public int Month { get; set; }

    public int Year { get; set; }

    public long Duration { get; set; }

    public IReadOnlyCollection<TimesheetUserDto> Users { get; set; } = [];
}
