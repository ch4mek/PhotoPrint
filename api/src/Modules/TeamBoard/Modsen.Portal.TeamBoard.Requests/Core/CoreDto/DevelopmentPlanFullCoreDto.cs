using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal sealed class DevelopmentPlanFullCoreDto
{
    public int? Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? DeadlineDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Summary { get; set; }
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public DevelopmentPlanSource? Source { get; set; }
    public int? ConversationId { get; set; }
}
