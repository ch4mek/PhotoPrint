using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public class ClientWithTrackedTimeReportOutput
{
    public required string Name { get; init; }
    public string? Color { get; init; }
    public required int TotalTimeInMinutes { get; init; }

    internal static ClientWithTrackedTimeReportOutput Of(ClientWithTrackedTimeReportDto source)
    {
        return new ClientWithTrackedTimeReportOutput
        {
            Name = source.Name,
            TotalTimeInMinutes = source.TotalTimeInMinutes,
            Color = source.Color
        };
    }
}
