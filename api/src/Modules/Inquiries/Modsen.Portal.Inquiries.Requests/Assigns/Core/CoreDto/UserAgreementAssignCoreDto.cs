using System.Linq.Expressions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.CoreDto;

internal class UserAgreementAssignCoreDto
{
    public int Id { get; set; }
    public int AgreementTeamplateId { get; set; }

    public string? AvatarFileId { get; set; }
    public int UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? Position { get; set; }
    public string? DepartmentId { get; set; }
    public string? Department { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }

    internal static readonly Expression<Func<UserAgreementEntity, UserAgreementAssignCoreDto>> MapAssign =
        x => new UserAgreementAssignCoreDto
        {
            Id = x.Id,
            AgreementTeamplateId = x.InquiryAgreementTemplateId,
            UserId = x.UserId,
            UserFullName = x.User!.LastNameRu != null && x.User.FirstNameRu != null
                    ? x.User.LastNameRu + " " + x.User.FirstNameRu +
                      " (" + x.User.LastName + " " + x.User.FirstName + ")"
                    : x.User.LastName + " " + x.User.FirstName,
            DepartmentId = x.User.DepartmentId,
            Position = x.User.Position,
            AvatarFileId = x.User.AvatarFileId,
            UserFirstName = x.User.FirstName,
            UserLastName = x.User.LastName,
            Department = null,
            DateFrom = x.DateFrom,
            DateTo = x.DateTo
        };
}
