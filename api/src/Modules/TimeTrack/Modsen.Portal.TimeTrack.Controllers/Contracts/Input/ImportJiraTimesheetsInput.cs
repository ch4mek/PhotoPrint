namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public sealed class ImportJiraTimesheetsInput
{
    public required int ProjectId { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
}
