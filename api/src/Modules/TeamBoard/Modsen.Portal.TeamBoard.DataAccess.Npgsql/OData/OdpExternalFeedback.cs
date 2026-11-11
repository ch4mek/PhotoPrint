using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpExternalFeedback
{
    public int Id { get; set; }
    public DateOnly ExternalFeedbackDate { get; set; }

    public string? Feedback { get; set; }
    public string? ActionsTodo { get; set; }

    public string? ExternalFeedbackType { get; set; }

    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ClientName { get; set; }

    public int? ConversationId { get; set; }

    public int UserId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserDepartmentId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFullName { get; set; }

    public string? Reviewer { get; set; }

    internal static readonly Expression<Func<ExternalFeedback, OdpExternalFeedback>> MapExternalFeedback =
        x => new OdpExternalFeedback
        {
            Id = x.Id,
            ExternalFeedbackDate = x.ExternalFeedbackDate,
            Feedback = x.Feedback,
            ActionsTodo = x.ActionsTodo,
            ExternalFeedbackType = x.ExternalFeedbackType.ToString(),
            ConversationId = x.ConversationId,

            ProjectId = x.ProjectId,
            ProjectName = x.Project!.ProjectName,
            ClientName = x.Project.ClientName,

            UserId = x.UserId,
            UserResourceGroupId = x.User!.ResourceGroupId,
            UserDepartmentId = x.User.DepartmentId,
            UserFirstName = x.User.FirstName,
            UserLastName = x.User.LastName,
            UserFullName = x.User.FirstName + " " + x.User.LastName,
            Reviewer = x.Reviewer
        };
}
