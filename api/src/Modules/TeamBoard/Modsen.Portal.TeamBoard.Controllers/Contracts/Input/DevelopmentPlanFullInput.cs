using System.Text.Json.Serialization;
using Modsen.Portal.Core.Api.Converters;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal class DevelopmentPlanFullInput
{
    public int? Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? DeadlineDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [JsonConverter(typeof(SideTrimmingConverter))]
    public string? Summary { get; set; }
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public DevelopmentPlanSource? Source { get; set; }
    public int? ConversationId { get; set; }

    public static DevelopmentPlanFullCoreDto Of(DevelopmentPlanFullInput input)
    {
        return new DevelopmentPlanFullCoreDto
        {
            Id = input.Id,
            StartDate = input.StartDate,
            DeadlineDate = input.DeadlineDate,
            EndDate = input.EndDate,
            Summary = input.Summary,
            Notes = input.Notes,
            UserId = input.UserId,
            Source = input.Source,
            ConversationId = input.ConversationId
        };
    }
}
