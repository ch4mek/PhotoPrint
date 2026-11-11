namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

public class ProjectToExportModel
{
    public IReadOnlyCollection<UserToExportModel> Users { get; set; } = [];
    public int TotalTimeInMinutes { get; set; }
    public required string ProjectName { get; set; }
}
