using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Resolvers;

internal interface IAgreementTransactionsHistoryResolver
{
    Task<int[]> ResolveAgreementTransactionIds(
        int userId,
        AgreementType agreementType,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken);
}

internal class AgreementTransactionsHistoryResolver : IAgreementTransactionsHistoryResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public AgreementTransactionsHistoryResolver(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<int[]> ResolveAgreementTransactionIds(
        int userId,
        AgreementType agreementType,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken)
    {
        var inquiryType = agreementType switch
        {
            AgreementType.ContractVacationAgreement => InquiryType.Vacation,
            AgreementType.SickDaysAgreement => InquiryType.SickDay,
            _ => InquiryType.Unknown
        };

        return await _inquiriesDbContext.UserAgreementTransactions
            .Where(x => x.UserId == userId)
            .Where(x => x.UserAgreement!.InquiryTemplateAgreement!.InquiryTemplate!.InquiryType == inquiryType)
            .WhereGreaterOrEqual(x => x.TransactionDate, startDate)
            .WhereLessOrEqual(x => x.TransactionDate, endDate)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }    
}

