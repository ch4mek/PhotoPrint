using System.Linq.Expressions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

public class UserInquiryBaseDto
{
    public int Id { get; set; }
    public string? InquiryType { get; set; }
    public string? InquiryName { get; set; }
    public string? Status { get; set; }
    public int TotalAcceptUsersCount { get; set; }
    public int TotalAcceptedCount { get; set; }
    public decimal? TotalHours { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }

    internal static readonly Expression<Func<UserInquiryEntity, UserInquiryBaseDto>> Map =
        x => new UserInquiryBaseDto
        {
            Id = x.Id,
            InquiryType = x.InquiryTemplate!.InquiryType.GetDescription(),
            InquiryName = x.InquiryTemplate.Name,
            Status = x.Status.GetDescription(),
            TotalAcceptedCount = x.UserInquiryAccepts.Count,
            TotalAcceptUsersCount = x.UserInquiryAccepts.Count(f => f.Status == UserInquiryAcceptStatus.Accepted),
            TotalHours = x.TotalHours,
            DateFrom = x.DateFrom,
            DateTo = x.DateTo
        };
}
