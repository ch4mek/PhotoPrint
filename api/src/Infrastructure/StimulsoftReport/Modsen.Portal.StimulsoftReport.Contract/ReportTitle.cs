namespace Modsen.Portal.StimulsoftReport.Contract;

public class ReportTitle
{
    public string? SubjectName { get; set; }
    public required string PeriodInfo { get; set; }
    public required DateTime FromDate { get; set; }
    public required DateTime ToDate { get; set; }
}
