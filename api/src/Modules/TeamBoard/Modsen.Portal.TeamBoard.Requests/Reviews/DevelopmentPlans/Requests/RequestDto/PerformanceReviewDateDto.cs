using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

internal class PerformanceReviewDateDto
{
    public DateTime Date { get; set; }
    public PerformanceReviewDateType PerformanceReviewDateTypes { get; set; }
    public int ConversationId { get; set; }
}
