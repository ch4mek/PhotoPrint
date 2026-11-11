using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpConversation
{
    public int Id { get; set; }
    public DateOnly? Date { get; set; }

    public required string ConversationType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ConversationResult? ConversationResult { get; set; }
    public string? Theses { get; set; }
    public string? EmployeeTheses { get; set; }
    public string? Agreements { get; set; }

    public int UserId { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public required string UserFullName { get; set; }

    public int SupervisorId { get; set; }
    public required string SupervisorFullName { get; set; }
    public required string SupervisorFirstName { get; set; }
    public required string SupervisorLastName { get; set; }

    public DateTime? PerformanceReviewDate { get; set; }
    public UserPerformanceReviewStatus? Status { get; set; }

    public IReadOnlyCollection<OdpConversationFile> OdpConversationFiles { get; set; } = [];
    public IReadOnlyCollection<OdpConversationDocumentLink> OdpConversationDocumentLinks { get; set; } = [];

    internal static readonly Expression<Func<Conversation, OdpConversation>> MapConversation =
        x => new OdpConversation
        {
            Id = x.Id,
            Date = x.Date,
            ConversationType = x.ConversationType.ToString(),
            ConversationResult = x.ConversationResult,
            Theses = x.Theses,
            EmployeeTheses = x.EmployeeTheses,
            Agreements = x.Agreements,
            UserId = x.UserId,
            UserDepartmentId = x.User!.DepartmentId,
            UserResourceGroupId = x.User.ResourceGroupId,
            UserFirstName = x.User.FirstName,
            UserLastName = x.User.LastName,
            UserFullName = x.User.LastName + ' ' + x.User.FirstName,
            SupervisorId = x.SupervisorId,
            SupervisorFirstName = x.Supervisor!.FirstName,
            SupervisorLastName = x.Supervisor.LastName,
            SupervisorFullName = x.Supervisor.LastName + ' ' + x.Supervisor.FirstName,
            PerformanceReviewDate = x.User.UserPerformanceReviews
                .Where(pr => pr.ConversationId == x.Id)
                .Select(x => x.InterviewDateFromUtc).FirstOrDefault(),
            Status = x.User.UserPerformanceReviews
                .Where(pr => pr.ConversationId == x.Id)
                .Select(x => (UserPerformanceReviewStatus?)x.Status)
                .FirstOrDefault(),

            OdpConversationFiles = x.ConversationFiles
                .AsQueryable()
                .OrderBy(file => file.Id)
                .Select(OdpConversationFile.MapConversationFile)
                .ToList(),

            OdpConversationDocumentLinks = x.ConversationDocumentLinks
                .AsQueryable()
                .OrderBy(link => link.Id)
                .Select(OdpConversationDocumentLink.MapConversationDocumentLink)
                .ToList()
        };
}
