using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal sealed class GetAllOfficeEntriesFromTodayRequest : IQueryHandler<GetAllOfficeEntriesFromTodayRequest.Query, MeetingRoomEntryCoreDto[]>
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IMeetingRoomEntryLoader _meetingRoomEntryLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;

    public GetAllOfficeEntriesFromTodayRequest(
        IBookingDbContext bookingDbContext,
        IMeetingRoomEntryLoader meetingRoomEntryLoader,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService)
    {
        _bookingDbContext = bookingDbContext;
        _meetingRoomEntryLoader = meetingRoomEntryLoader;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
    }

    public async Task<MeetingRoomEntryCoreDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var entryIds = await GetMeetingRoomEntryIds(request, cancellationToken);
        var meetingRoomEntries = await _meetingRoomEntryLoader.Load(entryIds, cancellationToken);
        var currentUserId = _authenticatedUserContext.UserId;

        return meetingRoomEntries
            .OrderByDescending(x => x.UserId == currentUserId ? 1 : 0)
            .ThenBy(x => x.ReservationFromUtc)
            .ToArray();
    }

    private Task<int[]> GetMeetingRoomEntryIds(Query request, CancellationToken token)
    {
        var currentUserId = _authenticatedUserContext.UserId;
        var query = _bookingDbContext.MeetingRoomEntries
            .Where(entry => entry.MeetingRoom!.OfficeId == request.OfficeId)
            .Where(entry => entry.MeetingRoom!.IsActive);

        if (_authenticatedUserContext.GetRoles().IsEmployee() || _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(x => x.UserId == currentUserId);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            var currentDepartmentId = _authenticatedUserContext.DepartmentId;
            query = query.Where(x =>
                x.UserId == currentUserId ||
                x.User!.DepartmentId == currentDepartmentId);
        }

        if(_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            var currentGroupId = _authenticatedUserContext.ResourceGroupId;
            query = query.Where(x =>
                x.UserId == currentUserId ||
                x.User!.ResourceGroupId == currentGroupId);
        }

        return query
            .Where(entry => !entry.IsDeleted)
            .Where(entry => entry.MeetingRoomReservations.Any(x =>                
                x.ReservationToUtc >= _clockService.UtcNow))
            .Select(x => x.Id)
            .Distinct()
            .ToArrayAsync(token);
    }

    internal sealed record Query : IQuery<MeetingRoomEntryCoreDto[]>, IBookingRequest
    {
        public int OfficeId { get; set; }
    }
}
