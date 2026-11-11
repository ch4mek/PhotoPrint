namespace Modsen.Portal.Booking.Requests.Workplaces.Core.CoreDto;

internal record BlockedBookingEntryWithLastReservationDateDto(int BookingEntryId, DateTime LastReservationDate);
