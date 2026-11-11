namespace Modsen.Portal.Core.Services;

public interface IClockService
{
    public DateTime UtcNow { get; }
    public DateTime GetMinskNow(int minskTimezoneHours);

    public DateTime RoundUpToHalfAnHour(DateTime dateTime);
    public DateOnly TodayUtc();
    public DateTime TodayStartOfDayUtc();
}
