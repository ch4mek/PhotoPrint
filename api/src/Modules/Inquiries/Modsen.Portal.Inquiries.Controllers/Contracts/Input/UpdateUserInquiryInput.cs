namespace Modsen.Portal.Inquiries.Controllers.Contracts.Input;

internal class UpdateUserInquiryInput
{
    public required string Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}
