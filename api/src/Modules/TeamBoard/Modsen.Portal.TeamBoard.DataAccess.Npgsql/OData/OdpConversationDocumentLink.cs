using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpConversationDocumentLink
{
    public int Id { get; set; }
    public string Link { get; set; } = null!;

    public int? ConversationId { get; set; }
    public int? PerformanceReviewId { get; set; }

    internal static readonly Expression<Func<ConversationDocumentLink, OdpConversationDocumentLink>> MapConversationDocumentLink =
        x => new OdpConversationDocumentLink
        {
            Id = x.Id,
            Link = x.Link,
            ConversationId = x.ConversationId,
            PerformanceReviewId = x.Conversation!.User!.UserPerformanceReviews
                .Where(pr => pr.ConversationId == x.ConversationId)
                .Select(f => (int?)f.Id).FirstOrDefault()
        };
}
