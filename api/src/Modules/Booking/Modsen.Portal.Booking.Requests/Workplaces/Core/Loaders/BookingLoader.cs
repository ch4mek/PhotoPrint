using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;

internal interface IBookingLoader
{
    Task<BookingAndMeetingRoomsDto> LoadAsync(int? officeId, int? workplaceId, CancellationToken cancellationToken);

    Task<BookingAndMeetingRoomsDto> LoadAsync(int? userId, int? officeId, int? workplaceId, CancellationToken cancellationToken);
}

internal sealed class BookingLoader : IBookingLoader
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IBookingStartEndDateLoader _bookingStartEndDateLoader;
    private readonly IBookingDbContext _bookingDbContext;

    public BookingLoader(
        IAuthenticatedUserContext authenticatedUserContext,
        IBookingStartEndDateLoader bookingStartEndDateLoader,
        IBookingDbContext bookingDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _bookingStartEndDateLoader = bookingStartEndDateLoader;
        _bookingDbContext = bookingDbContext;
    }

    public async Task<BookingAndMeetingRoomsDto> LoadAsync(int? officeId, int? workplaceId, CancellationToken cancellationToken)
    {
        var (fromDateToGetBookings, endDateToGetBookings) = await _bookingStartEndDateLoader.LoadAsync(cancellationToken);

        var query = _bookingDbContext.Bookings
            .Where(b => !b.IsDeleted)
            .Include(x => x.Workplace)
                .ThenInclude(f => f!.Workspace)
            .AsNoTracking()
            .AsSplitQuery();

        query = AddBookingPlaceFilters(
            query,
            officeId,
            workplaceId,
            _authenticatedUserContext.DepartmentId,
            _authenticatedUserContext.ResourceGroupId,
            IsIncludeGuests());

        var bookings = await query
            .Where(b => b.Reservations.Any(r => r.ReservationDate >= fromDateToGetBookings.Date &&
                r.ReservationDate <= endDateToGetBookings.Date))
            .OrderByDescending(b => b.IsBlock)
            .ThenBy(b => (b.UserId == _authenticatedUserContext.GetRequiredUserId() && !b.IsForGuest) ? 0 : 1)
            .ThenBy(b => b.Reservations.First().ReservationYear)
            .ThenBy(b => b.Reservations.First().ReservationForDay)
            .Include(b => b.User)
            .Include(b => b.Workplace)
            .Include(b => b.Reservations)
            .Include(b => b.Notifications)
            .Include(b => b.CreatedByUser)
            .Select(b => BookingDto.OfDatabaseEntity(b))
            .ToListAsync(cancellationToken);

        var bookingsDto = GetBookingsDto(bookings);

        return new BookingAndMeetingRoomsDto
        {
            Bookings = bookingsDto
                .Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId() && !x.IsForGuest)
                .OrderBy(x => x.From)
                .Concat(bookingsDto
                    .Where(x => (x.UserId != _authenticatedUserContext.GetRequiredUserId()) ||
                        (x.UserId == _authenticatedUserContext.GetRequiredUserId() && x.IsForGuest))
                    .OrderBy(x => x.From))
                .ToList()
        };
    }

    public async Task<BookingAndMeetingRoomsDto> LoadAsync(int? userId, int? officeId, int? workplaceId, CancellationToken cancellationToken)
    {
        var (fromDateToGetBookings, endDateToGetBookings) = await _bookingStartEndDateLoader.LoadAsync(cancellationToken);

        var currentUserId = _authenticatedUserContext.UserId;
        var resourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager() ? _authenticatedUserContext.ResourceGroupId : null;
        var includeGuests = IsIncludeGuests();

        var query = _bookingDbContext.Bookings
            .Where(b => b.UserId == userId || (b.IsForGuest && includeGuests))
            .Include(x => x.Workplace)
                .ThenInclude(f => f!.Workspace)
            .AsNoTracking()
            .AsSplitQuery();

        query = AddBookingPlaceFilters(
            query,
            officeId,
            workplaceId,
            _authenticatedUserContext.DepartmentId!,
            resourceGroupId,
            includeGuests);

        var bookings = await query
            .Where(b => !b.IsDeleted)
            .Where(b => b.Reservations.Any(r => r.ReservationDate >= fromDateToGetBookings.Date && r.ReservationDate <= endDateToGetBookings.Date))
            .OrderByDescending(b => b.IsBlock)
            .ThenBy(b => b.Reservations.First().ReservationYear)
            .ThenBy(b => b.Reservations.First().ReservationForDay)
            .Include(b => b.User)
            .Include(b => b.Workplace)
            .Include(b => b.Reservations)
            .Include(b => b.Notifications)
            .Include(b => b.CreatedByUser)
            .Select(b => BookingDto.OfDatabaseEntity(b))
            .ToListAsync(cancellationToken);

        var bookingsDto = GetBookingsDto(bookings);

        return new BookingAndMeetingRoomsDto
        {
            Bookings = bookingsDto
                .Where(x => x.UserId == currentUserId)
                .OrderBy(x => x.From)
                .Concat(bookingsDto
                    .Where(x => x.UserId != currentUserId)
                    .OrderBy(x => x.From))
                .ToList()
        };
    }

    private static List<BookingDto> GetBookingsDto(List<BookingDto> bookings)
    {
        foreach (var bookingDto in bookings)
        {
            bookingDto.From = bookingDto.DateList.Count > 0
                ? bookingDto.DateList.Min()
                : DateTime.MinValue;

            if (bookingDto.DateList.Count > 1)
            {
                bookingDto.DateList =
                [
                    bookingDto.From,
                    bookingDto.DateList.Max()
                ];
            }
        }

        return bookings;
    }

    private bool IsIncludeGuests()
    {
        return _authenticatedUserContext.GetRoles().IsCeo()
            || _authenticatedUserContext.GetRoles().IsCto()
            || _authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            || _authenticatedUserContext.GetRoles().IsGroupManager();
    }

    private static IQueryable<BookingEntry> AddBookingPlaceFilters(
        IQueryable<BookingEntry> query,
        int? officeId,
        int? workspaceId,
        string? departmentId,
        int? resourceGroupId,
        bool includeGuests
        )
    {
        if (officeId is not null)
        {
            query = query.Where(x => x.Workplace!.OfficeId == officeId);
        }

        if (workspaceId is not null)
        {
            query = query.Where(x => x.Workplace!.WorkspaceId == workspaceId);
        }

        if (departmentId is not null)
        {
            query = query.Where(b => b.User!.DepartmentId == departmentId
            || (includeGuests && b.IsForGuest));
        }

        if (resourceGroupId is not null)
        {
            query = query.Where(b => b.User!.ResourceGroupId == resourceGroupId
            || (includeGuests && b.IsForGuest));
        }

        return query;
    }
}
