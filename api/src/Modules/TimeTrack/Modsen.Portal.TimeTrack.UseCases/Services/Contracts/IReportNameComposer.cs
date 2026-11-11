using Modsen.Portal.Core;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

internal interface IReportNameComposer
{
    Task<string> ComposeForSingleFileAsync(
        ProcessReportRequest.Command request,
        CancellationToken cancellationToken);

    Task<string> ComposeForSingleFileAsync(
        int? userId,
        int? projectId,
        int? clientId,
        int? year,
        int[]? monthOfYear,
        DocumentType format,
        CancellationToken cancellationToken);

    Task<string> ComposeForPerUserArchiveAsync(
        ProcessReportRequest.Command request,
        CancellationToken cancellationToken);
}
