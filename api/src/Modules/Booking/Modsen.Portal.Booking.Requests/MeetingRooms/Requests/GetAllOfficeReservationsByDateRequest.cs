using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests.Queries;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests.RequestDto;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal sealed class GetAllOfficeReservationsByDateRequest : IQueryHandler<GetAllOfficeReservationsByDateRequest.Query, MeetingRoomReservationGroupRequestDto[]>
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly ITimeZoneConverter _officeTimeManager;
    private readonly IGetOfficeByIdDbQuery _getOfficeByIdDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IMeetingRoomReservationLoader _meetingRoomReservationLoader;

    public GetAllOfficeReservationsByDateRequest(IBookingDbContext bookingDbContext,
        ITimeZoneConverter officeTimeManager,
        IGetOfficeByIdDbQuery getOfficeByIdDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IMeetingRoomReservationLoader meetingRoomReservationLoader)
    {
        _bookingDbContext = bookingDbContext;
        _officeTimeManager = officeTimeManager;
        _getOfficeByIdDbQuery = getOfficeByIdDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _meetingRoomReservationLoader = meetingRoomReservationLoader;
    }

    public async Task<MeetingRoomReservationGroupRequestDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var office = await _getOfficeByIdDbQuery.Get(request.OfficeId, cancellationToken);
        var dayRange = _officeTimeManager.GetOfficeDayRange(request.DayInOfficeTimeZone, office.TimeZone!);
        var existingReservationIds = await _bookingDbContext.MeetingRoomReservations
            .Where(x => x.MeetingRoomEntry!.MeetingRoom!.OfficeId == request.OfficeId)
            .Where(x => x.MeetingRoomEntry!.MeetingRoom!.IsActive)
            .Where(x => !x.IsDeleted)
            .Where(x =>
                (x.ReservationFromUtc >= dayRange.UtcDayFrom && x.ReservationFromUtc <= dayRange.UtcDayTo) ||
                (x.ReservationToUtc >= dayRange.UtcDayFrom && x.ReservationToUtc <= dayRange.UtcDayTo))
            .Select(x => x.Id)
            .Distinct()
            .ToArrayAsync(cancellationToken);
        var reservations = await GetReservations(existingReservationIds, cancellationToken);
        var meetingRooms = await GetMeetingRooms(request.OfficeId, cancellationToken);

        return meetingRooms
            .Select(room => new MeetingRoomReservationGroupRequestDto
            {
                MeetingRoomId = room.Id,
                MeetingRoomName = room.Name,
                CountOfSeats = room.CountOfSeats,
                Equipments = room.Items.Select(item=> new MeetingRoomItemRequestDto
                {
                    Id = item.Id,
                    Count = item.Count,
                    Name = item.Name,
                    Type = item.ItemType,
                }).ToArray(),
                Reservations = reservations.Where(res => res.MeetingRoomId == room.Id).ToArray()
            })
            .ToArray();
    }

    private async Task<MeetingRoomReservationCoreDto[]> GetReservations(int[] reservationIds, CancellationToken token)
    {
        var reservations = await _meetingRoomReservationLoader.Load(reservationIds, token);
        var currentUser = _authenticatedUserContext.UserId;

        return reservations
            .OrderByDescending(x => x.UserId == currentUser ? 1 : 0)
            .ThenBy(x => x.ReservationFromUtc)
            .ToArray();
    }

    private Task<MeetingRoomEntity[]> GetMeetingRooms(int officeId, CancellationToken cancellationToken)
    {
        return _bookingDbContext.MeetingRooms
            .Where(x => x.OfficeId == officeId)
            .Where(x => x.IsActive)
            .Include(x=>x.Items)
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<MeetingRoomReservationGroupRequestDto[]>, IBookingRequest
    {
        public DateOnly DayInOfficeTimeZone { get; set; }
        public int OfficeId { get; set; }
    }
}
