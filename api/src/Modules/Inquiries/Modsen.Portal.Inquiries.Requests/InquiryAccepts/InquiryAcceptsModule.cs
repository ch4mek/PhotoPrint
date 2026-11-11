using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.Queries;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts;

internal sealed class InquiryAcceptsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IGetInquiryAcceptsByInquiryIdDbQuery, GetInquiryAcceptsByInquiryIdDbQuery>();
        services.AddScoped<IInquiryAcceptLoader, InquiryAcceptLoader>();
        services.AddScoped<IHasUserAccessToInquiryAcceptsBusinessRule, HasUserAccessToInquiryAcceptsBusinessRule>();

        services.AddScoped<IUpdateUserInfoInquiryAcceptsResolver, UpdateUserInfoInquiryAcceptsResolver>();
        services.AddScoped<IUpdatePolicyInquiryAcceptsResolver, UpdatePolicyInquiryAcceptsResolver>();
        services.AddScoped<IUpdateUserAsInquiryForAcceptResolver, UpdateUserAsInquiryForAcceptResolver>();
        services.AddScoped<IUpdateUserAsApproverAcceptResolver, UpdateUserAsApproverAcceptResolver>();
        services.AddScoped<IUpdateProjectManagerAcceptResolver, UpdateProjectManagerAcceptResolver>();
        services.AddScoped<IUpdateUserAcceptsResolver, UpdateUserAcceptsResolver>();
        services.AddScoped<IGetSubstituteResolver, GetSubstituteResolver>();
        services.AddScoped<IGetSubstitutesResolver, GetSubstitutesResolver>();
        services.AddScoped<INextApproversToNotifyResolver, NextApproversToNotifyResolver>();
        services.AddScoped<IUserInquiryAcceptToApproveRejectResolver, UserInquiryAcceptToApproveRejectResolver>();

        services.AddScoped<IUserInquiryAcceptStatusComposer, UserInquiryAcceptStatusComposer>();
        services.AddScoped<INotifySubstitutesOnApproverAbsenceService, NotifySubstitutesOnApproverAbsenceService>();
    }
}
