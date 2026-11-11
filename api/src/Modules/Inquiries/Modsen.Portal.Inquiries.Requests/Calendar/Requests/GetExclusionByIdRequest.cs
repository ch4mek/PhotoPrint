using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests;

internal sealed class GetExclusionByIdRequest : IQueryHandler<GetExclusionByIdRequest.Query, CalendarExclusionDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetExclusionByIdRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<CalendarExclusionDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var exclusion = await _inquiriesDbContext.CalendarExclusions
                .Where(exclusion => exclusion.Id == request.Id)
                .AsNoTracking()
                .Select(exclusion => new CalendarExclusionDto
                {
                    Id = exclusion.Id,
                    Name = exclusion.Name,
                    Date = exclusion.Date,
                    NewDate = exclusion.NewDate,
                    DateType = exclusion.DateType,
                    CountryId = exclusion.CountryId,
                })
                .FirstOrDefaultAsync(cancellationToken) ??
            throw new EntityNotFoundException($"Calendar exclusion {request.Id} couldn't be found.");

        return exclusion;
    }

    internal sealed record Query(int Id) : IQuery<CalendarExclusionDto>;
}
