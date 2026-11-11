using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.BusinessRules;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Notification.Contract.Events;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal sealed class DeleteMeetingRoomEntryRequest : ICommandHandler<DeleteMeetingRoomEntryRequest.Command, int[]>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IMeetingRoomAccessBusinessRule _meetingRoomAccessManager;
    private readonly IBookingDbContext _bookingDbContext;
    private readonly ITimeZoneConverter _officeTimeManager;
    private readonly ICommunicationBus _communicationBus;

    public DeleteMeetingRoomEntryRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IMeetingRoomAccessBusinessRule meetingRoomAccessManager,
        IBookingDbContext bookingDbContext,
        ITimeZoneConverter officeTimeManager,
        ICommunicationBus communicationBus)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _meetingRoomAccessManager = meetingRoomAccessManager;
        _bookingDbContext = bookingDbContext;
        _officeTimeManager = officeTimeManager;
        _communicationBus = communicationBus;
    }

    public async Task<int[]> Handle(Command request, CancellationToken cancellationToken)
    {
        var entry = await GetEntry(request.MeetingRoomEntryId, cancellationToken);
        var officeTimeZone = entry.MeetingRoom?.Office?.TimeZone;

        ArgumentException.ThrowIfNullOrEmpty(officeTimeZone);

        await _meetingRoomAccessManager.VerifyCurrentUserCanDeleteEntryForUserId(entry.UserId, cancellationToken);
        await RemoveEntryWithReservations(entry, cancellationToken);

        if (_authenticatedUserContext.IsManager &&
            _authenticatedUserContext.UserId != entry.UserId)
        {
            await SendNotificationToEntryUser(entry, officeTimeZone, cancellationToken);
        }

        return entry.MeetingRoomReservations.Select(x => x.Id).ToArray();
    }

    private Task SendNotificationToEntryUser(MeetingRoomEntryEntity entry, string timeZone, CancellationToken cancellationToken)
    {
        var dates = entry.MeetingRoomReservations
            .Select(x => _officeTimeManager.ConvertUtcToOfficeTime(x.ReservationFromUtc, timeZone).Date)
            .ToList();
        var output = new
        {
            Message = PortalMessages.BookingIsNoLongerAvailable,
            MeetingRoomEntryId = entry.Id,
            DateList = dates,
            entry.MeetingRoomId,
            MeetingRoomName = entry.MeetingRoom!.Name,
            FromTime = _officeTimeManager.ConvertUtcToOfficeTime(entry.ReservationFromUtc, timeZone),
            ToTime = _officeTimeManager.ConvertUtcToOfficeTime(entry.ReservationToUtc, timeZone)
        };

        var notificationsCreatedEvent = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            Arguments = JsonSerializer.Serialize(output),
            UserIds = [entry.UserId],
            NotificationChannel = NotificationChannels.SignalR,
            NotificationType = NotificationTypes.MeetingRoomBookingCancelled
        };

        return _communicationBus.PublishAsync(notificationsCreatedEvent, cancellationToken);
    }

    private async Task<MeetingRoomEntryEntity> GetEntry(int meetingRoomEntryId, CancellationToken token)
    {
        var entry = await _bookingDbContext.MeetingRoomEntries
            .Include(x => x.MeetingRoomReservations.Where(f => !f.IsDeleted))
            .Include(x => x.MeetingRoom)
            .ThenInclude(f => f!.Office)
            .Where(x => x.Id == meetingRoomEntryId)
            .FirstOrDefaultAsync(token);
        return entry ?? throw new EntityNotFoundException($"Meeting room entry by ID {meetingRoomEntryId} not found");
    }

    private Task<int> RemoveEntryWithReservations(MeetingRoomEntryEntity entry, CancellationToken cancellationToken)
    {
        foreach (var reservation in entry.MeetingRoomReservations)
        {
            reservation.IsDeleted = true;
        }

        entry.IsDeleted = true;

        _bookingDbContext.MeetingRoomReservations.UpdateRange(entry.MeetingRoomReservations);
        _bookingDbContext.MeetingRoomEntries.Update(entry);

        return _bookingDbContext.SaveChangesAsync(cancellationToken);
    }

    internal sealed class Command : ICommand<int[]>, IBookingRequest, ITransactionalRequest
    {
        public int MeetingRoomEntryId { get; set; }
    }
}
