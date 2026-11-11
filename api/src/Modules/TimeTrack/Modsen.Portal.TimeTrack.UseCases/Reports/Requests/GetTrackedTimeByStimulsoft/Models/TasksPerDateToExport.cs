namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;

internal class TasksPerDateToExport
{
    public DateOnly Date { get; set; }
    public IReadOnlyCollection<TaskToExport> Tasks { get; set; } = [];
    public required Duration TotalTime { get; set; }
    public int TotalTimeInMinutes { get; set; }
}
