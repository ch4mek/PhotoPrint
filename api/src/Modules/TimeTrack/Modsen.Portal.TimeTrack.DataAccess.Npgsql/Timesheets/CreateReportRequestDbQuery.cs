using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal class CreateReportRequestDbQuery : ICreateReportRequestDbQuery
{
    private readonly ITimeTrackDbContext _trackDbContext;
    private readonly IClockService _clockService;

    public CreateReportRequestDbQuery(ITimeTrackDbContext trackDbContext,
        IClockService clockService)
    {
        _trackDbContext = trackDbContext;
        _clockService = clockService;
    }

    public async Task<Guid> ExecuteAsync(CreateReportRequestDbRequest request, CancellationToken cancellationToken)
    {
        var entityEntry = await _trackDbContext.ReportRequests.AddAsync(new ReportRequest
        {
            ReportName = request.ReportName,
            SizeInKilobytes = request.SizeInKilobytes,
            RequestedAtUtc = _clockService.UtcNow,
            RequestIsValidTill = request.RequestIsValidTill,
            RequestedById = request.UserId,
            ReportFileId = request.ReportFileId,
            FilterHash = request.FilterHash,
            FilterArgs = request.FilterArgs
        }, cancellationToken);

        await _trackDbContext.SaveChanges(cancellationToken);

        return entityEntry.Entity.Id;
    }
}
