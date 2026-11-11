using FluentValidation;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Validators
{
    internal sealed class AcceptPolicyValidator : AbstractValidator<UpdateInquiryTemplateAcceptPolicyDto>
    {
        public AcceptPolicyValidator()
        {
            RuleFor(command => command.OrderWeight)
                .NotNull();

            When(command => command.Scope == InquiryAcceptPolicyScopes.User, () =>
            {
                RuleFor(command => command.UserId)
                    .NotNull();
            });

            When(command => command.Scope != InquiryAcceptPolicyScopes.User, () =>
            {
                RuleFor(command => command.UserId)
                    .Null();
            });

            When(command => command.SubstituteScope is null, () =>
            {
                RuleFor(command => command.SubstituteUserId)
                    .Null();
            });
        }
    }
}
