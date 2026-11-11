namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto
{
    public class UserTimesheetCalendarDto
    {
        public DateOnly Date { get; set; }
        public int FactTrackedSum { get; set; }
        public IReadOnlyCollection<ProjectTaskDto> Tasks { get; set; } = [];
    }
}
