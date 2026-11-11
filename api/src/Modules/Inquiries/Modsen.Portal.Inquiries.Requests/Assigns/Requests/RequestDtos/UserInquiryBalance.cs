using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos
{
    internal class UserInquiryBalance
    {
        public required int UserId { get; set; }
        public required string FullName { get; set; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? FullNameRu { get; set; }
        public string? FirstNameRu { get; set; }
        public string? LastNameRu { get; set; }
        public required string Email { get; set; }

        public required decimal Balance { get; set; }
        public required TrackingUnit TrackingUnit { get; set; }

        public required int InquiryTemplateId { get; set; }
        public required string InquiryTemplateName { get; set; }
        public required InquiryType InquiryType { get; set; }
    }
}
