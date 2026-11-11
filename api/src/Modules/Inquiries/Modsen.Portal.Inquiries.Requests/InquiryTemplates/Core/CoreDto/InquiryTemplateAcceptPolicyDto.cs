using FluentValidation;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

internal class InquiryTemplateAcceptPolicyDto
{
    public int Id { get; init; }
    public InquiryAcceptPolicyScopes PolicyScope { get; init; }
    public int? UserId { get; init; }
    public InquiriesUserDto? User { get; init; }
    public int OrderWeight { get; set; }
    public string? UserFullName { get; set; }
    public InquiryAcceptPolicyScopes? SubstituteScope { get; set; }
    public int? SubstituteUserId { get; set; }
    public string? SubstituteUserFullName { get; set; }

    internal sealed class InputValidator : AbstractValidator<InquiryTemplateAcceptPolicyDto>
    {
        public InputValidator()
        {
            RuleFor(x => x.UserId).NotNull()
                .When(x => x.PolicyScope == InquiryAcceptPolicyScopes.User);

            RuleFor(x => x.UserId).Null()
                .When(x => x.PolicyScope != InquiryAcceptPolicyScopes.User);
        }
    }
}
