using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

public interface ICreateReportRequestDbQuery : IDbQuery<CreateReportRequestDbRequest, Guid>;

public record CreateReportRequestDbRequest(string ReportName, double SizeInKilobytes,
    DateTime RequestIsValidTill, int UserId, string ReportFileId, string FilterHash, string FilterArgs);
