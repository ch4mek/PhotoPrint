using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface IMeetingRoomReservationDateGenerator
{
    IEnumerable<DateRangeCoreDto> GenerateReservationDates(
        MeetingRoomReservationDateGeneratorRequest request);

    IEnumerable<DateRangeCoreDto> GenerateReservationDatesUntil(
        DateOnly untilDate, MeetingRoomReservationDateGeneratorRequest request);
}

internal class MeetingRoomReservationDateGenerator : IMeetingRoomReservationDateGenerator
{
    private readonly ITimeZoneConverter _officeTimeManager;

    public MeetingRoomReservationDateGenerator(ITimeZoneConverter officeTimeManager)
    {
        _officeTimeManager = officeTimeManager;
    }

    public IEnumerable<DateRangeCoreDto> GenerateReservationDates(MeetingRoomReservationDateGeneratorRequest request)
    {
        var maxDate = request.ReservationPeriodFromOfficeDate.AddMonths(request.ReservationsMonthCount);
        return Generate(maxDate, request);        
    }

    public IEnumerable<DateRangeCoreDto> GenerateReservationDatesUntil(DateOnly untilDate, MeetingRoomReservationDateGeneratorRequest request)
    {
        return Generate(untilDate, request);
    }

    private IEnumerable<DateRangeCoreDto> Generate(DateOnly maxDate, MeetingRoomReservationDateGeneratorRequest request)
    {
        if (request.RepeatReservationWeeks is > 0)
        {
            var stepInDays = request.RepeatReservationWeeks.Value * 7;
            for (var date = request.ReservationPeriodFromOfficeDate;
                 date <= maxDate;
                 date = date.AddDays(stepInDays))
            {
                foreach (var generatedDate in GenerateInternal(request))
                {
                    yield return generatedDate;
                }

                request.ReservationPeriodFromOfficeDate = request.ReservationPeriodFromOfficeDate.AddDays(stepInDays);
                request.ReservationPeriodToOfficeDate = request.ReservationPeriodToOfficeDate.AddDays(stepInDays);
            }
        }
        else
        {
            foreach (var date in GenerateInternal(request))
            {
                yield return date;
            }
        }
    }

    private IEnumerable<DateRangeCoreDto> GenerateInternal(MeetingRoomReservationDateGeneratorRequest request)
    {
        var timeZone = request.OfficeTimeZone;
        var officeTimeFrom = _officeTimeManager.ConvertUtcToOfficeTime(request.ReservationFromUtc, timeZone);
        var officeTimeTo = _officeTimeManager.ConvertUtcToOfficeTime(request.ReservationToUtc, timeZone);
        var isAllDay = _officeTimeManager.IsAllDay(officeTimeFrom, officeTimeTo);

        for (var reservationDate = request.ReservationPeriodFromOfficeDate;
             reservationDate <= request.ReservationPeriodToOfficeDate;
             reservationDate = reservationDate.AddDays(1))
        {
            if (isAllDay)
            {
                yield return new DateRangeCoreDto
                {
                    FromUtc = BuildUtcDateTime(reservationDate, TimeOnly.MinValue, timeZone),
                    ToUtc = BuildUtcDateTime(reservationDate, new TimeOnly(23, 59, 59), timeZone)
                };
            }
            else
            {
                yield return new DateRangeCoreDto
                {
                    FromUtc = BuildUtcDateTime(reservationDate, GetTime(officeTimeFrom), timeZone),
                    ToUtc = BuildUtcDateTime(reservationDate, GetTime(officeTimeTo), timeZone)
                };
            }
        }
    }

    private static TimeOnly GetTime(DateTime dateTime)
    {
        return new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second);
    }

    private DateTime BuildUtcDateTime(DateOnly date, TimeOnly time, string officeTimeZone)
    {
        var officeDateTime = new DateTime(date.Year, date.Month, date.Day,
            time.Hour, time.Minute, time.Second, DateTimeKind.Unspecified);

        return _officeTimeManager.ConvertToUtcFromOfficeTime(officeDateTime, officeTimeZone);
    }
}

internal record MeetingRoomReservationDateGeneratorRequest
{
    public required string OfficeTimeZone { get; init; }
    public DateTime ReservationFromUtc { get; init; }
    public DateTime ReservationToUtc { get; init; }
    public DateOnly ReservationPeriodFromOfficeDate { get; set; }
    public DateOnly ReservationPeriodToOfficeDate { get; set; }
    public int? RepeatReservationWeeks { get; init; }
    public int ReservationsMonthCount { get; init; }
}
