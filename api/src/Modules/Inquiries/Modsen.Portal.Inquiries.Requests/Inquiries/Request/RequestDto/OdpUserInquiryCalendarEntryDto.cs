using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

public class OdpUserInquiryCalendarEntryDto
{
    [Key]
    public int UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? UserFullNameRu { get; set; }
    public string? UserPosition { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public bool IsManagedByRoleRequest { get; set; }
    public IReadOnlyCollection<UserInquiryBaseDto> Inquiries { get; init; } = [];

    internal static readonly Expression<Func<InquiriesUserEntity, OdpUserInquiryCalendarEntryDto>> Map =
        x => new OdpUserInquiryCalendarEntryDto
        {
            UserId = x.UserId,
            UserFullName = x.LastNameRu == null && x.FirstNameRu == null
                ? x.LastName + " " + x.FirstName
                : x.LastNameRu != null && x.FirstNameRu != null
                    ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                    : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")",
            UserFullNameRu = x.LastNameRu != null && x.FirstNameRu != null
                ? x.LastNameRu + " " + x.FirstNameRu
                : x.LastNameRu ?? x.FirstNameRu,
            UserPosition = x.Position,
            DepartmentId = x.DepartmentId,
            ResourceGroupId = x.ResourceGroupId
        };
}
