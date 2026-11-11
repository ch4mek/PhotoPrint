using System.Reflection;
using Modsen.Portal.StimulsoftReport.Contract;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;

namespace Modsen.Portal.StimulsoftReport;

public sealed class StimulsoftReportExporter : IReportExporter
{
    private const string EmployeeTemplateName = "employee_template.mrt";
    private const string ProjectTemplateName = "project_template.mrt";
    private const string ClientTemplateName = "client_template.mrt";
    private const string TotalTemplateName = "total_template.mrt";
    private const string DiscountTemplateName = "discount_template.mrt";
    private const string ArticleTemplateName = "article_template.mrt";
    private const string RootDataObjectName = "Projects";
    private const string TitleObjectName = "Title";

    public Task<byte[]> ExportPdfAsync(ReportRequest reportRequest, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(reportRequest);
        return ExportAsync(reportRequest, ReportExportType.Pdf);
    }

    public Task<byte[]> ExportWordAsync(ReportRequest reportRequest, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(reportRequest);
        return ExportAsync(reportRequest, ReportExportType.Word);
    }

    private static async Task<byte[]> ExportAsync(ReportRequest reportRequest, ReportExportType exportType)
    {
        ArgumentNullException.ThrowIfNull(reportRequest);
        using var report = new StiReport();
        using var memoryStream = new MemoryStream();

        await FillStiReportAsync(report, reportRequest);

        switch (exportType)
        {
            case ReportExportType.Pdf:
                ExportPdfToStream(report, memoryStream);
                break;
            case ReportExportType.Word:
                ExportWordToStream(report, memoryStream, reportRequest.ReportsAuthorName);
                break;
            default:
                throw new InvalidOperationException("Unsupported file format");
        }

        return memoryStream.ToArray();
    }

    private static async Task FillStiReportAsync(StiReport report, ReportRequest request)
    {
        var templateName = GetReportTemplateName(request.TemplateType);

        await using var template = LoadReportTemplate(templateName);

        report = report.Load(template);
        report.ReportAuthor = request.ReportsAuthorName;
        report.ReportName = request.ReportName;

        report.RegBusinessObject(TitleObjectName, request.TitleObject);
        report.RegBusinessObject(RootDataObjectName, request.RootDataObject);

        await report.RenderAsync();
    }

    private static Stream LoadReportTemplate(string templateName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var templateStream = assembly.GetManifestResourceStream(
            $"{assembly.GetName().Name}.Templates.{templateName}");

        return templateStream!;
    }

    private static void ExportPdfToStream(StiReport report, MemoryStream toStream)
    {
        var pdfExportService = new StiPdfExportService();
        var pdfExportSettings = new StiPdfExportSettings
        {
            PageRange = StiPagesRange.All,
            ImageQuality = 1f,
            ImageResolution = 200f,
            EmbeddedFonts = true
        };

        pdfExportService.ExportPdf(report, toStream, pdfExportSettings);
    }

    private static void ExportWordToStream(StiReport report, MemoryStream toStream, string reportsAuthorName)
    {
        var wordExportService = new StiWord2007ExportService();
        var wordExportSettings = new StiWord2007ExportSettings
        {
            RemoveEmptySpaceAtBottom = false,
            PageRange = StiPagesRange.All,
            ImageQuality = 1f,
            ImageResolution = 200f,
            CompanyString = reportsAuthorName
        };

        wordExportService.ExportWord(report, toStream, wordExportSettings);
    }

    private static string GetReportTemplateName(TemplateType templateType)
    {
        return templateType switch
        {
            TemplateType.PerEmployee => EmployeeTemplateName,
            TemplateType.PerProject => ProjectTemplateName,
            TemplateType.PerClient => ClientTemplateName,
            TemplateType.Total => TotalTemplateName,
            TemplateType.PerDiscount => DiscountTemplateName,
            TemplateType.PerArticle => ArticleTemplateName,
            _ => throw new InvalidOperationException("Unsupported template type")
        };
    }
}
