using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests.RequestDto;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal sealed class GetOfficeWorkplacesRequest : IQueryHandler<GetOfficeWorkplacesRequest.Query, OfficeWorkplaceRequestDto[]>
{
    private readonly IBookingDbContext _bookingDbContext;

    public GetOfficeWorkplacesRequest(IBookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<OfficeWorkplaceRequestDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var workplaces = await _bookingDbContext.Workplaces
            .Where(x => x.OfficeId == request.OfficeId)
            .Select(x => new OfficeWorkplaceRequestDto
            {
                Id = x.Id,
                WorkspaceId = x.WorkspaceId,
                WorkplaceNumber = x.WorkplaceNumber,
            })
            .ToArrayAsync(cancellationToken);

        return workplaces
            .OrderBy(x => x.WorkspaceId ?? 0)
            .ThenBy(x => x.WorkplaceNumber)
            .ToArray();
    }

    internal sealed record Query(int OfficeId) : IQuery<OfficeWorkplaceRequestDto[]>, IBookingRequest;
}
