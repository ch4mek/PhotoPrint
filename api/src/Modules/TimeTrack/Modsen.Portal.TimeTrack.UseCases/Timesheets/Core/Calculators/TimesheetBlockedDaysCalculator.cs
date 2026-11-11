using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Calculators;

internal interface ITimesheetBlockedDaysCalculator
{
    Task<List<DateOnly>> CalculateAsync(int userId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken);
}

internal sealed class TimesheetBlockedDaysCalculator : ITimesheetBlockedDaysCalculator
{
    private readonly ITimeTrackDbContext _dbContext;

    public TimesheetBlockedDaysCalculator(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DateOnly>> CalculateAsync(int userId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var statuses = await _dbContext.TimesheetSubmitStatuses.Where(submitStatus =>
            submitStatus.UserId == userId &&
            (submitStatus.Year > startDate.Year || (submitStatus.Year == startDate.Year && submitStatus.Month >= startDate.Month)) &&
            (submitStatus.Year < endDate.Year || (submitStatus.Year == endDate.Year && submitStatus.Month <= endDate.Month)))
            .ToListAsync(cancellationToken);

        var blockedDays = GetBlockedDays(statuses, startDate, endDate);

        return blockedDays;
    }

    private static List<DateOnly> GetBlockedDays(List<TimesheetSubmitStatus> statuses, DateOnly startDate,
        DateOnly endDate)
    {
        var blockedDays = new HashSet<DateOnly>();
        var groupedStatusesByDays = statuses.GroupBy(ts => ts.Date).ToList();

        foreach (var reportDay in groupedStatusesByDays)
        {
            AddBlockedDaysForGroup(blockedDays, reportDay, startDate, endDate);
        }

        return blockedDays.ToList();
    }

    private static void AddBlockedDaysForGroup(HashSet<DateOnly> blockedDays, IGrouping<DateOnly, TimesheetSubmitStatus> group,
        DateOnly startDate, DateOnly endDate)
    {
        foreach (var status in group)
        {
            AddBlockedDaysForStatus(blockedDays, status, startDate, endDate);
        }
    }

    private static void AddBlockedDaysForStatus(HashSet<DateOnly> blockedDays, TimesheetSubmitStatus status,
        DateOnly startDate, DateOnly endDate)
    {
        if (!IsApprovedOrUnderReview(status))
        {
            return;
        }

        var (rangeStart, rangeEnd) = GetBlockedRange(status.Date, startDate, endDate);
        if (rangeStart == null)
        {
            return;
        }

        AddDaysInRange(blockedDays, rangeStart.Value, rangeEnd);
    }

    private static bool IsApprovedOrUnderReview(TimesheetSubmitStatus status)
    {
        return status.Status is TimesheetSubmitStatusType.UnderReview or TimesheetSubmitStatusType.Approved;
    }

    private static (DateOnly? RangeStart, DateOnly RangeEnd) GetBlockedRange(DateOnly statusDate, DateOnly startDate, DateOnly endDate)
    {
        var isStartMonthInRange = statusDate.Month == startDate.Month && statusDate.Year == startDate.Year;
        var isEndMonthInRange = statusDate.Month == endDate.Month && statusDate.Year == endDate.Year;

        if (!isStartMonthInRange && !isEndMonthInRange)
        {
            return (null, statusDate);
        }

        var statusMonthStart = new DateOnly(statusDate.Year, statusDate.Month, 1);
        var statusMonthEnd = new DateOnly(statusDate.Year, statusDate.Month, DateTime.DaysInMonth(statusDate.Year, statusDate.Month));

        var rangeStart = isStartMonthInRange ? startDate : statusMonthStart;
        var rangeEnd = isEndMonthInRange ? endDate : statusMonthEnd;

        return (rangeStart, rangeEnd);
    }

    private static void AddDaysInRange(HashSet<DateOnly> blockedDays, DateOnly rangeStart, DateOnly rangeEnd)
    {
        var currentDay = rangeStart;
        while (currentDay <= rangeEnd)
        {
            blockedDays.Add(currentDay);
            currentDay = currentDay.AddDays(1);
        }
    }
}
