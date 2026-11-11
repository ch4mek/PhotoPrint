using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal sealed class GetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery
    : IGetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse>> ExecuteAsync
        (GetTimesheetByProjectIdAndUserIdAndDateIntervalDbRequest request, CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .Where(x => x.ProjectId == request.ProjectId && x.UserId == request.UserId && x.ReportForDate >= request.DateFrom);

        if (request.DateTo.HasValue)
        {
            query = query.Where(x => x.ReportForDate <= request.DateTo);
        }

        return query.OrderBy(x => x.ReportForDate)
            .Select(x => new GetTimesheetByProjectIdAndUserIdAndDateIntervalDbResponse(x.ProjectId, x.UserId,
                x.ReportForDate))
            .ToListAsync(cancellationToken);
    }
}
