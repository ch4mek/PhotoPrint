namespace Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

public class BarChartInfoResponseDto
{
    public int ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public string? ProjectColor { get; init; }
    public int? TotalTime { get; init; }
    public bool? IsBillableProject { get; set; }
}
