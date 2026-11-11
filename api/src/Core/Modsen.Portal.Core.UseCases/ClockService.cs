using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public sealed class ClockService : IClockService
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime GetMinskNow(int minskTimezoneHours)
    {
        return DateTime.UtcNow.AddHours(minskTimezoneHours);
    }

    public DateTime RoundUpToHalfAnHour(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour,
            dateTime.Minute - (dateTime.Minute % 30), 0, DateTimeKind.Utc);
    }

    public DateOnly TodayUtc()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public DateTime TodayStartOfDayUtc()
    {
        var now = UtcNow;
        return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
    }
}
