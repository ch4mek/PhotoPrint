namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;

public class TimeTrackPortalSettingsDto
{
    public string? SystemAdministrationDepartmentId { get; init; }
    public string? PmoDepartmentId { get; init; }
    public string? SalesDepartmentId { get; init; }
    public int BelarusLocationId { get; init; }
    public string? DefaultTimesheetReportName { get; init; }
    public string? ReportsAuthorName { get; init; }
    public string? DefaultTimeReportFileName { get; init; }
    public Excel? ExcelSettings {  get; init; }

    public class Excel
    {
        public string? SummaryReportWorksheet { get; init; }
        public string? SummaryWorkingHoursReportWorksheet { get; init; }
        public string? DefaultFontName { get; init; }
        public int BaseFontSize { get; init; }
        public string? NumberFormat { get; init; }
        public string? DefaultDateFormat { get; init; }
    }
}
