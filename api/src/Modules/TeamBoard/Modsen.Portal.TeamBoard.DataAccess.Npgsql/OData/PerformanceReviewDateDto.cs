using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class PerformanceReviewDateDto
{
    public int PerformanceReviewId { get; set; }
    public DateTime Date { get; set; }
    public PerformanceReviewDateType PerformanceReviewDateType { get; set; }
    public int? ConversationId { get; set; }
}
