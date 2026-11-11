using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Builders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Validators;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;

namespace Modsen.Portal.Inquiries.Requests.Inquiries;

internal sealed class InquiriesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IInquiryDatesComposer, InquiryDatesComposer>();
        services.AddScoped<IInquiryAcceptsComposer, InquiryAcceptsComposer>();

        services.AddScoped<IUserInquiryBuilder, UserInquiryBuilder>();
        services.AddScoped<IUserAgreementBuilder, UserAgreementBuilder>();

        services.AddScoped<IUserInquiryLoader, UserInquiryLoader>();

        services.AddScoped<ITotalAccountantDaysCalculator, TotalAccountantDaysCalculator>();

        services.AddScoped<IIsAgreementValidForInquiryTemplateBusinessRule, IsAgreementValidForInquiryTemplateBusinessRule>();
        services.AddScoped<IHasUserAccessToInquiryBusinessRule, HasUserAccessToInquiryBusinessRule>();
        services.AddScoped<IHasUserAccessToMutateInquiryBusinessRule, HasUserAccessToMutateInquiryBusinessRule>();
        services.AddScoped<IIsUserApproverBusinessRule, IsUserApproverBusinessRule>();
        services.AddScoped<IIsEnoughAvailableDaysForInquiryBusinessRule, IsEnoughAvailableDaysForInquiryBusinessRule>();

        services.AddScoped<ICreateUserInquiryAgreementRequestValidator, CreateUserInquiryAgreementRequestValidator>();

        services.AddScoped<IInquiryAcceptedStatusResolver,  InquiryAcceptedStatusResolver>();
        services.AddScoped<IInquiryRejectedStatusResolver, InquiryRejectedStatusResolver>();
        services.AddScoped<IAvailableInquiryAcceptsResolver, AvailableInquiryAcceptsResolver>();
        services.AddScoped<IHistoricalRemainingBalanceDaysHoursResolver, HistoricalRemainingBalanceDaysHoursResolver>();
        services.AddScoped<IGetAbsentApproversResolver, GetAbsentApproversResolver>();
        services.AddScoped<ISetAvailabilityToApproveRejectResolver, SetAvailabilityToApproveRejectResolver>();
        services.AddScoped<IGetInquiriesCanBeApprovedAsSubstituteResolver, GetInquiriesCanBeApprovedAsSubstituteResolver>();
        services.AddScoped<IRejectInquiriesAfterUnassignResolver, RejectInquiriesAfterUnassignResolver>();
        services.AddScoped<IInquiriesPortalSettingsAccessor, InquiriesPortalSettings.InquiriesPortalSettingsAccessor>();
        services.AddScoped<ISetUserInfoVisibilityResolver, SetUserInfoVisibilityResolver>();
    }
}
