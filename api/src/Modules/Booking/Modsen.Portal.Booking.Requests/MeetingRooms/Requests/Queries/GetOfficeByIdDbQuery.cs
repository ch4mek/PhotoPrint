using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests.Queries;

internal interface IGetOfficeByIdDbQuery
{
    Task<OfficeEntity> Get(int officeId, CancellationToken cancellationToken);
}

internal class GetOfficeByIdDbQuery : IGetOfficeByIdDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetOfficeByIdDbQuery(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<OfficeEntity> Get(int officeId, CancellationToken cancellationToken)
    {
        var office = await _bookingDbContext.Offices.FirstOrDefaultAsync(x => x.Id == officeId, cancellationToken);
        return office ?? throw new EntityNotFoundException($"Office with ID {officeId} not found");
    }
}
