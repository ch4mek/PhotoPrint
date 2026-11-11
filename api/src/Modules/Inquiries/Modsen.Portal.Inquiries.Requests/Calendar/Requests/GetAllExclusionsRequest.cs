using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Calendar.Requests;

internal sealed class GetAllExclusionsRequest : IQueryHandler<GetAllExclusionsRequest.Query, PageableListOfItems<CalendarExclusionCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetAllExclusionsRequest(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<PageableListOfItems<CalendarExclusionCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;
        var query = _inquiriesDbContext.CalendarExclusions
             .AsNoTracking()
            .Select(c => new CalendarExclusionCoreDto
            {
                Id = c.Id,
                Name = c.Name,
                Date = c.Date,
                NewDate = c.NewDate,
                DateType = c.DateType.ToString(),
                CountryId = c.CountryId
            });

        var queryResult = await queryOptions
               .ApplyTo(query)
               .Cast<CalendarExclusionCoreDto>()
               .ToListAsync(cancellationToken);

        var totalCount = await queryOptions
            .RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<CalendarExclusionCoreDto>()
            .CountAsync(cancellationToken);

        return queryResult.ToPageable(queryOptions, totalCount);
    }

    internal sealed record Query(ODataQueryOptions<CalendarExclusionCoreDto> QueryOptions)
        : IQuery<PageableListOfItems<CalendarExclusionCoreDto>>;
}
