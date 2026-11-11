using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.RequestDtos;

public sealed class PendingInquiryApprovalDto
{
    public required int UserId { get; init; }
    public string? DepartmentId { get; init; }
    public required int InquiryId { get; init; }
    public required int InquiryTemplateId { get; init; }
    public required string InquiryTemplateName { get; set; }
    public required string UserFullName { get; init; }
    public InquiryType InquiryType { get; init; }
    public DateOnly DateFrom { get; init; }
    public int? ResourceGroupId { get; init; }

    /// <summary>
    /// Property is used at the UI to define could the requesting (current) user see details of the specified user.
    /// This is used because sometimes manager from another department could still be required to approve the inquiry,
    /// but won't be able to view the actual user info
    /// </summary>
    public bool CanUserInfoBeRetrievedByCurrentUser { get; set; }
}
