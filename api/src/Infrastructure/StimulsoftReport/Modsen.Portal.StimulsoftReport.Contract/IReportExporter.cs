namespace Modsen.Portal.StimulsoftReport.Contract;

public interface IReportExporter
{
    Task<byte[]> ExportPdfAsync(ReportRequest reportRequest, CancellationToken token);

    Task<byte[]> ExportWordAsync(ReportRequest reportRequest, CancellationToken token);
}
