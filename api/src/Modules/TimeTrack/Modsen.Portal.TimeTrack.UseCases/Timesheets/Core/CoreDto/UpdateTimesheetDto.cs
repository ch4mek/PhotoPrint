using System.Text.Json.Serialization;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

public record UpdateTimesheetDto
{
    public int Id { get; init; }

    [JsonPropertyName("ftm")]
    public int FactTrackedInMinutes { get; init; }

    [JsonPropertyName("btm")]
    public int? BillableTrackedInMinutes { get; init; }
    public bool BillableTrackedTimeUpdated { get; init; }
    public int ProjectId { get; init; }
}
