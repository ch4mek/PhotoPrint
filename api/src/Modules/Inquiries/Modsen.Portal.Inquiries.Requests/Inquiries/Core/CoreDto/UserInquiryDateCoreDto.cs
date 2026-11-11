using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

internal class UserInquiryDateCoreDto
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }
    public decimal RequestedDays { get; set; }
    public DateType DateType { get; set; }

    public bool IsExcludedFromInquiry { get; set; }
    public string? ExcludedDayName { get; set; }

}
