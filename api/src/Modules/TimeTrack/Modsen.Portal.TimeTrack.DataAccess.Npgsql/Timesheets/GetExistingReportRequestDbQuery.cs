using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal class GetExistingReportRequestDbQuery : IGetExistingReportRequestDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IClockService _clockService;

    public GetExistingReportRequestDbQuery(ITimeTrackDbContext timeTrackDbContext,
        IClockService clockService)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _clockService = clockService;
    }

    public Task<GetExistingReportRequestDbResponse?> ExecuteAsync(
        GetExistingReportRequestDbRequest request, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.ReportRequests
            .Where(reportRequest => reportRequest.FilterHash == request.FilterHash &&
                                    reportRequest.RequestedById == request.UserId &&
                                    reportRequest.RequestIsValidTill > _clockService.UtcNow)
            .Select(reportRequest => new GetExistingReportRequestDbResponse(reportRequest.ReportFileId,
                reportRequest.SizeInKilobytes))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
