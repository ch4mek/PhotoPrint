using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class UserInquiryDateOutput
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }
    public decimal RequestedDays { get; set; }
    public DateType DateType { get; set; }

    public bool IsExcludedFromInquiry { get; set; }
    public string? ExcludedDayName { get; set; }

    public static UserInquiryDateOutput Of(UserInquiryDateCoreDto userInquiryDateCoreDto)
    {
        return new UserInquiryDateOutput
        {
            Id = userInquiryDateCoreDto.Id,
            Date = userInquiryDateCoreDto.Date,
            RequestedDays = userInquiryDateCoreDto.RequestedDays,
            DateType = userInquiryDateCoreDto.DateType,
            IsExcludedFromInquiry = userInquiryDateCoreDto.IsExcludedFromInquiry,
            ExcludedDayName = userInquiryDateCoreDto.ExcludedDayName
        };
    }
}
