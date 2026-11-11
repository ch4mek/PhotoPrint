using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Loaders;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Queries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Agreements;

internal sealed class AgreementsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IAccrualAmountOfDaysCalculator, AccrualAmountOfDaysCalculator>();
        services.AddScoped<IAccrualTransactionDatesCalculator, AccrualTransactionDatesCalculator>();
        services.AddScoped<IUserContractEndDateCalculator, UserContractEndDateCalculator>();

        services.AddScoped<IGetDefaultAgreementByInquiryTypeDbQuery, GetDefaultAgreementByInquiryTypeDbQuery>();

        services.AddScoped<IAgreementTransactionsHistoryResolver, AgreementTransactionsHistoryResolver>();

        services.AddScoped<IAccrualUserAgreementTransactionComposer, AccrualUserAgreementTransactionComposer>();
        services.AddScoped<IDisposeUserAgreementTransactionComposer, DisposeUserAgreementTransactionComposer>();
        services.AddScoped<IUserAgreementTransactionComposer, UserAgreementTransactionComposer>();
        services.AddScoped<ICompensationTransactionComposer, CompensationTransactionComposer>();
        services.AddScoped<IDefaultAgreementsComposer, DefaultAgreementsComposer>();
        services.AddScoped<IDefaultInquiryTemplatesComposer, DefaultInquiryTemplatesComposer>();
        services.AddScoped<IChangedContractTransactionComposer, ChangedContractTransactionComposer>();
        services.AddScoped<IChangedAgreementTransactionComposer, ChangedAgreementTransactionComposer>();
        services.AddScoped<IResetUserAgreementBalanceComposer, ResetUserAgreementBalanceComposer>();

        services.AddScoped<IUsersToGenerateTransactionsResolver, UsersToGenerateTransactionsResolver>();

        services.AddScoped<IAgreementTransactionsLoader, AgreementTransactionsLoader>();
        services.AddScoped<IAgreementsLoader, AgreementsLoader>();
    }
}
