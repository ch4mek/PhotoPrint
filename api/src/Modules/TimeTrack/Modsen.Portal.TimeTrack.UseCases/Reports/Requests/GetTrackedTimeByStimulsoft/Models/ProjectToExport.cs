namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;

internal class ProjectToExport
{
    public IReadOnlyCollection<UserToExport> Users { get; set; } = [];
    public required Duration TotalTime { get; set; }
    public required string ProjectName { get; set; }
    public required string ClientName { get; set; }
}
