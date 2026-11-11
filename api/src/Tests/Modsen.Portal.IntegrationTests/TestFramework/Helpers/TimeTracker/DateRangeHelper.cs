namespace Modsen.Portal.IntegrationTests.TestFramework.Helpers.TimeTracker;

internal static class DateRangeHelper
{
    public static DateRange GetDateRangeFromYear(int year)
    {
        return new DateRange(
                new DateOnly(year, 1, 1).ToString("yyyy-MM-dd"),
                new DateOnly(year, 12, DateTime.DaysInMonth(year, 12)).ToString("yyyy-MM-dd")
            );
    }

    public static DateRange GetDateRangeFromMonth(int year, int month)
    {
        return new DateRange(
                new DateOnly(year, month, 1).ToString("yyyy-MM-dd"),
                new DateOnly(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd")
            );
    }
}

internal record DateRange(string DateFrom, string DateTo);
