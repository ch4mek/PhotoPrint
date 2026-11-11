using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

internal interface IGetExistingReportRequestDbQuery : IDbQuery<GetExistingReportRequestDbRequest, GetExistingReportRequestDbResponse?>;

public record GetExistingReportRequestDbRequest(int UserId, string FilterHash);

public record GetExistingReportRequestDbResponse(string? FileId, double SizeInKilobytes);
