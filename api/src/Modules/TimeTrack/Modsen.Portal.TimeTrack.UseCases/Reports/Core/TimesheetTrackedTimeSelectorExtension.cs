using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Core;

internal static class TimesheetTrackedTimeSelectorExtension
{
    public static Func<Timesheet, int> GetTrackedTimeSelector(this TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => x => x.FactTrackedInMinutes,
            TrackedTimeType.Billable => x => x.BillableTrackedInMinutes,
            _ => throw new InvalidOperationException("Unsupported tracked time type")
        };
    }
}
