namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.GetWithTotalTrackedTime;

public class ProjectWithTotalTrackedTimeModel
{
    public required string Name { get; init; }

    public string? Color { get; init; }

    public int TotalTimeInMinutes { get; init; }
}
