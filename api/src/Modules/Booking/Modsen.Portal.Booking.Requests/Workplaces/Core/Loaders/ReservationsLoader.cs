using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Booking.Requests.Workplaces.Requests.RequestDto;

namespace Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;

internal interface IReservationsLoader
{
    Task<List<ReservationDto>> LoadAsync(int[] reservationIds, CancellationToken cancellationToken);
}

internal class ReservationsLoader  : IReservationsLoader
{
    private readonly IBookingDbContext _bookingDbContext;

    public ReservationsLoader(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public Task<List<ReservationDto>> LoadAsync(int[] reservationIds, CancellationToken cancellationToken)
    {
        return _bookingDbContext.Reservations
            .Where(x => reservationIds.Contains(x.Id))
            .Include(reservation => reservation.BookingEntry)
            .ThenInclude(bookingEntry => bookingEntry!.Workplace)
            .Include(reservation => reservation.BookingEntry)
            .ThenInclude(bookingEntry => bookingEntry!.User)
            .Include(reservation => reservation.BookingEntry)
            .ThenInclude(bookingEntry => bookingEntry!.CreatedByUser)
            .GroupBy(x => x.ReservationDate)
            .Select(reservationGroup => new ReservationDto
                {
                    Date = reservationGroup.Key,
                    UserWorkplaces = reservationGroup.Select(
                        reservation => new UserWorkplaceDto
                        {
                            BookingId = reservation.BookingEntry!.Id,
                            FirstName = reservation.BookingEntry!.User!.FirstName,
                            LastName = reservation.BookingEntry!.User!.LastName,
                            FirstNameRu = reservation.BookingEntry!.User!.FirstNameRu,
                            LastNameRu = reservation.BookingEntry!.User!.LastNameRu,
                            GuestName = reservation.BookingEntry.GuestName,
                            BookedBy = reservation.BookingEntry!.CreatedByUser!.LastName + " " + reservation.BookingEntry!.CreatedByUser!.FirstName,
                            IsForGuest = reservation.BookingEntry.IsForGuest,
                            DepartmentId = reservation.BookingEntry.User.DepartmentId!,
                            UserId = reservation.BookingEntry.User.UserId,
                            WorkplaceNumber = reservation.BookingEntry.Workplace!.WorkplaceNumber,
                            WorkplaceId = reservation.BookingEntry.Workplace.Id,
                            WorkplaceOfficeId = reservation.BookingEntry.Workplace.OfficeId,
                            IsBlocked = reservation.BookingEntry.IsBlock
                        }).ToList()
                })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
    }
}
