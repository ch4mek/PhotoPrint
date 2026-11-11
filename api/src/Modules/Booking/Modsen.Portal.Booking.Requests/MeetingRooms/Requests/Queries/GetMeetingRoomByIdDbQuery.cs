using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests.Queries;

internal interface IGetMeetingRoomByIdDbQuery
{
    Task<MeetingRoomEntity> Get(int meetingRoomId, CancellationToken cancellationToken);
}

internal class GetMeetingRoomByIdDbQuery : IGetMeetingRoomByIdDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetMeetingRoomByIdDbQuery(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<MeetingRoomEntity> Get(int meetingRoomId, CancellationToken cancellationToken)
    {
        var meetingRoom = await _bookingDbContext.MeetingRooms
            .Include(x => x.Office)
            .Where(x => x.Id == meetingRoomId)
            .FirstOrDefaultAsync(cancellationToken);
        return meetingRoom ?? throw new EntityNotFoundException($"Meeting room with ID {meetingRoomId} not found");
    }
}
