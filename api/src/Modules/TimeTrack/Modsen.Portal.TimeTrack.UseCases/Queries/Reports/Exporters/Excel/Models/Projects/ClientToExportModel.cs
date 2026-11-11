namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

public class ClientToExportModel
{
    public required string Name { get; set; }
    public IReadOnlyCollection<ProjectToExportModel> Projects { get; set; } = [];
    public int TotalTimeInMinutes { get; set; }
}
