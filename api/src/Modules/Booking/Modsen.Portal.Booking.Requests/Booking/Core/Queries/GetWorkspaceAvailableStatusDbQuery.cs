using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;

namespace Modsen.Portal.Booking.Requests.Booking.Core.Queries;

internal interface IGetWorkspaceAvailableStatusDbQuery
{
    Task<bool> Get(int workspaceId, CancellationToken cancellationToken);
}

internal class GetWorkspaceAvailableStatusDbQuery : IGetWorkspaceAvailableStatusDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetWorkspaceAvailableStatusDbQuery(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<bool> Get(int workspaceId, CancellationToken cancellationToken)
    {
        return await _bookingDbContext.Workspaces
            .Where(x => x.Id == workspaceId)
            .Select(x => x.IsAvailableForBooking)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
