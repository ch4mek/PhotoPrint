using System.Text.Json.Serialization;
using Modsen.Portal.Core.Api.Converters;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal class CreateDevelopmentPlanInput
{
    public DateOnly StartDate { get; set; }

    public DateOnly? DeadlineDate { get; set; }

    [JsonConverter(typeof(SideTrimmingConverter))]
    public string? Summary { get; set; }
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public int? ConversationId { get; set; }
}
