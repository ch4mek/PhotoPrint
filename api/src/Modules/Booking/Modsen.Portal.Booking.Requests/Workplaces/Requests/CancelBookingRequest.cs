using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Workplaces.Requests.DbQueries;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.Contract.Events;

namespace Modsen.Portal.Booking.Requests.Workplaces.Requests;

internal sealed class CancelBookingRequest : ICommandHandler<CancelBookingRequest.Command, int>
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly ICanCurrentUserDeleteBooking _canCurrentUserDeleteBooking;
    private readonly IRemoveBookingsRangeDbQuery _removeBookingsRangeDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly ICommunicationBus _communicationBus;

    public CancelBookingRequest(
        IBookingDbContext bookingDbContext,
        ICanCurrentUserDeleteBooking canCurrentUserDeleteBooking,
        IRemoveBookingsRangeDbQuery removeBookingsRangeDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        ICommunicationBus communicationBus)
    {
        _bookingDbContext = bookingDbContext;
        _canCurrentUserDeleteBooking = canCurrentUserDeleteBooking;
        _removeBookingsRangeDbQuery = removeBookingsRangeDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _communicationBus = communicationBus;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var booking = await _bookingDbContext.Bookings
            .Where(bookingEntity => bookingEntity.Id == request.BookingId)
            .Select(bookingEntity => new
            {
                bookingEntity.Id,
                bookingEntity.IsBlock,
                bookingEntity.UserId,
                bookingEntity.IsForGuest,
                UserResourceGroup = bookingEntity.User!.ResourceGroupId,
                UserDepartment = bookingEntity.User!.DepartmentId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
        {
            throw new EntityNotFoundException($"Booking with id = {request.BookingId} does not exists");
        }

        var (canDelete, message) = _canCurrentUserDeleteBooking.CanDelete(new(
            booking.UserId, booking.UserResourceGroup, booking.UserDepartment, booking.IsForGuest, booking.IsBlock));

        if (!canDelete)
        {
            throw new ActionNotAllowedException(message!);
        }

        if (booking.UserId == _authenticatedUserContext.UserId)
        {
            await _removeBookingsRangeDbQuery.RemoveAsync([booking.Id], cancellationToken);
        }
        else if (_authenticatedUserContext.IsManager)
        {
            if (!booking.IsForGuest)
            {
                await SendNotificationAsync(booking.Id, booking.UserId, cancellationToken);
            }

            await _removeBookingsRangeDbQuery.RemoveAsync([booking.Id], cancellationToken);
        }
        else
        {
            throw new ActionNotAllowedException(PortalMessages.NotHavePermissionToCancelBooking);
        }

        return booking.Id;
    }

    private async Task SendNotificationAsync(int bookingId, int userId, CancellationToken cancellationToken)
    {
        var workplace = await _bookingDbContext.Workplaces
            .AsNoTracking()
            .Where(workplaceEntity => workplaceEntity.BookingEntries.Any(bookingEntity => bookingEntity.Id == bookingId))
            .Select(workplaceEntity => new
            {
                workplaceEntity.Id,
                workplaceEntity.WorkspaceId,
                workplaceEntity.WorkplaceNumber,
                workplaceEntity.OfficeId
            })
            .FirstOrDefaultAsync(cancellationToken);

        var currentDate = _clockService.UtcNow;

        var reservationDates = await _bookingDbContext.Reservations
            .AsNoTracking()
            .Where(reservationEntity => reservationEntity.BookingEntryId == bookingId)
            .Where(reservationEntity => !reservationEntity.IsDeleted)
            .Where(reservationEntity => reservationEntity.ReservationDate.Date >= currentDate.Date &&
                reservationEntity.ReservationDate.Date <= currentDate.AddYears(1).Date)
            .Select(reservationEntity => DateOnly.FromDateTime(reservationEntity.ReservationDate))
            .Distinct()
            .OrderBy(reservationDate => reservationDate)
            .ToListAsync(cancellationToken);

        var output = new WorkplaceBookingCancelledNotificationOutputDto
        {
            DateList = reservationDates,
            WorkplaceNumber = workplace!.WorkplaceNumber,
            WorkplaceId = workplace.Id,
            WorkspaceId = workplace.WorkspaceId,
            WorkplaceOfficeId = workplace.OfficeId,
            Message = PortalMessages.BookingHasBeenCancelled,
            BookingEntryId = bookingId,
        };

        var notificationsCreatedEvent = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            Arguments = JsonSerializer.Serialize(output),
            UserIds = [userId],
            NotificationType = NotificationTypes.WorkplaceBookingCancelled,
            NotificationChannel = NotificationChannels.WebNotification
        };

        await _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
    }

    internal sealed record Command(int BookingId) : ICommand<int>, IBookingRequest, ITransactionalRequest;
}
