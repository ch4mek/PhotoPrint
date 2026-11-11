using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Booking.Requests.Booking.Core.Queries;

internal interface IGetUserByIdDbQuery
{
    Task<BookingUserEntity?> Get(int userId, CancellationToken cancellationToken);
}

internal class GetUserByIdDbQuery : IGetUserByIdDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetUserByIdDbQuery(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<BookingUserEntity?> Get(int userId, CancellationToken cancellationToken)
    {
        return await _bookingDbContext.BookingUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
