using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Booking.Requests.Booking.Core.Queries;

internal interface IGetWorkPlaceByIdDbQuery
{
    Task<Workplace?> Get(int workplaceId, CancellationToken cancellationToken);
}

internal class GetWorkPlaceByIdDbQuery : IGetWorkPlaceByIdDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetWorkPlaceByIdDbQuery(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<Workplace?> Get(int workplaceId, CancellationToken cancellationToken)
    {
        return await _bookingDbContext
            .Workplaces
            .AsNoTracking()
            .Where(w => w.Id == workplaceId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
