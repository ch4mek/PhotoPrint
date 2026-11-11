using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.Domain;

public class TimesheetDomain
{
    public int Id { get; set; }
    public required string TaskName { get; set; }
    public required int FactTrackedInMinutes { get; set; }
    public int BillableTrackedInMinutes { get; set; }

    public required DateOnly ReportForDate { get; set; }
    public required int ReportYear { get; set; }
    public required int ReportMonthOfYear { get; set; }
    public required int ReportWeekOfYear { get; set; }
    public required int ReportDayOfYear { get; set; }

    public required int UserId { get; set; }
    public required string UserFullName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserColor { get; set; }

    public required int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public required string ProjectColor { get; set; }

    public static Func<TimesheetDomain, int> GetTrackedTimeSelector(TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => x => x.FactTrackedInMinutes,
            TrackedTimeType.Billable => x => x.BillableTrackedInMinutes,
            _ => throw new InvalidOperationException("Unsupported tracked time type")
        };
    }

    public int GetTrackedTimeByType(TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => FactTrackedInMinutes,
            TrackedTimeType.Billable => BillableTrackedInMinutes,
            _ => throw new InvalidOperationException("Unsupported tracked time type")
        };
    }
}
