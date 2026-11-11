using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal sealed class GetOfficesClassificatorRequest : IQueryHandler<GetOfficesClassificatorRequest.Query, OfficeClassificationDto[]>
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetOfficesClassificatorRequest(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public Task<OfficeClassificationDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        return _bookingDbContext.Offices
            .AsNoTracking()
            .Include(x => x.Workspaces)
            .Include(x => x.MeetingRooms.Where(f => f.IsActive))
            .AsSplitQuery()
            .Where(x => x.LocationType == LocationType.Minsk || x.LocationType == LocationType.Warsaw /*|| x.LocationType == LocationType.Vitebsk*/)
            .Select(office => new OfficeClassificationDto
            {
                Id = office.Id,
                Name = office.Name,
                Location = office.LocationType,
                TimeZone = office.TimeZone!,
                TimeZoneDisplayName = office.TimeZoneDisplayName!,
                Workspaces = office.Workspaces
                    .Select(x => new OfficeWorkspaceClassificationDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        InternalCode = x.InternalCode
                    })
                    .ToArray(),
                MeetingRooms = office.MeetingRooms
                    .Select(x => new MeetingRoomClassificationDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToArray()
            })
            .ToArrayAsync(cancellationToken);
    }

    internal sealed class Query : IQuery<OfficeClassificationDto[]>, IBookingRequest;
}
