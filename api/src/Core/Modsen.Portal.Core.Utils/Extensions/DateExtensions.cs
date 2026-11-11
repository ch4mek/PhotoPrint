namespace Modsen.Portal.Core.Utils.Extensions;

public static class DateExtensions
{
    private static readonly Dictionary<LocationType, TimeZoneInfo> _locationTimeZones = new()
    {
        [LocationType.Minsk] = TimeZoneInfo.FindSystemTimeZoneById("Belarus Standard Time"),
        [LocationType.Vitebsk] = TimeZoneInfo.FindSystemTimeZoneById("Belarus Standard Time"),
        [LocationType.Warsaw] = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"),
        [LocationType.Tbilisi] = TimeZoneInfo.FindSystemTimeZoneById("Georgian Standard Time")
    };

    public static int GetWeekOfYear(this DateOnly date, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        var firstDayOfYear = new DateOnly(date.Year, 1, 1);
        var firstDayOffset = (7 + (firstDayOfYear.DayOfWeek - firstDayOfWeek)) % 7;

        var firstWeekStart = firstDayOfYear.AddDays(-firstDayOffset);

        var daysSinceFirstWeekStart = date.DayNumber - firstWeekStart.DayNumber;

        return (daysSinceFirstWeekStart / 7) + 1;
    }

    public static int GetWeekOfYear(this DateTime date, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
    {
        return DateOnly.FromDateTime(date).GetWeekOfYear(firstDayOfWeek);
    }

    public static DateTime ToLocal(this DateTime dateTime, LocationType location)
    {
        if (_locationTimeZones.TryGetValue(location, out var timeZone))
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        throw new ArgumentException($"Time zone for {location} is not specified");
    }

    public static DateTime ToUtc(this DateTime dateTime, LocationType location)
    {
        if (_locationTimeZones.TryGetValue(location, out var timeZone))
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        throw new ArgumentException($"Time zone for {location} is not specified");
    }

    /// <param name="targetDate">Target date</param>
    /// <param name="yearForLocalDate">Year for local date</param>
    /// <param name="useNextDayIfNotLeapYearCase">Use 1 of March if current
    /// year does not have 29 of February, otherwise previous will be chosen</param>
    /// <returns>Date</returns>
    public static DateOnly GetLocalDateFromOldDate(
        this DateOnly targetDate,
        int yearForLocalDate,
        bool useNextDayIfNotLeapYearCase = true)
    {
        const int leapYearFebruaryDay = 29;
        const int februaryMonthNumber = 2;
        const int marchMonthNumber = 3;

        var targetDateDay = targetDate.Day;
        var targetDateMonth = targetDate.Month;

        if (
            targetDateDay == leapYearFebruaryDay &&
            targetDateMonth == februaryMonthNumber &&
            !DateTime.IsLeapYear(yearForLocalDate))
        {
            targetDateDay = useNextDayIfNotLeapYearCase ? 1 : 28;
            targetDateMonth = useNextDayIfNotLeapYearCase ? marchMonthNumber : februaryMonthNumber;
        }

        return new DateOnly(yearForLocalDate, targetDateMonth, targetDateDay);
    }

    /// <summary>
    /// Check if the specified date is in range between two provided dates
    /// </summary>
    /// <param name="targetDate">Specified date</param>
    /// <param name="from">From this date</param>
    /// <param name="to">Up to this date</param>
    /// <returns>Result</returns>
    public static bool IsDateInRange(
        this DateOnly targetDate,
        DateOnly from,
        DateOnly to)
    {
        return from <= targetDate && targetDate <= to;
    }
}
