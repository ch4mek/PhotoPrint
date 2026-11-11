namespace Modsen.Portal.TimeTrack.UseCases.Reports.Core.Dto;

public class ClientWithTotalTrackedTimeCoreDto
{
    public required string Name { get; init; }
    public string? Color { get; init; }
    public int TotalTimeInMinutes { get; init; }
}

