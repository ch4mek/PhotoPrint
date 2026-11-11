using System.Linq.Expressions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpPerformanceReview
{
    public int? Id { get; set; }
    public int UserId { get; set; }
    public int? ConversationId { get; set; }
    public DateTime? PerformanceReviewDate { get; set; }
    public string? Status { get; set; }
    public string? PerformanceReviewDateType { get; set; }
    public string? Theses { get; set; }
    public string? EmployeeTheses { get; set; }
    public string? Agreements { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }

    public IReadOnlyCollection<OdpConversationFile> OdpConversationFiles { get; set; } = [];
    public IReadOnlyCollection<OdpConversationDocumentLink> OdpConversationDocumentLinks { get; set; } = [];

    internal static readonly Expression<Func<UserPerformanceReview, OdpPerformanceReview>> MapPerformanceReview =
        x => new OdpPerformanceReview
        {
            Id = x.Id,
            UserId = x.UserId,
            PerformanceReviewDate = x.InterviewDateFromUtc,
            Status = x.Status.GetDescription(),
            PerformanceReviewDateType = x.Type.GetDescription(),
            Theses = x.ResponsibleUserFeedback,
            EmployeeTheses = x.EmployeeFeedback,
            Agreements = x.Agreements,
            UserDepartmentId = x.User!.DepartmentId,
            UserResourceGroupId = x.User.ResourceGroupId,
            ConversationId = x.ConversationId,

            OdpConversationFiles = x.Conversation!.ConversationFiles
                .AsQueryable()
                .Select(OdpConversationFile.MapConversationFile)
                .ToList(),
            OdpConversationDocumentLinks = x.Conversation.ConversationDocumentLinks
                .AsQueryable()
                .Select(OdpConversationDocumentLink.MapConversationDocumentLink)
                .ToList()
        };
}
