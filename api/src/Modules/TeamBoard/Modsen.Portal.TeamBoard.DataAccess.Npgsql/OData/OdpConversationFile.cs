using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpConversationFile
{
    public int Id { get; set; }

    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public required string FileType { get; set; }
    public required long FileSize { get; set; }

    public int? ConversationId { get; set; }
    public int? PerformanceReviewId { get; set; }

    internal static readonly Expression<Func<ConversationFile, OdpConversationFile>> MapConversationFile =
        x => new OdpConversationFile
        {
            Id = x.Id,
            FileId = x.FileId,
            FileName = Path.GetFileNameWithoutExtension(x.FileName),
            FileType = Path.GetExtension(x.FileName),
            FileSize = x.FileSize,            
            ConversationId = x.ConversationId,
            PerformanceReviewId = x.Conversation!.User!.UserPerformanceReviews
                .Where(pr => pr.ConversationId == x.ConversationId)
                .Select(x => (int?)x.Id).FirstOrDefault()
        };
}
