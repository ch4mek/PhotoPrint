using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

public class ProjectWithDatesToExportModel
{
    public required string ClientName { get; set; }
    public required ProjectToExportModel Project { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public required string ReportsAuthorName { get; set; }
    public required TimeTrackPortalSettingsDto.Excel ExcelConfig { get; init; }
}
