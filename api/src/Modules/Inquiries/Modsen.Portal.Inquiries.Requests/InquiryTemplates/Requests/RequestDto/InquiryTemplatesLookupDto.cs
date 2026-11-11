using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests.RequestDto
{
    public class InquiryTemplatesLookupDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public InquiryType InquiryType { get; set; }
    }
}
