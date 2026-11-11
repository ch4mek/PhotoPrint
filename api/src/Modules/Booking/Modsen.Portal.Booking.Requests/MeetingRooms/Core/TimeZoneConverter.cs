using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface ITimeZoneConverter
{
    bool IsAllDay(DateTime from, DateTime to);

    DateTime ConvertUtcToOfficeTime(DateTime utcDateTime, string officeTimeZone);
    DateTime ConvertToUtcFromOfficeTime(DateTime dateTime, string officeTimeZone);

    OfficeDayRangeCoreDto GetOfficeDayRange(DateOnly officeTimeZoneDay, string officeTimeZone);
}

internal class TimeZoneConverter : ITimeZoneConverter
{
    public bool IsAllDay(DateTime from, DateTime to)
    {
        return from.AddDays(1) == to ||
            from.AddDays(1) == to.AddSeconds(1);
    }

    public DateTime ConvertUtcToOfficeTime(DateTime utcDateTime, string officeTimeZone)
    {
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcDateTime, officeTimeZone);
    }

    public DateTime ConvertToUtcFromOfficeTime(DateTime dateTime, string officeTimeZone)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(officeTimeZone));
    }

    public OfficeDayRangeCoreDto GetOfficeDayRange(DateOnly officeTimeZoneDay, string officeTimeZone)
    {
        var officeTimeFrom = new DateTime(
            officeTimeZoneDay.Year, officeTimeZoneDay.Month, officeTimeZoneDay.Day,
            0, 0, 0, DateTimeKind.Unspecified);
        var officeTimeTo = new DateTime(officeTimeZoneDay.Year, officeTimeZoneDay.Month, officeTimeZoneDay.Day,
            23, 59, 59, DateTimeKind.Unspecified);

        var fromUtc = ConvertToUtcFromOfficeTime(officeTimeFrom, officeTimeZone);
        var toUtc = ConvertToUtcFromOfficeTime(officeTimeTo, officeTimeZone);

        return new OfficeDayRangeCoreDto
        {
            OfficeTimeZoneDay = officeTimeZoneDay,
            OfficeTimeDayFrom = officeTimeFrom,
            OfficeTimeDayTo = officeTimeTo,
            UtcDayFrom = fromUtc,
            UtcDayTo = toUtc
        };
    }
}
