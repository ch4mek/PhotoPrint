using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Booking.Requests.Booking.Core;

internal interface IBookingEntryComposer
{
    Task<BookingEntry> Compose(BookingEntryComposerRequest request, CancellationToken cancellationToken);
}

internal class BookingEntryComposer : IBookingEntryComposer
{
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IAreCollisionsExistBusinessRule _areCollisionsExistBusinessRule;
    private readonly IGetWorkPlaceByIdDbQuery _getWorkPlaceByIdDbQuery;
    private readonly IGetWorkspaceAvailableStatusDbQuery _getWorkspaceAvailableStatusDbQuery;
    private readonly IBookingDbContext _bookingDbContext;

    public BookingEntryComposer(
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor,
        IAuthenticatedUserContext authenticatedUserContext,
        IAreCollisionsExistBusinessRule areCollisionsExistBusinessRule,
        IGetWorkPlaceByIdDbQuery getWorkPlaceByIdDbQuery,
        IGetWorkspaceAvailableStatusDbQuery getWorkspaceAvailableStatusDbQuery,
        IBookingDbContext bookingDbContext)
    {
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
        _authenticatedUserContext = authenticatedUserContext;
        _areCollisionsExistBusinessRule = areCollisionsExistBusinessRule;
        _getWorkPlaceByIdDbQuery = getWorkPlaceByIdDbQuery;
        _getWorkspaceAvailableStatusDbQuery = getWorkspaceAvailableStatusDbQuery;
        _bookingDbContext = bookingDbContext;
    }

    public async Task<BookingEntry> Compose(BookingEntryComposerRequest request, CancellationToken cancellationToken)
    {
        var workplace = await _getWorkPlaceByIdDbQuery.Get(request.WorkplaceId, cancellationToken) ??
            throw new EntityNotFoundException($"Cannot find workspace by ID {request.WorkplaceId}");

        await ValidateRequest(request, workplace, cancellationToken);

        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var userId = request.UserId ?? _authenticatedUserContext.GetRequiredUserId();

        if (!request.IsForGuest)
        {
            await _areCollisionsExistBusinessRule.AreCollisionsForUserByDatesExist(
                request.WeeksInInterval,
                userId,
                workplace.Id,
                workplace.OfficeId,
                dates,
                cancellationToken);
        }
        else
        {
            await _areCollisionsExistBusinessRule.AreCollisionsForGuestByDatesExist(
                request.WeeksInInterval,
                request.GuestName,
                workplace.Id,
                workplace.OfficeId,
                dates,
                cancellationToken);
        }

        int? userIdOrNullForGuest = request.IsForGuest ? null : userId;

        await _areCollisionsExistBusinessRule.AreCollisionsForWorkplaceByDatesExist(
            request.WeeksInInterval,
            userId: userIdOrNullForGuest,
            guestName: request.GuestName,
            workplace.Id,
            dates,
            cancellationToken);

        var bookingEntry = new BookingEntry
        {
            WorkplaceId = workplace.Id,
            UserId = userId,
            GuestName = request.GuestName,
            IsForGuest = request.IsForGuest,
            WeeksInInterval = request.WeeksInInterval
        };

        await RemoveReservationsInRangeAsync(
            request.FromDate,
            request.ToDate,
            userIdOrNullForGuest,
            bookingEntry.WorkplaceId,
            request.GuestName,
            cancellationToken);

        await RemoveUserBookingsAsync(
            userIdOrNullForGuest,
            request.GuestName,
            cancellationToken);

        await AddBookingEntityAsync(bookingEntry, cancellationToken);

        await AddReservationRangeAsync(dates, bookingEntry.Id, cancellationToken);

        return bookingEntry;
    }

    private async Task ValidateRequest(BookingEntryComposerRequest request, Workplace? workplace, CancellationToken cancellationToken)
    {
        if (request is { IsForGuest: false, WeeksInInterval: > 0 } && request.ToDate.Date != request.FromDate.Date)
        {
            throw new EntityConflictException($"Booking can be repeated after {request.WeeksInInterval} weeks when from and to dates are equal");
        }

        if (workplace == null)
        {
            throw new EntityNotFoundException(PortalMessages.NotExistingWorkspace);
        }
        if (workplace.WorkspaceId != null && !await _getWorkspaceAvailableStatusDbQuery.Get(workplace.WorkspaceId.Value, cancellationToken))
        {
            throw new InvalidDataAppException("Booking in this space is currently unavailable.");
        }
    }

    private async Task AddBookingEntityAsync(
    BookingEntry bookingEntry,
    CancellationToken cancellationToken)
    {
        await _bookingDbContext.Bookings.AddAsync(bookingEntry, cancellationToken);
        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddReservationRangeAsync(
        List<DateTime> dates,
        int bookingId,
        CancellationToken cancellationToken)
    {
        var reservations = dates.Select(date => new Reservation
        {
            ReservationDate = date,
            BookingEntryId = bookingId
        })
        .ToList();

        await _bookingDbContext.Reservations.AddRangeAsync(reservations, cancellationToken);
        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task RemoveReservationsInRangeAsync(
        DateTime startDate,
        DateTime endDate,
        int? userId,
        int workplaceId,
        string? guestName,
        CancellationToken cancellationToken)
    {
        await _bookingDbContext.Reservations
            .Include(reservation => reservation.BookingEntry)
            .Where(reservation => userId == null || reservation.BookingEntry!.UserId == userId)
            .Where(reservation => guestName == null || reservation.BookingEntry!.GuestName == guestName)
            .Where(reservation => reservation.BookingEntry!.WorkplaceId == workplaceId &&
                                  reservation.ReservationDate >= startDate &&
                                  reservation.ReservationDate <= endDate)
            .ForEachAsync(reservation => reservation.IsDeleted = true, cancellationToken);

        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task RemoveUserBookingsAsync(
        int? userId,
        string? guestName,
        CancellationToken cancellationToken)
    {
        await _bookingDbContext.Bookings
            .Include(bookingEntry => bookingEntry.Reservations)
            .Where(bookingEntry => userId == null || bookingEntry.UserId == userId)
            .Where(bookingEntry => guestName == null || bookingEntry.GuestName == guestName)
            .Where(bookingEntry => !bookingEntry.IsDeleted && bookingEntry.Reservations.All(reservation => reservation.IsDeleted))
            .ForEachAsync(bookingEntry => bookingEntry.IsDeleted = true, cancellationToken);

        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }
}

internal class BookingEntryComposerRequest
{
    public int? UserId { get; set; }
    public int WorkplaceId { get; init; }
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public int WeeksInInterval { get; init; }
    public bool IsForGuest { get; init; }
    public string? GuestName { get; init; }
}
