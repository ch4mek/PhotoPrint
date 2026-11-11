using System.Linq.Expressions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpInterview
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFullName { get; set; }
    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get; set; }
    public string? UserFullNameRu { get; set; }
    public string? UserPosition { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public bool IsUserActive { get; set; }

    public required string Specialization { get; set; }

    public DateOnly? UserInterviewDate { get; set; }
    public TimeOnly? UserInterviewFromUtc { get; set; }

    public string? ClientName { get; set; }
    public UserInterviewStatus? Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public UserInterviewType? InterviewType { get; set; }
    public int InterviewTypeOrderWeight { get; set; }

    public int? SalesResponsibleUserId { get; set; }
    public string? SalesResponsibleUserFirstName { get; set; }
    public string? SalesResponsibleUserLastName { get;set; }
    public string? SalesResponsibleUserFullName { get; set; }
    public string? SalesResponsibleUserFirstNameRu { get; set; }
    public string? SalesResponsibleUserLastNameRu { get; set; }
    public string? SalesResponsibleUserFullNameRu { get; set; }

    public string? JiraTask { get; set; }
    public string? Notes { get; set; }
    public string? ReasonForRefusal { get; set; }

    public Guid GroupId { get; set; }


    internal static readonly Expression<Func<UserInterview, OdpInterview>> Map =
        x => new OdpInterview
        {
            Id = x.Id,
            UserId = x.UserId,
            UserFirstName = x.User!.FirstName,
            UserLastName = x.User.LastName,
            UserFullName = x.User.LastName + " " + x.User.FirstName,
            UserFirstNameRu = x.User.FirstNameRu,
            UserLastNameRu = x.User.LastNameRu,
            UserFullNameRu = x.User.LastNameRu == null && x.User.FirstNameRu == null
                ? null
                : (x.User.LastNameRu + " " + x.User.FirstNameRu).Trim(),

            UserDepartmentId = x.User.DepartmentId,
            UserResourceGroupId = x.User.ResourceGroupId,
            UserPosition = x.User.Position,
            IsUserActive = x.User.IsActive,
            Specialization = x.Specialization,
            UserInterviewDate = x.UserInterviewDate,
            UserInterviewFromUtc = x.UserInterviewFromUtc,
            ClientName = x.ClientName,
            Status = x.Status,
            StatusOrderWeight = x.StatusOrderWeight,
            InterviewType = x.InterviewType,
            InterviewTypeOrderWeight = x.InterviewTypeOrderWeight,
            SalesResponsibleUserId = x.SalesResponsibleUserId,
            SalesResponsibleUserFirstName = x.SalesResponsibleUserId != null ? x.SalesResponsibleUser!.FirstName : null,
            SalesResponsibleUserLastName = x.SalesResponsibleUserId != null ? x.SalesResponsibleUser!.LastName : null,
            SalesResponsibleUserFullName = x.SalesResponsibleUserId != null
                ? x.SalesResponsibleUser!.LastName + " " + x.SalesResponsibleUser!.FirstName
                : null, 

            SalesResponsibleUserFirstNameRu = x.SalesResponsibleUserId != null ? x.SalesResponsibleUser!.FirstNameRu : null,
            SalesResponsibleUserLastNameRu = x.SalesResponsibleUserId != null ? x.SalesResponsibleUser!.LastNameRu : null,

            SalesResponsibleUserFullNameRu =
                x.SalesResponsibleUserId == null || (x.SalesResponsibleUser!.LastNameRu == null && x.SalesResponsibleUser.FirstNameRu == null)
                ? null
                : (x.SalesResponsibleUser!.LastNameRu + " " + x.SalesResponsibleUser!.FirstNameRu).Trim(),
            JiraTask = x.JiraTask,
            Notes = x.Notes,
            ReasonForRefusal = x.ReasonForRefusal,
            GroupId = x.GroupId,
        };
}
