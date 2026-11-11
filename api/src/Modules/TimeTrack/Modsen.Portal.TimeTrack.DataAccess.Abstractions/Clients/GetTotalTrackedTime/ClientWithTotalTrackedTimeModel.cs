namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients.GetTotalTrackedTime;

public class ClientWithTotalTrackedTimeModel
{
    public required string Name { get; init; }
    public string? Color { get; init; }
    public int TotalTimeInMinutes { get; init; }
}
