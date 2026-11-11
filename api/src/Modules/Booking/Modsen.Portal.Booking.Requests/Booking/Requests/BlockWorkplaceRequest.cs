using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Notification.Contract.Events;

namespace Modsen.Portal.Booking.Requests.Booking.Requests;

internal sealed class BlockWorkplaceRequest : ICommandHandler<BlockWorkplaceRequest.Command, BookingEntryCoreDto>
{
    private readonly ICommunicationBus _communicationBus;
    private readonly IClockService _clockService;
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetWorkspaceAvailableStatusDbQuery _getWorkspaceAvailableStatusDbQuery;
    private readonly IBookingByDatesResolver _bookingByDatesResolver;
    private readonly IBookingEntryLoader _bookingEntryLoader;
    private readonly IGetUserByIdDbQuery _getUserByIdDbQuery;
    private readonly IGetWorkPlaceByIdDbQuery _getWorkPlaceByIdDbQuery;
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IIsAllowedToBookBusinessRule _isAllowedToBookBusinessRule;

    public BlockWorkplaceRequest(
        ICommunicationBus communicationBus,
        IClockService clockService,
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetWorkspaceAvailableStatusDbQuery getWorkspaceAvailableStatusDbQuery,
        IBookingByDatesResolver bookingByDatesResolver,
        IBookingEntryLoader bookingEntryLoader,
        IGetUserByIdDbQuery getUserByIdDbQuery,
        IGetWorkPlaceByIdDbQuery getWorkPlaceByIdDbQuery,
        IBookingDbContext bookingDbContext,
        IIsAllowedToBookBusinessRule isAllowedToBookBusinessRule)
    {
        _communicationBus = communicationBus;
        _clockService = clockService;
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
        _authenticatedUserContext = authenticatedUserContext;
        _getWorkspaceAvailableStatusDbQuery = getWorkspaceAvailableStatusDbQuery;
        _bookingByDatesResolver = bookingByDatesResolver;
        _bookingEntryLoader = bookingEntryLoader;
        _getUserByIdDbQuery = getUserByIdDbQuery;
        _getWorkPlaceByIdDbQuery = getWorkPlaceByIdDbQuery;
        _bookingDbContext = bookingDbContext;
        _isAllowedToBookBusinessRule = isAllowedToBookBusinessRule;
    }

    public async Task<BookingEntryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var workplace = await _getWorkPlaceByIdDbQuery.Get(request.WorkplaceId, cancellationToken);
        if (workplace == null)
        {
            throw new EntityNotFoundException($"Workplace with id '{request.WorkplaceId}' not found");
        }

        if (workplace.WorkspaceId != null && !await _getWorkspaceAvailableStatusDbQuery.Get(workplace.WorkspaceId.Value, cancellationToken))
        {
            throw new InvalidDataAppException("Booking in this space is currently unavailable.");
        }

        var user = await _getUserByIdDbQuery.Get(request.UserId, cancellationToken);

        var isWorkplaceCanNotBeBlock = user != null &&
            ((_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && user.DepartmentId != _authenticatedUserContext.DepartmentId) ||
            (_authenticatedUserContext.GetRoles().IsGroupManager() && user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId));

        if (isWorkplaceCanNotBeBlock)
        {
            throw new ActionNotAllowedException($"You can't block workspace for user {request.UserId}");
        }

        if (! await _isAllowedToBookBusinessRule.IsCorrespondsToBusiness(request.WorkplaceId, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.DifferentDepartments);
        }

