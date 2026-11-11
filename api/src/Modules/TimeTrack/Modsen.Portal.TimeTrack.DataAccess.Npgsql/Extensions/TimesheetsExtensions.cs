using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;

internal static class TimesheetsExtensions
{
    public static IQueryable<Timesheet> FilterByPeriod(this IQueryable<Timesheet> timesheets, int? year, IReadOnlyCollection<int>? monthOfYear, int? weekOfYear)
    {
        if (!year.HasValue)
        {
            return timesheets;
        }

        timesheets = timesheets.Where(x => x.ReportYear == year);

        if (monthOfYear is { Count: > 0 })
        {
            timesheets = timesheets.Where(x => monthOfYear.Contains(x.ReportMonthOfYear));
        }

        if (weekOfYear.HasValue)
        {
            timesheets = timesheets.Where(x => x.ReportWeekOfYear == weekOfYear.Value);
        }

        return timesheets;
    }

    public static IQueryable<Timesheet> ApplyTimesheetFilter(this IQueryable<Timesheet> timesheets, TimesheetFilter filter)
    {
        if (filter.UserId.HasValue)
        {
            timesheets = timesheets.Where(x => x.UserId == filter.UserId.Value);
        }

        if (filter.DepartmentId is not null)
        {
            timesheets = timesheets.Where(x => x.User!.DepartmentId == filter.DepartmentId);
        }

        if (filter.ClientId.HasValue)
        {
            timesheets = timesheets.Where(x => x.Project!.ClientId == filter.ClientId.Value);
        }

        if (filter.ProjectId.HasValue)
        {
            timesheets = timesheets.Where(x => x.ProjectId == filter.ProjectId.Value);
        }

        if (filter.ResourceGroupId.HasValue)
        {
            if (filter.ResourceGroupId.Value == 0)
            {
                timesheets = timesheets.Where(x =>
                    x.User!.ResourceGroupId == null ||
                    (x.User.ResourceGroup != null && !x.User.ResourceGroup.IsActive));
            }
            else
            {
                timesheets = timesheets.Where(x => x.User!.ResourceGroupId == filter.ResourceGroupId.Value);
            }
        }

        if (filter.TrackedTimeType.HasValue)
        {
            timesheets = timesheets.Where(x => x.FactTrackedInMinutes != 0 && filter.TrackedTimeType == TrackedTimeType.Fact ||
                                               x.BillableTrackedInMinutes != 0 && filter.TrackedTimeType == TrackedTimeType.Billable);
        }

        return timesheets;
    }
}
