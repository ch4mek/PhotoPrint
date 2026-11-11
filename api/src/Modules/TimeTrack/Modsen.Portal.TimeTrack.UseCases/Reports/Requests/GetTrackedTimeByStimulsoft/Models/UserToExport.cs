namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;

internal class UserToExport
{
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public IReadOnlyCollection<TasksPerDateToExport> Dates { get; set; } = [];
    public required Duration TotalTime { get; set; }
    public int TotalTimeInMinutes { get; set; }
}