        var startDate = request.StartDate;
        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);
        var dates = DatesGenerator.GetDatesRangeTillDate(startDate, startDate.AddMonths(settings.ReservationsMonthCount)).ToList();

        var selfBookingsIds = await _bookingByDatesResolver.ResolveBookingIdsByDatesAsync(dates, request.WorkplaceId, request.UserId, null, null, cancellationToken);

        if (selfBookingsIds.Length != 0)
        {
            await RemoveRangeAsync(selfBookingsIds, cancellationToken);
        }

        var blocks = await GetBlockedReservationsAsync(dates, workplace.Id, cancellationToken);

        if (blocks.Count != 0)
        {
            throw new EntityConflictException($"Workplace is already blocked from this date '{blocks[0].ReservationDate}'");
        }

        var bookingIds = await _bookingByDatesResolver.ResolveBookingIdsByDatesAsync(dates, null, null, workplace.OfficeId, null, cancellationToken);
        var bookings = await _bookingEntryLoader.Load(bookingIds, cancellationToken);

        var bookingsToDelete = bookings
            .Where(x => (x.UserId == request.UserId && !x.IsForGuest) ||
                x.WorkplaceId == workplace.Id)
            .ToList();
        var bookingsToDeleteIds = bookingsToDelete
            .Select(b => b.BookingId)
            .ToList();

        await RemoveRangeAsync(bookingsToDeleteIds, cancellationToken);

        var fromDate = _clockService.UtcNow;
        var toDate = fromDate.AddYears(1);

        foreach (var bookingEntry in bookingsToDelete.Where(b => b.UserId != request.UserId))
        {
            var outputJson = new WorkplaceBookingCancelledNotificationOutputDto
            {
                DateList = (await GetReservationsAsync(bookingEntry.BookingId, fromDate, toDate, cancellationToken))
                            .OrderBy(x => x)
                            .ToList(),
                WorkplaceNumber = workplace.WorkplaceNumber,
                WorkplaceId = workplace.Id,
                WorkspaceId = workplace.WorkspaceId,
                WorkplaceOfficeId = workplace.OfficeId,
                Message = PortalMessages.BookingHasBeenCancelled,
                BookingEntryId = bookingEntry.BookingId,
                IsBlock = bookingEntry.IsBlock
            };

            var notificationsCreatedEvent = new NotificationsCreatedEvent
            {
                EventId = Guid.CreateVersion7().ToString(),
                Arguments = JsonSerializer.Serialize(outputJson),
                UserIds = new List<int> { bookingEntry.UserId },
                NotificationType = NotificationTypes.WorkplaceBookingCancelled,
                NotificationChannel = NotificationChannels.WebNotification
            };

            await _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
        }

        var newBlockBooking = new BookingEntry
        {
            UserId = request.UserId,
            IsBlock = true,
            WorkplaceId = workplace.Id
        };

        await _bookingDbContext.Bookings.AddAsync(newBlockBooking, cancellationToken);
        await _bookingDbContext.SaveChangesAsync(cancellationToken);

        var reservations = dates
            .Select(date => new Reservation
            {
                ReservationDate = date,
                BookingEntryId = newBlockBooking.Id
            })
            .ToList();

        await AddReservationRangeAsync(reservations, cancellationToken);

        return (await _bookingEntryLoader.Load(newBlockBooking.Id, cancellationToken))!;
    }

    private async Task RemoveRangeAsync(
    IReadOnlyCollection<int> ids,
    CancellationToken cancellationToken)
    {
        await _bookingDbContext.Bookings
            .Where(bookingEntry => ids.Contains(bookingEntry.Id))
            .ForEachAsync(bookingEntry => bookingEntry.IsDeleted = true, cancellationToken);

        await _bookingDbContext.Reservations
            .Where(reservation => ids.Contains(reservation.BookingEntryId))
            .ForEachAsync(reservation => reservation.IsDeleted = true, cancellationToken);

        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<DateOnly>> GetReservationsAsync(
        int bookingEntryId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken)
    {
        var result = await _bookingDbContext
            .Reservations
            .AsNoTracking()
            .Where(x => x.BookingEntryId == bookingEntryId)
            .Where(x => !x.IsDeleted)
            .Where(x => x.ReservationDate.Date >= fromDate.Date && x.ReservationDate.Date <= toDate.Date)
            .Select(x => x.ReservationDate)
            .Distinct()
            .ToListAsync(cancellationToken);

        return result.Select(DateOnly.FromDateTime).ToList();
    }

    private async Task AddReservationRangeAsync(
    IReadOnlyCollection<Reservation> reservations,
    CancellationToken cancellationToken)
    {
        await _bookingDbContext.Reservations.AddRangeAsync(reservations, cancellationToken);
        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<Reservation>> GetBlockedReservationsAsync(
    IReadOnlyCollection<DateTime> dates,
    int workplaceId,
    CancellationToken cancellationToken)
    {
        return await _bookingDbContext
            .Reservations
            .AsNoTracking()
            .Where(r => r.BookingEntry!.WorkplaceId == workplaceId)
            .Where(r => r.BookingEntry!.IsBlock)
            .Where(r => !r.IsDeleted)
            .Where(r => dates.Any(d => d == r.ReservationDate))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Command : ICommand<BookingEntryCoreDto>, IBookingRequest, ITransactionalRequest
    {
        public int WorkplaceId { get; init; }
        public DateTime StartDate { get; init; }
        public int UserId { get; init; }
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator(IClockService clock)
        {
            RuleFor(x => x.UserId).NotNull();
            RuleFor(x => x.StartDate.Date)
                .GreaterThanOrEqualTo(clock.UtcNow.Date)
                .WithMessage("StartDate should be greater than or equal to current date");

            RuleFor(x => x.StartDate.TimeOfDay).Equal(TimeSpan.Zero);
        }
    }
}
