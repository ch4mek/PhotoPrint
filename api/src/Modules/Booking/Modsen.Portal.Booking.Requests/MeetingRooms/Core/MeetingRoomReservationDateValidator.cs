using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Core;

internal interface IMeetingRoomReservationDateValidator
{
    void Validate(DateTime reservationFromUtc, DateTime reservationToUtc, string timeZone);
}

internal class MeetingRoomReservationDateValidator : IMeetingRoomReservationDateValidator
{
    private readonly ITimeZoneConverter _officeTimeManager;

    public MeetingRoomReservationDateValidator(ITimeZoneConverter officeTimeManager)
    {
        _officeTimeManager = officeTimeManager;
    }

    public void Validate(DateTime reservationFromUtc, DateTime reservationToUtc, string timeZone)
    {
        if (_officeTimeManager.IsAllDay(reservationFromUtc, reservationToUtc))
        {
            var reservationOfficeTime = _officeTimeManager.ConvertUtcToOfficeTime(reservationFromUtc, timeZone);
            var startOfDayOfficeTime = new DateTime(reservationOfficeTime.Year, reservationOfficeTime.Month,
                reservationOfficeTime.Day, 0, 0, 0, DateTimeKind.Unspecified);
            var startDayUtc = _officeTimeManager.ConvertToUtcFromOfficeTime(startOfDayOfficeTime, timeZone);
            if (reservationFromUtc < startDayUtc)
            {
                throw new AppException(PortalMessages.ConflictingReservationTime);
            }
        }
    }
}
