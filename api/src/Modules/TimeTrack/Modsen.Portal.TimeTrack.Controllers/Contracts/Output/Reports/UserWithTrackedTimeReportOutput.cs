using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;

public sealed class UserWithTrackedTimeReportOutput
{
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? Color { get; init; }
    public bool IsActive { get; set; }
    public required int TotalTimeInMinutes { get; init; }

    internal static UserWithTrackedTimeReportOutput Of(UserWithTrackedTimeReportDto source)
    {
        return new UserWithTrackedTimeReportOutput
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            Color = source.Color,
            IsActive = source.IsActive,
            TotalTimeInMinutes = source.TotalTimeInMinutes
        };
    }
}
