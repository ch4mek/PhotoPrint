using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Assigns;

public sealed class InquiriesAssignsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IUserInquiryTemplateComposer, UserInquiryTemplateComposer>();
        services.AddScoped<IResetUserAgreementsComposer, ResetUserAgreementsComposer>();

        services.AddScoped<ICanAccessUserAssignsBusinessRule, CanAccessUserAssignsBusinessRule>();

        services.AddScoped<IUserAgreementInquiryAvailableDaysCalculator,
            UserAgreementInquiryAvailableDaysCalculator>();

        services.AddScoped<IUnassignUserFromInquiryTemplatesResolver, UnassignUserFromInquiryTemplatesResolver>();
        services.AddScoped<IAssignDefaultInquiryTemplatesResolver, AssignDefaultInquiryTemplatesResolver>();
    }
}
