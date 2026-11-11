using System.Linq.Expressions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserPerformanceReview
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime? InterviewDateFromUtc { get; set; }
    public DateTime? InterviewDateToUtc { get; set; }

    internal UserPerformanceReviewType Type { get; set; }
    internal UserPerformanceReviewStatus Status { get; set; }

    public int? ConversationId { get; set; }

    internal static readonly Expression<Func<UserPerformanceReview, OdpUserPerformanceReview>> MapUserPerformanceReview =
        x => new OdpUserPerformanceReview
        {
            Id = x.Id,
            UserId = x.UserId,
            InterviewDateFromUtc = x.InterviewDateFromUtc,
            InterviewDateToUtc = x.InterviewDateToUtc,
            Type = x.Type,
            Status = x.Status,
            ConversationId = x.ConversationId
        };
}
