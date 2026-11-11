namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class TimesheetUserDto
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public long Duration { get; set; }

    public int Count { get; set; }
}
