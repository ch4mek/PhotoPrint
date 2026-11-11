namespace Modsen.Portal.StimulsoftReport.Contract;

public class ReportRequest
{
    public required string ReportName { get; init; }
    public required TemplateType TemplateType { get; init; }
    public required string ReportsAuthorName { get; init; }
    public object? TitleObject { get; set; }
    public object? RootDataObject { get; set; }
}
