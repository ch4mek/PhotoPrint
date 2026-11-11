using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests;

internal sealed class GetCalendarNameLookup : IQueryHandler<GetCalendarNameLookup.Query, List<CalendarNameDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetCalendarNameLookup(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }
    public Task<List<CalendarNameDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _inquiriesDbContext.CalendarExclusions
            .Where(calendar => calendar.Name != null)
            .Select(calendar => new CalendarNameDto
            {
                Id = calendar.Id,
                Name = calendar.Name!,
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<CalendarNameDto>>;
}
