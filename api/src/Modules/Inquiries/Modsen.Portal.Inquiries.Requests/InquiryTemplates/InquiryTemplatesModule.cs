using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates;

internal sealed class InquiryTemplatesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IInquiryTemplateLoader, InquiryTemplateLoader>();
        services.AddScoped<IInquiryTemplateValidator, InquiryTemplateValidator>();
        services.AddScoped<IAcceptPoliciesValidator, AcceptPoliciesValidator>();
        services.AddScoped<IAreAcceptPoliciesValidBusinessRule, AreAcceptPoliciesValidBusinessRule>();
        services.AddScoped<IIsUserApproverOrSubstituteBusinessRule, IsUserApproverOrSubstituteBusinessRule>();
        services.AddScoped<IUpdateTemplateAcceptPoliciesResolver, UpdateTemplateAcceptPoliciesResolver>();
    }
}
