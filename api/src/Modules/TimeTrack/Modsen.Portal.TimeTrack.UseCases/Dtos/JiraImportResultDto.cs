namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

public sealed class JiraImportResultDto
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public int Total { get; set; }
    public IReadOnlyCollection<string> ExceptionMessages { get; set; } = [];
}
