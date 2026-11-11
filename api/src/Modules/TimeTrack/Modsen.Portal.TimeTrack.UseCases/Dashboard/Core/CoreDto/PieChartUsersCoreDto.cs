namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

public class PieChartUsersCoreDto
{
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? Color { get; set; }
    public int TrackedTime { get; set; }
    public bool IsDeleted { get; set; }
}
