namespace Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;

public class DashboardOfUserProjectsCoreDto
{
    public int ProjectId { get; set; }

    public required string ProjectName { get; set; }

    public string? UserColor { get; set; }

    public string? ProjectColor { get; set; }

    public int ClientId { get; set; }

    public required string ClientName { get; set; }

    public int TotalTrackedInMinutes { get; set; }

    public int TasksCount { get; set; }

    public bool UserIsActive { get; set; }
}
