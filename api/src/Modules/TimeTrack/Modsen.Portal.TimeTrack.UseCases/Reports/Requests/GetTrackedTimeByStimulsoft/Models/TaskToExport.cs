namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;

internal class TaskToExport
{
    public required string TaskName { get; set; }
    public required Duration TrackedTime { get; set; }
}
