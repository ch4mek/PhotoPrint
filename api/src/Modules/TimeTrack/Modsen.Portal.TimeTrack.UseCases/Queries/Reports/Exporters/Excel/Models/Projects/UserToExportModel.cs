namespace Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

public class UserToExportModel
{
    public int UserId { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? Department { get; set; }
    public IReadOnlyCollection<TasksPerDateToExportModel> Dates { get; set; } = [];
    public int TotalTimeInMinutes { get; set; }
}
