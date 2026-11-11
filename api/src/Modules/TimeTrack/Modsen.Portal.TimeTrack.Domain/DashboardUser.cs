namespace Modsen.Portal.TimeTrack.Domain;

public class DashboardUser
{
    public int UserId { get; set; }

    public required string UserName { get; set; }

    public string? Color { get; set; }

    public int CountOfProjects { get; set; }

    public int TotalTrackedInMinutes { get; set; }

    public bool IsActive { get; set; }
}
